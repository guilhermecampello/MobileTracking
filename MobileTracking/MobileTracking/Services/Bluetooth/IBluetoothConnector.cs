using MobileTracking.Services;
using MobileTracking.Services.Bluetooth;
using System.Collections.Generic;

namespace MobileTracking
{
    public interface IBluetoothConnector
    {
        public MonitoringState State { get; }

        Dictionary<string, BluetoothScanResult> DevicesResults { get; }

        public void StartScanning();
    }
}
