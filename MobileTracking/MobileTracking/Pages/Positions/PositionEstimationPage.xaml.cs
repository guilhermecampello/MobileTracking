using MobileTracking.Core;
using MobileTracking.Core.Application;
using MobileTracking.Core.Commands;
using MobileTracking.Core.Interfaces;
using MobileTracking.Core.Models;
using MobileTracking.Pages.Views;
using MobileTracking.Services;
using MobileTracking.Services.MagneticField;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileTracking.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PositionEstimationPage : ContentPage
    {
        private readonly IWifiConnector wifiConnector;

        private readonly MagneticFieldSensor magneticFieldSensor;

        private readonly IBluetoothConnector bluetoothConnector;

        private readonly IPositionEstimationService positionEstimationService;

        private readonly IPositionService positionsService;

        private readonly Configuration configuration;

        private Timer timer;

        public PositionEstimationPage(Locale locale)
        {
            InitializeComponent();
            var serviceProvider = Startup.ServiceProvider;
            this.wifiConnector = serviceProvider.GetService<IWifiConnector>();
            this.bluetoothConnector = serviceProvider.GetService<IBluetoothConnector>();
            this.magneticFieldSensor = serviceProvider.GetService<MagneticFieldSensor>();
            this.positionEstimationService = serviceProvider.GetService<IPositionEstimationService>();
            this.positionsService = serviceProvider.GetService<IPositionService>();
            this.configuration = serviceProvider.GetService<Configuration>();
            this.Locale = locale;

            PositionDataCollection.ItemsSource = PositionData;

            BindingContext = this;
            timer = new Timer(5000);
            timer.Elapsed += UpdateMonitoringState;
            timer.Start();
        }

        public Locale Locale { get; set; }

        public ObservableCollection<CalibrationView> PositionData { get; set; } = new ObservableCollection<CalibrationView>();

        public ObservableCollection<PositionEstimationView> PositionEstimations { get; set; } = new ObservableCollection<PositionEstimationView>();

        public Configuration Configuration { get => this.configuration; }

     
        public void RefreshMeasurements(List<Measurement> measurements)
        {
            PositionDataCollection.IsRefreshing = true;
            try
            {
                this.PositionData.Clear();
                measurements
                .OrderBy(data => data.SignalType)
                .ThenByDescending(data => data.Strength)
                .ToList()
                .ForEach(data => PositionData.Add(new CalibrationView(new Calibration(0, data))));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                PositionDataCollection.IsRefreshing = false;
            }
        }

        public void UpdateCollectingDataState()
        {
            if (_isCollecting)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    calibrationButton.Text = AppResources.Stop;
                    calibrationButton.BackgroundColor = Color.Red;
                    calibrationButton.TextColor = Color.White;
                    activity.IsRunning = true;
                    activity.IsVisible = true;
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    calibrationButton.Text = AppResources.Estimate_position;
                    calibrationButton.BackgroundColor = Color.Default;
                    calibrationButton.TextColor = Color.Black;
                    activity.IsRunning = false;
                    activity.IsVisible = false;
                });
            }
        }

        public string BluetoothState
        {
            get
            {
                Resources.TryGetValue(bluetoothConnector.State.ToString(), out var translatedState);
                return (string?)translatedState ?? bluetoothConnector.State.ToString();
            }
        }

        public string WifiState
        {
            get
            {
                Resources.TryGetValue(wifiConnector.State.ToString(), out var translatedState);
                return (string?)translatedState ?? wifiConnector.State.ToString();
            }
        }

        public string MagnetometerState
        {
            get
            {
                Resources.TryGetValue(magneticFieldSensor.State.ToString(), out var translatedState);
                return (string?)translatedState ?? magneticFieldSensor.State.ToString();
            }
        }

        public Color BluetoothStateColor { get => GetStateColor(bluetoothConnector.State); }

        public Color WifiStateColor { get => GetStateColor(wifiConnector.State); }

        public Color MagnetometerStateColor { get => GetStateColor(magneticFieldSensor.State); }

        public async void UpdateMonitoringState(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            Device.BeginInvokeOnMainThread(() =>
            {
                wifiColor.BackgroundColor = WifiStateColor;
                wifiState.Text = WifiState;
                bluetoothColor.BackgroundColor = BluetoothStateColor;
                bluetoothState.Text = BluetoothState;
                magnetometerColor.BackgroundColor = MagnetometerStateColor;
                magnetometerState.Text = MagnetometerState;
            });
            if (IsCollecting)
            {
                var data = FetchCalibrationData();
                var command = new EstimatePositionCommand()
                {
                    Measurements = data,
                    LocaleId = Locale.Id
                };
                try
                {
                    Device.BeginInvokeOnMainThread(() => RefreshMeasurements(data));
                    var result = await positionEstimationService.EstimatePosition(command);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        PositionEstimations.Clear();
                        result.ForEach(position => PositionEstimations.Add(new PositionEstimationView(position)));
                    });
                    StopDataAquisition();
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(ex.Message, ex.InnerException?.Message, "OK");
                    });
                }
            }
        }

        private bool _isCollecting;

        public bool IsCollecting
        {
            get => _isCollecting;
            set
            {
                _isCollecting = value;
                UpdateCollectingDataState();
            }
        }

        private List<Measurement> FetchCalibrationData()
        {
            var data = new List<Measurement>();
            this.wifiConnector.ScanResults.ToList().ForEach(device =>
            {
                var measurement = new Measurement(device.Key, device.Value.Rssi, device.Value.SignalType, device.Value.CreatedAt);
                data.Add(measurement);
            });

            this.bluetoothConnector.DevicesResults.ToList().ForEach(device =>
            {
                var measurement = new Measurement(device.Key, device.Value.Rssi, device.Value.SignalType, device.Value.CreatedAt);
                data.Add(measurement);
            });

            var magneticFieldVector = magneticFieldSensor.TryCalculateMagneticFieldVector();
            if (magneticFieldVector.HasValue)
            {
                data.Add(MeasurementsFactory.CreateMagnetometerMeasurement(magneticFieldVector.Value));
            }

            return data;
        }

        private Color GetStateColor(MonitoringState state)
        {
            switch (state)
            {
                case MonitoringState.Unavailable:
                    return Color.Red;
                case MonitoringState.Available:
                    return Color.Green;
                default:
                    return Color.FromHex("#2196F3");
            }
        }

        private async void Calibration_Button_Clicked(object sender, EventArgs e)
        {
            if (!_isCollecting)
            {
                try
                {
                    StartDataAquisition();
                }
                catch (Exception ex)
                {
                    await DisplayAlert(ex.Message, ex.InnerException?.Message, "OK");
                }
            }
            else
            {
                StopDataAquisition();
                var data = FetchCalibrationData();
                RefreshMeasurements(data);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StopDataAquisition();
        }

        public void StopDataAquisition()
        {
            IsCollecting = false;
            this.timer.Stop();
        }

        public void StartDataAquisition()
        {
            
            magneticFieldSensor.Start();           
            bluetoothConnector.StartScanning();
            wifiConnector.StartScanning();
            IsCollecting = true;
            this.timer.Start();
        }

        private async void PositionEstimationsCollection_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var positionEstimation = (PositionEstimationView)e.Item;
            var positionId = positionEstimation.PositionEstimation.Position.Id;
            try
            {
                var query = new PositionQuery()
                {
                    IncludeData = true,
                    IncludeZone = true
                };
                var position = await positionsService.FindPositionById(positionId, query);
                await Navigation.PushAsync(new PositionPage(position));
            }
            catch(Exception ex)
            {
                await DisplayAlert(ex.Message, ex.InnerException?.Message, "OK");
            }
        }
    }
}