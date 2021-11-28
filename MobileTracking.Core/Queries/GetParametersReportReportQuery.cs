using System;
using System.Collections.Generic;
using System.Text;

namespace MobileTracking.Core.Queries
{
    public class GetParametersReportQuery
    {
        public int? LocaleId { get; set; }

        public int MaxNeighbours { get; set; } = 5;

        public int MinNeighbours { get; set; } = 2;

        public int NeighboursStep { get; set; } = 1;

        public double MinUnmatchedSignalsWeight { get; set; } = 0;

        public double MaxUnmatchedSignalsWeight { get; set; } = 20;

        public double UnmatchedSignalsWeightStep { get; set; } = 2;

        public double MinBleWeight { get; set; } = 1;

        public double MaxBleWeight { get; set; } = 5;

        public double BleWeightStep { get; set; } = 1;

        public double MinWifiWeight { get; set; } = 1;

        public double MaxWifiWeight { get; set; } = 5;

        public double WifiWeightStep { get; set; } = 1;

        public double MinMagnetometerWeight { get; set; } = 1;

        public double MaxMagnetometerWeight { get; set; } = 5;

        public double MagnetometerWeightStep { get; set; } = 1;

        public DateTime? AfterDate { get; set; }

        public DateTime? BeforeDate { get; set; }
    }
}
