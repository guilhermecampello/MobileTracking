using MobileTracking.Services;
using System.Collections.Generic;

namespace MobileTracking
{
    public interface IBluetoothConnector
    {
        public MonitoringState State { get; }

        Dictionary<string, SignalScanResult> DevicesResults { get; }

        public void StartScanning();
    }
}
