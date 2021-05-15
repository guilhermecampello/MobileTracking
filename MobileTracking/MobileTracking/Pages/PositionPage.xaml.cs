using MobileTracking.Core;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using MobileTracking.Pages.Views;
using MobileTracking.Services;
using MobileTracking.Services.MagneticField;
using Plugin.Toast;
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
    public partial class PositionPage : ContentPage
    {
        private IWifiConnector wifiConnector;

        private MagneticFieldSensor magneticFieldSensor;

        private IBluetoothConnector bluetoothConnector;

        private ICalibrationService calibrationsService;

        private LocaleProvider localeProvider;

        private Configuration configuration;

        private IPositionDataService positionDataService;

        private Timer timer;

        private int count = 0;

        public PositionPage(Position position)
        {
            InitializeComponent();
            InitializeServices();
            this.Position = position;

            PositionDataCollection.RefreshCommand = RefreshData_Command;
            PositionDataCollection.ItemsSource = PositionData;
            Position.PositionData?.ForEach(data => PositionData.Add(new PositionDataView(data)));

            BindingContext = this;
            timer = new Timer(configuration!.DataAquisitionInterval * 1000);
            timer.Elapsed += UpdateMonitoringState;
            timer.Start();
            countLabel.Text = count.ToString();
        }

        public void InitializeServices()
        {
            var serviceProvider = Startup.ServiceProvider;
            this.wifiConnector = serviceProvider.GetService<IWifiConnector>();
            this.bluetoothConnector = serviceProvider.GetService<IBluetoothConnector>();
            this.magneticFieldSensor = serviceProvider.GetService<MagneticFieldSensor>();
            this.calibrationsService = serviceProvider.GetService<ICalibrationService>();
            this.positionDataService = serviceProvider.GetService<IPositionDataService>();
            this.localeProvider = serviceProvider.GetService<LocaleProvider>();
            this.configuration = serviceProvider.GetService<Configuration>();
        }

        public Position Position { get; set; }

        public ObservableCollection<PositionDataView> PositionData { get; set; } = new ObservableCollection<PositionDataView>();

        public Configuration Configuration { get => this.configuration; }

        public ICommand RefreshData_Command
        {
            get => new Command(async () =>
            {
                PositionDataCollection.IsRefreshing = true;
                await RefreshData();
                PositionDataCollection.IsRefreshing = false;
            });
        }

        public async Task RefreshData()
        {
            var query = new PositionDataQuery()
            {
                PositionId = Position.Id
            };
            try
            {
                if (await positionDataService.RecalculatePositionData(query))
                {
                    var newData = await positionDataService.GetPositionDatas(query);
                    this.PositionData.Clear();
                    newData.ForEach(data => PositionData.Add(new PositionDataView(data)));
                }
            }
            catch (Exception e)
            {
                await DisplayAlert(e.Message, e.InnerException.Message, "OK");
            }
        }

        public void UpdateCollectingDataState()
        {
            if (_isCollecting)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    calibrationButton.Text = AppResources.Stop_calibration;
                    calibrationButton.BackgroundColor = Color.Red;
                    calibrationButton.TextColor = Color.White;
                    activity.IsRunning = true;
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    calibrationButton.Text = AppResources.Start_calibration;
                    calibrationButton.BackgroundColor = Color.Default;
                    calibrationButton.TextColor = Color.Black;
                    activity.IsRunning = false;
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
                var command = new CreateCalibrationsCommand()
                {
                    Measurements = data,
                    PositionId = Position.Id
                };
                try
                {
                    await calibrationsService.CreateCalibrations(command);
                    count++;
                }
                catch(Exception ex)
                {
                    await DisplayAlert(ex.Message, ex.InnerException.Message, "OK");
                }

                if (count > configuration.SamplesPerPosition)
                {
                    count = 0;
                    IsCollecting = false;
                }
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                countLabel.Text = count.ToString();
            });
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
                var measurement = MeasurementsFactory.CreateWifiMeasurement(device.Key, (int)Math.Round(device.Value));
                data.Add(measurement);
            });

            this.bluetoothConnector.DevicesResults.ToList().ForEach(device =>
            {
                var measurement = MeasurementsFactory.CreateBluetoothMeasurement(device.Key, device.Value.Rssi);
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

        private void Calibration_Button_Clicked(object sender, EventArgs e)
        {
            if (!_isCollecting)
            {
                StartDataAquisition(); 
            }
            else
            {
                StopDataAquisition();
            }
        }

        private async void DeletePosition_Clicked(object sender, EventArgs e)
        {
            var deleteConfirmation = await DisplayAlert(AppResources.Delete_position + " " + Position.Name, AppResources.Confirm_delete_position, AppResources.Delete, AppResources.Cancel);
            if (deleteConfirmation)
            {
                var positionService = Startup.ServiceProvider.GetService<IPositionService>();
                
                if (await positionService.DeletePosition(Position.Id))
                {
                    await localeProvider.RefreshLocale();
                    CrossToastPopUp.Current.ShowToastError($"{AppResources.Position} {Position.Name} {AppResources.Deleted.ToLower()}");
                }
                await Navigation.PopAsync();
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
    }
}