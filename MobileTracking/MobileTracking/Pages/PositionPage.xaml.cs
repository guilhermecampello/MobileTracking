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

        private Thread bluetoothThread;

        private Thread wifiThread;

        private Timer? timer;

        private string position = string.Empty;

        private int count = 0;

        public PositionPage(Position position)
        {
            InitializeComponent();
            this.Position = position;
            var serviceProvider = Startup.ServiceProvider;
            this.wifiConnector = serviceProvider.GetService<IWifiConnector>();
            this.bluetoothConnector = serviceProvider.GetService<IBluetoothConnector>();
            this.magneticFieldSensor = serviceProvider.GetService<MagneticFieldSensor>();

            bluetoothThread = new Thread(StartBluetoothScan);
            wifiThread = new Thread(StartWifiScan);
            magneticFieldSensor.Start();
            BindingContext = this;
        }

        public void StartBluetoothScan()
        {
            bluetoothConnector.StartScanning();
        }

        public void StartWifiScan()
        {
            wifiConnector.StartScanning();
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
    }
}