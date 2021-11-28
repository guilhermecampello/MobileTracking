using MobileTracking.Charts;
using MobileTracking.Core;
using MobileTracking.Core.Application;
using MobileTracking.Core.Commands;
using MobileTracking.Core.Interfaces;
using MobileTracking.Core.Models;
using MobileTracking.Pages.Views;
using MobileTracking.Services;
using MobileTracking.Services.MagneticField;
using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
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

        private SfChart chart;

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

            PositionSignalDataCollection.ItemsSource = PositionSignalData;

            BindingContext = this;
            timer = new Timer(configuration.DataAquisitionInterval * 1000);
            timer.Elapsed += UpdateMonitoringState;
            timer.Start();

            var positions = new List<Position>();
            Locale.Zones?.ForEach(zone => positions.AddRange(zone.Positions));
            chart = ChartsFactory.CreatePositionSignalDataScatterChart(AppResources.Current_locale, "", "", positions, "X", "Y", showError: false, markerLabel: "Id", height: 1200 + ((int)Math.Floor(positions.Count / 2.0) * 30));
            chartStack.Children.Add(chart);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                StartDataAquisition();
            }
            catch (Exception ex)
            {
                await DisplayAlert(ex.Message, ex.InnerException?.Message, "OK");
            }
        }

        public Locale Locale { get; set; }

        public ObservableCollection<CalibrationView> PositionSignalData { get; set; } = new ObservableCollection<CalibrationView>();

        public ObservableCollection<PositionEstimationView> PositionEstimations { get; set; } = new ObservableCollection<PositionEstimationView>();

        public Configuration Configuration { get => this.configuration; }

        public void RefreshMeasurements(List<Measurement> measurements)
        {
            PositionSignalDataCollection.IsRefreshing = true;
            try
            {
                this.PositionSignalData.Clear();
                measurements
                .OrderBy(data => data.SignalType)
                .ThenByDescending(data => data.Strength)
                .ToList()
                .ForEach(data => PositionSignalData.Add(new CalibrationView(new Calibration(0, data))));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                PositionSignalDataCollection.IsRefreshing = false;
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
                    UseBestParameters = true,
                    LocaleId = Locale.Id
                };
                try
                {
                    var result = await positionEstimationService.EstimatePosition(command);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        RefreshMeasurements(data);
                        x.Text = result.X.ToString("0.00");
                        y.Text = result.Y.ToString("0.00");
                        PositionEstimations.Clear();
                        result.NeighbourPositions.ForEach(position => PositionEstimations.Add(new PositionEstimationView(position)));
                        UpdateChart(result);
                    });
                    // StopDataAquisition();
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await DisplayAlert(ex.Message, ex.InnerException?.Message, "OK");
                    });
                }
            }
        }

        private void UpdateChart(PositionEstimation positionEstimation)
        {
            chart.SuspendSeriesNotification();
            var secondarySeries = new List<ScatterSeries>();
            var neighboursSeries = new ScatterSeries()
            {
                ItemsSource = positionEstimation.NeighbourPositions.Select(neighbour => new { Score = neighbour.Score.ToString("0.00"), X = neighbour.Position.X, Y = neighbour.Position.Y, Name = neighbour.Position.Name }),
                ScatterHeight = 15,
                ScatterWidth = 15,
                ShapeType = ChartScatterShapeType.Triangle,
                XBindingPath = "X",
                YBindingPath = "Y",
                DataMarker = new ChartDataMarker()
                {
                    ShowLabel = true,
                    LabelStyle = new DataMarkerLabelStyle() { LabelPosition = DataMarkerLabelPosition.Inner },
                    LabelTemplate = new DataTemplate(() =>
                    {
                        var label = new Label()
                        {
                            VerticalTextAlignment = TextAlignment.Center,
                            FontSize = 13
                        };
                        label.SetBinding(Label.TextProperty, "Score");
                        return label;
                    }),
                }
            };

            var calculatedPositionSeries = new ScatterSeries()
            {
                ItemsSource = new List<object>() { new { X = positionEstimation.X, Y = positionEstimation.Y, } },
                ScatterHeight = 18,
                ScatterWidth = 18,
                ShapeType = ChartScatterShapeType.Cross,
                XBindingPath = "X",
                YBindingPath = "Y",
                DataMarker = new ChartDataMarker()
                {
                    ShowLabel = true,
                    MarkerHeight = 18,
                    MarkerWidth = 18,
                    MarkerColor = Color.Red,
                    LabelStyle = new DataMarkerLabelStyle() { LabelPosition = DataMarkerLabelPosition.Inner },
                    LabelTemplate = new DataTemplate(() =>
                    {
                        var label = new Label()
                        {
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 13,
                            Text = "VOCÊ"
                        };
                        return label;
                    }),
                }
            };

            secondarySeries.Add(neighboursSeries);
            secondarySeries.Add(calculatedPositionSeries);

            while (chart.Series.Count > 1)
            {
                chart.Series.RemoveAt(1);
            }

            chart.Series.Add(neighboursSeries);
            chart.Series.Add(calculatedPositionSeries);
            chart.ResumeSeriesNotification();
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
            catch (Exception ex)
            {
                await DisplayAlert(ex.Message, ex.InnerException?.Message, "OK");
            }
        }
    }
}