using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Net.Wifi;
using Android.Net.Wifi.Rtt;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileTracking.Services.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ScanResult = Android.Bluetooth.LE.ScanResult;

[assembly: Dependency(typeof(MobileTracking.Droid.Services.BluetoothConnector))]
namespace MobileTracking.Droid.Services
{
    class BluetoothConnector : IBluetoothConnector
    {
        private Context context = null;

        private Dictionary<string, BluetoothScanResult> devicesResults;

        private BluetoothManager bluetoothManager;

        private BluetoothAdapter bluetoothAdapter;

        private BluetoothLeScanner bluetoothScanner;

        public BluetoothConnector()
        {
            this.context = Android.App.Application.Context;
        }

        public void StartScanning(Dictionary<string, BluetoothScanResult> devicesResults)
        {
            bluetoothManager = (BluetoothManager)context.GetSystemService(Context.BluetoothService);
            bluetoothAdapter = bluetoothManager.Adapter;
            bluetoothScanner = bluetoothManager.Adapter.BluetoothLeScanner;
            this.devicesResults = devicesResults;
            if (!bluetoothAdapter.IsEnabled)
            {
                throw new Exception("Please enable Bluetooth");
            }
            var scanCallback = new BluetoothScanCallback(devicesResults);
            while (true)
            {
                try
                {
                    bluetoothScanner.StartScan(scanCallback);
                    Task.Delay(5000).Wait();
                    bluetoothScanner.StopScan(scanCallback);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public class BluetoothScanCallback : ScanCallback
        {
            private Dictionary<string, BluetoothScanResult> devicesResults;

            public BluetoothScanCallback(Dictionary<string, BluetoothScanResult> devicesResults)
            {
                this.devicesResults = devicesResults;
            }

            public override void OnBatchScanResults(IList<ScanResult> results)
            {
                results.AsEnumerable().ToList().ForEach(result =>
                {
                    AddScanResult(result);
                });
            }

            public override void OnScanResult([GeneratedEnum] ScanCallbackType callbackType, ScanResult result)
            {
                AddScanResult(result);
            }

            private void AddScanResult(ScanResult result)
            {
                var bluetoothScanResult = new BluetoothScanResult(result);
                if (this.devicesResults.ContainsKey(bluetoothScanResult.Name))
                {
                    this.devicesResults.Remove(bluetoothScanResult.Name);
                }
                this.devicesResults.Add(bluetoothScanResult.Name, bluetoothScanResult);
            }
        }
    }
}