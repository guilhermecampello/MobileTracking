using Android.Net.Wifi.Rtt;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobileTracking
{
    public interface IWifiConnector
    {
        void StartScanning(Dictionary<string, decimal> scanResults);
    }
}
