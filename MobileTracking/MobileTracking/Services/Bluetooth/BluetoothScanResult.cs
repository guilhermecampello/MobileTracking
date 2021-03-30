using Android.Bluetooth.LE;
using System;

namespace MobileTracking.Services.Bluetooth
{
    public class BluetoothScanResult
    {
        public BluetoothScanResult(ScanResult scanResult)
        {
            this.Name = scanResult.Device.Name ?? scanResult.Device.Address;
            this.Rssi = scanResult.Rssi;
        }

        public string Name { get; set; }

        public int Rssi { get; set; }

        public DateTime CreatedAt { get; } = DateTime.Now;
    }
}
