using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Runtime;
using MobileTracking.Services;
using MobileTracking.Services.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using ScanResult = Android.Bluetooth.LE.ScanResult;

[assembly: Dependency(typeof(MobileTracking.Droid.Services.BluetoothConnector))]
namespace MobileTracking.Droid.Services
{
    class BluetoothConnector : IBluetoothConnector
    {
        private Context context = null;

        private Dictionary<string, BluetoothScanResult> devicesResults { get; set; } = new Dictionary<string, BluetoothScanResult>();

        private BluetoothManager bluetoothManager;

        private BluetoothAdapter bluetoothAdapter;

        private BluetoothLeScanner bluetoothScanner;

        public BluetoothConnector()
        {
            this.context = Android.App.Application.Context;
            bluetoothManager = (BluetoothManager)context.GetSystemService(Context.BluetoothService);
            bluetoothAdapter = bluetoothManager.Adapter;
            bluetoothScanner = bluetoothManager.Adapter.BluetoothLeScanner;
        }

        public Dictionary<string, BluetoothScanResult> DevicesResults { get => devicesResults; }

        public MonitoringState State
        {
            get
            {
                if (bluetoothAdapter != null && bluetoothAdapter.IsDiscovering)
                {
                    return MonitoringState.Monitoring;
                }

                if (bluetoothAdapter != null && bluetoothAdapter.IsEnabled)
                {
                    return MonitoringState.Available;
                }

                return MonitoringState.Unavailable;
            }
        }

        public void StartScanning()
        {
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
                    Task.Delay(3000).Wait();
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