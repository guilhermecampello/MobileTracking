using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Runtime;
using MobileTracking.Services;
using MobileTracking.Services.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using ScanResult = Android.Bluetooth.LE.ScanResult;

[assembly: Dependency(typeof(MobileTracking.Droid.Services.BluetoothConnector))]
namespace MobileTracking.Droid.Services
{
    class BluetoothConnector : IBluetoothConnector
    {
        private Context context = null;

        private Dictionary<string, string> macAddressMapping { get; set; } = new Dictionary<string, string>();

        private Dictionary<string, BluetoothScanResult> devicesResults { get; set; } = new Dictionary<string, BluetoothScanResult>();

        private BluetoothManager bluetoothManager;

        private BluetoothAdapter bluetoothAdapter;

        private BluetoothLeScanner bluetoothScanner;

        private BluetoothReceiver bluetoothReceiver;

        private Thread scanThread;

        private ScanCallback scanCallback;

        public BluetoothConnector()
        {
            this.context = Android.App.Application.Context;
            bluetoothManager = (BluetoothManager)context.GetSystemService(Context.BluetoothService);
            bluetoothAdapter = bluetoothManager.Adapter;
            bluetoothScanner = bluetoothManager.Adapter.BluetoothLeScanner;
            bluetoothReceiver = new BluetoothReceiver(devicesResults, macAddressMapping);
            context.RegisterReceiver(bluetoothReceiver, new IntentFilter(BluetoothDevice.ActionFound));
            this.scanThread = new Thread(Scan);
            this.scanCallback = new BluetoothScanCallback(devicesResults, macAddressMapping);
        }

        public Dictionary<string, BluetoothScanResult> DevicesResults
        {
            get => devicesResults
                .Where(result => DateTime.Now.Subtract(result.Value.CreatedAt).TotalSeconds < 10)
                .ToDictionary(result => result.Key, result => result.Value);
        }

        public MonitoringState State
        {
            get
            {
                if (bluetoothAdapter != null && bluetoothAdapter.IsEnabled && scanThread.IsAlive)
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
            if (!scanThread.IsAlive)
            {
                this.scanThread.Start();
            }
        }

        public void Scan()
        {
            if (!bluetoothAdapter.IsEnabled)
            {
                throw new Exception("Please enable Bluetooth");
            }
            while (true)
            {
                try
                {
                    bluetoothAdapter.StartDiscovery();
                    bluetoothScanner.StartScan(scanCallback);
                    Task.Delay(3000).Wait();
                    bluetoothScanner.StopScan(scanCallback);
                    bluetoothAdapter.CancelDiscovery();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public class BluetoothReceiver : BroadcastReceiver
        {
            private Dictionary<string, BluetoothScanResult> devicesResults;

            private Dictionary<string, string> macAddressMapping { get; set; }

            public BluetoothReceiver(
                Dictionary<string, BluetoothScanResult> devicesResults,
                Dictionary<string, string> macAddressMapping)
            {
                this.devicesResults = devicesResults;
                this.macAddressMapping = macAddressMapping;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                String action = intent.Action;            
                if (action == BluetoothDevice.ActionFound)
                {
                    var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    var rssi = intent.GetShortExtra(BluetoothDevice.ExtraRssi, 0);
                    lock (macAddressMapping)
                    {
                        macAddressMapping[device.Address] = device.Name;
                    }

                    AddScanResult(new BluetoothScanResult(device.Name, rssi));
                }
            }

            private void AddScanResult(BluetoothScanResult bluetoothScanResult)
            {
                lock (devicesResults)
                {
                    if (this.devicesResults.ContainsKey(bluetoothScanResult.Name))
                    {
                        this.devicesResults.Remove(bluetoothScanResult.Name);
                    }
                    this.devicesResults.Add(bluetoothScanResult.Name, bluetoothScanResult);
                }
            }
        }

        public class BluetoothScanCallback : ScanCallback
        {
            private Dictionary<string, BluetoothScanResult> devicesResults;

            private Dictionary<string, string> macAddressMapping { get; set; }

            public BluetoothScanCallback(Dictionary<string, BluetoothScanResult> devicesResults, Dictionary<string, string> macAddressMapping)
            {
                this.devicesResults = devicesResults;
                this.macAddressMapping = macAddressMapping;
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
                macAddressMapping.TryGetValue(result.Device.Address, out string name);
                if (string.IsNullOrEmpty(name))
                {
                    name = bluetoothScanResult.Name;
                }
                lock (devicesResults)
                {
                    if (this.devicesResults.ContainsKey(name))
                    {
                        this.devicesResults.Remove(name);
                    }
                    this.devicesResults.Add(name, bluetoothScanResult);
                }
            }
        }
    }
}