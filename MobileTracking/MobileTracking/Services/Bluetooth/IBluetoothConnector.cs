using MobileTracking.Services.Bluetooth;
using System.Collections.Generic;

namespace MobileTracking
{
    public interface IBluetoothConnector
    {
        public void StartScanning(Dictionary<string, BluetoothScanResult> devicesResults);
    }
}
