using Android.Net.Wifi.Rtt;
using MobileTracking.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobileTracking
{
    public interface IWifiConnector
    {
        public MonitoringState State { get; }

        Dictionary<string, SignalScanResult> ScanResults { get; }
        
        void StartScanning();
    }
}
