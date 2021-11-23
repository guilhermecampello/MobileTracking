using MobileTracking.Core;
using MobileTracking.Core.Application;
using MobileTracking.Core.Interfaces;
using MobileTracking.Core.Models;
using MobileTracking.Pages.Positions;
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
        private readonly IWifiConnector wifiConnector;

        private readonly MagneticFieldSensor magneticFieldSensor;

        private readonly IBluetoothConnector bluetoothConnector;

        private readonly ICalibrationService calibrationsService;

        private readonly IPositionEstimationService positionEstimationService;

        private readonly LocaleProvider localeProvider;

        private readonly Configuration configuration;

        private readonly IPositionSignalDataService positionSignalDataService;

        private Timer timer;

        private int count = 0;

        public PositionPage(Position position)
        {
            InitializeComponent();
            var serviceProvider = Startup.ServiceProvider;
            this.wifiConnector = serviceProvider.GetService<IWifiConnector>();
            this.bluetoothConnector = serviceProvider.GetService<IBluetoothConnector>();
            this.magneticFieldSensor = serviceProvider.GetService<MagneticFieldSensor>();
            this.calibrationsService = serviceProvider.GetService<ICalibrationService>();
            this.positionSignalDataService = serviceProvider.GetService<IPositionSignalDataService>();
            this.positionEstimationService = serviceProvider.GetService<IPositionEstimationService>();
            this.localeProvider = serviceProvider.GetService<LocaleProvider>();
            this.configuration = serviceProvider.GetService<Configuration>();
            this.Position = position;

            PositionSignalDataCollection.RefreshCommand = RefreshData_Command;
            PositionSignalDataCollection.ItemsSource = PositionSignalData;
            Position.PositionSignalData?
                .OrderBy(data => data.SignalType)
                .ThenByDescending(data => data.Strength)
                .ToList()
                .ForEach(data => PositionSignalData.Add(new PositionSignalDataView(data)));

            BindingContext = this;
            timer = new Timer(configuration!.DataAquisitionInterval * 1000);
            timer.Elapsed += UpdateMonitoringState;
            timer.Start();
            countLabel.Text = count.ToString();
        }

        public Position Position { get; set; }

        public ObservableCollection<PositionSignalDataView> PositionSignalData { get; set; } = new ObservableCollection<PositionSignalDataView>();

        public Configuration Configuration { get => this.configuration; }

        public ICommand RefreshData_Command
        {
            get => new Command(async () =>
            {
                await RefreshData();
            });
        }

        public async Task RefreshData()
        {
            PositionSignalDataCollection.IsPullToRefreshEnabled = true;
            PositionSignalDataCollection.IsRefreshing = true;
            var query = new PositionSignalDataQuery()
            {
                PositionId = Position.Id
            };
            try
            {
                if (await positionSignalDataService.RecalculatePositionSignalData(query))
                {
                    var newData = await positionSignalDataService.GetPositionSignalDatas(query);
                    this.PositionSignalData.Clear();
                    newData
                        .OrderBy(data => data.SignalType)
                        .ThenByDescending(data => data.Strength)
                        .ToList()
                        .ForEach(data => PositionSignalData.Add(new PositionSignalDataView(data)));
                }
            }
            catch (Exception e)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(e.Message, e.InnerException.Message, "OK");
                });
            }
            finally
            {
                PositionSignalDataCollection.IsPullToRefreshEnabled = false;
                PositionSignalDataCollection.IsRefreshing = false;
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
                    if (testingSwitch.IsToggled)
                    {
                        _ = positionEstimationService.EstimatePosition(new Core.Commands.EstimatePositionCommand()
                        {
                            LocaleId = localeProvider.Locale!.Id,
                            Measurements = data,
                            RealX = Position.X,
                            RealY = Position.Y,
                            Neighbours = configuration.K
                        });
                    }
                    count++;
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await DisplayAlert(ex.Message, ex.InnerException?.Message, "OK");
                    });
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
                catch(Exception ex)
                {
                    await DisplayAlert(ex.Message, ex.InnerException.Message, "OK");
                }
            }
            else
            {
                StopDataAquisition();
                await RefreshData();
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

        private async void PositionSignalDataCollection_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var positionSignalData = ((PositionSignalDataView)e.Item).PositionSignalData;
            var query = new CalibrationsQuery()
            {
                PositionId = positionSignalData.PositionId,
                SignalId = positionSignalData.SignalId,
                SignalType = positionSignalData.SignalType,
            };
            try
            {
                var calibrations = await calibrationsService.GetCalibrations(query);
                await Navigation.PushAsync(new PositionCalibrationsPage(Position, calibrations));
            }
            catch (Exception ex)
            {
                await DisplayAlert(ex.Message, ex.InnerException.Message, "OK");
            }
        }

        private async void ResetDataButton_Clicked(object sender, EventArgs e)
        {
            var deleteConfirmation = await DisplayAlert(AppResources.Reset_data + " ", AppResources.Confirm_delete_data, AppResources.Delete, AppResources.Cancel);
            if (deleteConfirmation)
            {
                try
                {
                    var query = new CalibrationsQuery() { PositionId = Position.Id }; 
                    if (await calibrationsService.DeleteCalibrations(query))
                    {
                        CrossToastPopUp.Current.ShowToastError($"{AppResources.Data_deleted}");
                        await RefreshData();
                    }
                }
                catch(Exception ex)
                {
                    await DisplayAlert(ex.Message, ex.InnerException.Message, "OK");
                }
            }
        }
    }
}