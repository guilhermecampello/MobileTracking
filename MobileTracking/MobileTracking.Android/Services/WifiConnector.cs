using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.Net.Wifi.Rtt;
using Android.Runtime;
using MobileTracking.Core.Models;
using MobileTracking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(MobileTracking.Droid.Services.WifiConnector))]
namespace MobileTracking.Droid.Services
{
    class WifiConnector : IWifiConnector
    {
        private Context context = null;

        private Dictionary<string, SignalScanResult> scanResults { get; set; } = new Dictionary<string, SignalScanResult>();

        private WifiManager wifiManager;

        private WifiReceiver wifiReceiver;

        private Thread scanThread;

        public WifiConnector()
        {
            this.context = Android.App.Application.Context;
            wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
            wifiReceiver = new WifiReceiver(wifiManager, scanResults);
            context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            this.scanThread = new Thread(Scan);
        }

        public MonitoringState State
        {
            get
            {
                if (!wifiManager.IsScanAlwaysAvailable)
                {
                    return MonitoringState.Unavailable;
                }

                if (wifiManager.IsWifiEnabled && !scanThread.IsAlive)
                {
                    return MonitoringState.Available;
                }

                return MonitoringState.Monitoring;
            }
        }

        public Dictionary<string, SignalScanResult> ScanResults =>
            scanResults
            .Where(result => DateTime.Now.Subtract(result.Value.CreatedAt).TotalSeconds <= 8)
            .ToDictionary(result => result.Key, result => result.Value);

        public void StartScanning()
        {
            if (!this.scanThread.IsAlive)
            {
                this.scanThread.Start();
            }
        }

        public void Scan()
        {
            while (true)
            {
                try
                {
                    if (!wifiManager.IsWifiEnabled)
                    {
                        wifiManager.SetWifiEnabled(true);
                    }

                    if (wifiManager.WifiState != Android.Net.WifiState.Enabled)
                    {
                        throw new Exception("Please enable wi-fi.");
                    }

                    if (wifiManager.StartScan())
                    {
                        Console.WriteLine("Started scanning");
                    }
                }
                catch(Exception e)
                {
                    // Console.WriteLine(e.Message);
                }
                finally
                {
                    Task.Delay(500).Wait();
                }
            }
        }
    }

    public class WifiReceiver : BroadcastReceiver
    {
        private WifiManager wifiManager;

        private IDictionary<string, SignalScanResult> rangingResults;

        public WifiReceiver(WifiManager wifiManager, IDictionary<string, SignalScanResult> rangingResults)
        {
            this.wifiManager = wifiManager;
            this.rangingResults = rangingResults;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            IList<ScanResult> scanResults = wifiManager.ScanResults;

            foreach (var scanResult in scanResults)
            {
                if (this.rangingResults.ContainsKey(scanResult.Ssid))
                {
                    this.rangingResults.Remove(scanResult.Ssid);
                }
                this.rangingResults.Add(scanResult.Ssid, new SignalScanResult(scanResult.Ssid, scanResult.Level, SignalType.Wifi));
            }
        }
    }

    public class RangingCallback : RangingResultCallback
    {
        private IDictionary<string, SignalScanResult> rangingResults;

        private string ssid;

        public RangingCallback(string ssid, IDictionary<string, SignalScanResult> rangingResults)
        {
            this.rangingResults = rangingResults;
            this.ssid = ssid;
        }

        public override void OnRangingFailure([GeneratedEnum] RangingResultStatusCode code)
        {
            Console.WriteLine(code);
        }

        public override void OnRangingResults(IList<RangingResult> results)
        {
            foreach (var result in results)
            {
                if (this.rangingResults.ContainsKey(this.ssid))
                {
                    this.rangingResults.Remove(this.ssid);
                }
                this.rangingResults.Add(this.ssid, new SignalScanResult(this.ssid, result.DistanceMm, Core.Models.SignalType.Wifi));
            }
        }
    }
}