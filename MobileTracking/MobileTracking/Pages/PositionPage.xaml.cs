using MobileTracking.Core;
using MobileTracking.Core.Models;
using MobileTracking.Services;
using MobileTracking.Services.Bluetooth;
using MobileTracking.Services.MagneticField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        private Timer? statetimer;

        private int count = 0;

        public PositionPage(Position position)
        {
            InitializeComponent();
            this.Position = position;
            var serviceProvider = Startup.ServiceProvider;
            this.wifiConnector = serviceProvider.GetService<IWifiConnector>();
            this.bluetoothConnector = serviceProvider.GetService<IBluetoothConnector>();
            this.magneticFieldSensor = serviceProvider.GetService<MagneticFieldSensor>();

            BindingContext = this;
            statetimer = new Timer(UpdateMonitoringState);
            statetimer.Change(0, 1000);
        }

        public Position Position { get; set; }

        

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

        public void UpdateMonitoringState(object state)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                wifiColor.BackgroundColor = WifiStateColor;
                wifiState.Text = WifiState;
                bluetoothColor.BackgroundColor = BluetoothStateColor;
                bluetoothState.Text = BluetoothState;
                magnetometerColor.BackgroundColor = MagnetometerStateColor;
                magnetometerState.Text = MagnetometerState;
                var magneticVector = magneticFieldSensor.CalculateMagneticFieldVector();
                X.Text = "X: "+ magneticVector.X.ToString("F");
                Y.Text = "Y: " + magneticVector.Y.ToString("F");
                Z.Text = "Z: " + magneticVector.Z.ToString("F");
            });
            FetchCalibrationData();
        }

        private List<Calibration> FetchCalibrationData()
        {
            var data = new List<Calibration>();
            this.wifiConnector.ScanResults.ToList().ForEach(device =>
            {
                var measurement = MeasurementsFactory.CreateWifiMeasurement(device.Key, (int)Math.Round(device.Value));
                data.Add(new Calibration(Position.Id, measurement));
            });

            this.bluetoothConnector.DevicesResults.ToList().ForEach(device =>
            {
                var measurement = MeasurementsFactory.CreateBluetoothMeasurement(device.Key, device.Value.Rssi);
                data.Add(new Calibration(Position.Id, measurement));
            });

            data.Add(new Calibration(
                Position.Id,
                MeasurementsFactory.CreateMagnetometerMeasurement(magneticFieldSensor.CalculateMagneticFieldVector())
            ));

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
            magneticFieldSensor.Start();
            bluetoothConnector.StartScanning();
            wifiConnector.StartScanning();
        }
    }
}