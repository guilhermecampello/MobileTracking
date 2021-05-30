using Android.Bluetooth.LE;
using MobileTracking.Core.Models;
using System;

namespace MobileTracking.Services
{
    public class SignalScanResult
    {
        public SignalScanResult(ScanResult bleScanResult)
        {
            this.Name = bleScanResult.Device!.Name ?? bleScanResult.Device.Address ?? string.Empty;
            this.Rssi = bleScanResult.Rssi;
            this.SignalType = SignalType.Bluetooth;
        }

        public SignalScanResult(string name, int rssi, SignalType signalType)
        {
            this.Name = name;
            this.Rssi = rssi;
            this.SignalType = signalType;
        }

        public SignalType SignalType { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Rssi { get; set; }

        public DateTime CreatedAt { get; } = DateTime.Now;
    }
}
