using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Runtime;
using MobileTracking.Core.Models;
using MobileTracking.Services;
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

        private Dictionary<string, SignalScanResult> devicesResults { get; set; } = new Dictionary<string, SignalScanResult>();

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
            context.RegisterReceiver(bluetoothReceiver, new IntentFilter(BluetoothDevice.ExtraRssi));
            this.scanThread = new Thread(Scan);
            this.scanCallback = new BluetoothScanCallback(devicesResults, macAddressMapping);
        }

        public Dictionary<string, SignalScanResult> DevicesResults
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
                bluetoothAdapter.Enable();
            }
            while (true)
            {
                try
                {
                    bluetoothAdapter.StartDiscovery();
                    bluetoothScanner.StartScan(scanCallback);
                    Task.Delay(10000).Wait();
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
            private Dictionary<string, SignalScanResult> devicesResults;

            private Dictionary<string, string> macAddressMapping { get; set; }

            public BluetoothReceiver(
                Dictionary<string, SignalScanResult> devicesResults,
                Dictionary<string, string> macAddressMapping)
            {
                this.devicesResults = devicesResults;
                this.macAddressMapping = macAddressMapping;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                try
                {
                    var action = intent.Action;            
                    if (action == BluetoothDevice.ActionFound || action == BluetoothDevice.ExtraRssi)
                    {
                        var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                        var rssi = intent.GetShortExtra(BluetoothDevice.ExtraRssi, 0);
                        if (device != null)
                        {
                            lock (macAddressMapping)
                            {
                                macAddressMapping[device.Address] = device.Name;
                            }
                        }
                        if (rssi !=0)
                        {
                            var name = $"{device.Name}({device.Address})";
                            AddScanResult(new SignalScanResult(name, rssi, SignalType.Bluetooth));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            private void AddScanResult(SignalScanResult bluetoothScanResult)
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
            private Dictionary<string, SignalScanResult> devicesResults;

            private Dictionary<string, string> macAddressMapping { get; set; }

            public BluetoothScanCallback(Dictionary<string, SignalScanResult> devicesResults, Dictionary<string, string> macAddressMapping)
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
                Console.WriteLine("TxPower" + result.TxPower);
                Console.WriteLine(result.ScanRecord.ManufacturerSpecificData);
                Console.WriteLine("ServiceData:"+ result.ScanRecord.ServiceData.Count);
                if (result.ScanRecord.ServiceSolicitationUuids != null)
                {
                    foreach (var uuid in result.ScanRecord.ServiceSolicitationUuids)
                    {
                        Console.WriteLine(uuid.Uuid + ";");
                    }
                }

                if (result.ScanRecord.ServiceUuids != null)
                {
                    foreach (var uuid in result.ScanRecord.ServiceUuids)
                    {
                        Console.WriteLine(uuid.Uuid + ";");
                    }
                }

                var bluetoothScanResult = new SignalScanResult(result);                
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
    }
}