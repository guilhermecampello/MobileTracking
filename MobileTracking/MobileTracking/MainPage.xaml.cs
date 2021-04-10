using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Android.Net.Wifi;
using Xamarin.Essentials;
using MobileTracking.Services.Bluetooth;
using System.Threading;
using Android.App;
using MobileTracking.Services.MagneticField;
using System.Net;

namespace MobileTracking
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        Canvas Canvas = new Canvas();

        public List<Marker> Markers { get; set; } = new List<Marker>();

        public IWifiConnector wifiConnector = DependencyService.Get<IWifiConnector>();

        private Dictionary<string, decimal> wifiResults = new Dictionary<string, decimal>();

        private Thread bluetoothThread;

        private Thread wifiThread;

        private Timer timer;

        private MagneticFieldSensor magneticFieldSensor = new MagneticFieldSensor();

        public IBluetoothConnector bluetoothConnector = DependencyService.Get<IBluetoothConnector>();

        private Dictionary<string, BluetoothScanResult> bluetoothResults = new Dictionary<string, BluetoothScanResult>();

        private string position = string.Empty;

        private int count = 0;

        HttpClient Client = new HttpClient(new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
            {
                return true;
            },
        });



        public MainPage()
        {
            InitializeComponent();
            this.CanvasView.Content = this.Canvas;
            bluetoothThread = new Thread(StartBluetoothScan);
            wifiThread = new Thread(StartWifiScan);
            magneticFieldSensor.Start();
            Client.Timeout = TimeSpan.FromSeconds(3);
        }

        public void StartBluetoothScan()
        {
            bluetoothConnector.StartScanning(bluetoothResults);
        }

        public void StartWifiScan()
        {
            wifiConnector.StartScanning(wifiResults);
        }

        private async Task SendPositionData()
        {
            var wifiRssi = wifiResults.GetValueOrDefault("Campello ");
            var bluetoothRssi = 0;
            if (bluetoothResults.ContainsKey("MLT-BT05"))
            {
                bluetoothRssi = bluetoothResults.GetValueOrDefault("MLT-BT05").Rssi;
            }
            var magneticField = magneticFieldSensor.MagneticFieldVector;
            var content = new StringContent($"{position}," +
                $"{wifiRssi}," +
                $"{bluetoothRssi}," +
                $"{magneticField.X.ToString().Replace(",",".")}," +
                $"{magneticField.Y.ToString().Replace(",",".")}," +
                $"{magneticField.Z.ToString().Replace(",",".")}",
                Encoding.ASCII);
            try
            {
                foreach (IPAddress adress in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    Console.WriteLine("IP Adress: " + adress.ToString());
                }
                var response = await Client.PostAsync("http://192.168.15.7:5000/experiment", content);
                Console.WriteLine(response.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            count++;
            Device.BeginInvokeOnMainThread(() => UpdateCounter());
        }

        private void UpdateSignalStrengths(object state)
        {
            try
            {
                Device.BeginInvokeOnMainThread(() => UpdateMagneticField());
                if (wifiResults.ContainsKey("Campello "))
                {
                    Device.BeginInvokeOnMainThread(() => UpdateWifi());
                }
                var expiredResults = this.bluetoothResults.Values
                    .Where(bluetoothResult => DateTime.Now.Subtract(bluetoothResult.CreatedAt).TotalSeconds > 5)
                    .ToList();
                expiredResults.ForEach(expiredResult => this.bluetoothResults.Remove(expiredResult.Name));
                if (bluetoothResults.ContainsKey("MLT-BT05"))
                {
                    Device.BeginInvokeOnMainThread(() => UpdateBluetooth());
                }

                SendPositionData().Wait();

                if (count >= 10)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        count = 0;
                        UpdateCounter();
                    }
                    );
                    timer.Dispose();
                    timer = null;
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Connection Error", ex.Message, "OK");
            }
        }

        public async void calibrate_btn_clicked(object sender, System.EventArgs e)
        {
            try
            {
                position = await DisplayPromptAsync("Insira a posição", "");
                var permission = await Permissions.RequestAsync<Permissions.LocationAlways>();
                if (permission == PermissionStatus.Granted)
                {
                    if (!wifiThread.IsAlive)
                    {
                        wifiThread.Start();
                    }
                    if (!bluetoothThread.IsAlive)
                    {
                        bluetoothThread.Start();
                    }
                    count = 0;
                    Device.BeginInvokeOnMainThread(() => UpdateCounter());
                    if (timer != null)
                    {
                        timer.Dispose();
                        timer = null;
                    }
                    timer = new Timer(UpdateSignalStrengths);
                    timer.Change(1000, 3000);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        public void UpdateWifi()
        {
            var value = wifiResults.GetValueOrDefault("Campello ");
            wifi.Text = $"WIFI: {value}";
        }

        public void UpdateBluetooth()
        {
            var value = bluetoothResults.GetValueOrDefault("MLT-BT05").Rssi;
            bluetooth.Text = $"Bluetooth: {value}";
        }

        public void UpdateCounter()
        {
            counter.Text = $"{count}/10";
        }

        public void UpdateMagneticField()
        {
            var magneticField = magneticFieldSensor.MagneticFieldVector;
            magX.Text = $"X: {magneticField.X:0.00}";
            magY.Text = $"Y: {magneticField.Y:0.00}";
            magZ.Text = $"Z: {magneticField.Z:0.00}";
        }

        public async void btn_clicked(object sender, System.EventArgs e)
        {

        }

        public async void add_marker_clicked(object sender, EventArgs e)
        {
            await AddMarker();
        }

        private async Task AddMarker()
        {
            var name = await DisplayPromptAsync("New marker", "Input marker name:");
            if (!string.IsNullOrEmpty(name))
            {
            }
        }
    }
}
