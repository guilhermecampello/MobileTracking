using System;
using System.Collections.Generic;
using System.Text;

namespace MobileTracking.Core.Queries
{
    public class GetPrecisionReportQuery
    {
        public int? LocaleId { get; set; }

        public int Neighbours { get; set; }

        public double UnmatchedSignalsWeight { get; set; } = 0;

        public double BleWeight { get; set; } = 1;

        public double WifiWeight { get; set; } = 1;

        public double MagnetometerWeight { get; set; } = 1;

        public DateTime? AfterDate { get; set; }

        public DateTime? BeforeDate { get; set; }
    }
}
