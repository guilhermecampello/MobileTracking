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
        private readonly IWifiConnector wifiConnector;

        private readonly MagneticFieldSensor magneticFieldSensor;
        
        private readonly IBluetoothConnector bluetoothConnector;
        
        private Dictionary<string, decimal> wifiResults = new Dictionary<string, decimal>();

        private Dictionary<string, BluetoothScanResult> bluetoothResults = new Dictionary<string, BluetoothScanResult>();
        
        private Thread bluetoothThread;

        private Thread wifiThread;

        private Timer? timer;

        private string position = string.Empty;

        private int count = 0;


        public MainPage(
            IWifiConnector wifiConnector,
            MagneticFieldSensor magneticFieldSensor,
            IBluetoothConnector bluetoothConnector)
        {
            InitializeComponent();
            this.wifiConnector = wifiConnector;
            this.magneticFieldSensor = magneticFieldSensor;
            this.bluetoothConnector = bluetoothConnector;
            bluetoothThread = new Thread(StartBluetoothScan);
            wifiThread = new Thread(StartWifiScan);
            magneticFieldSensor.Start();
        }

        public void StartBluetoothScan()
        {
            bluetoothConnector.StartScanning(bluetoothResults);
        }

        public void StartWifiScan()
        {
            wifiConnector.StartScanning(wifiResults);
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
            }
            catch (Exception)
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
            //wifi.Text = $"WIFI: {value}";
        }

        public void UpdateBluetooth()
        {
            var value = bluetoothResults.GetValueOrDefault("MLT-BT05").Rssi;
            //bluetooth.Text = $"Bluetooth: {value}";
        }

        public void UpdateCounter()
        {
            counter.Text = $"{count}/10";
        }

        public void UpdateMagneticField()
        {
        }
    }
}
