using System;
using System.Collections.Generic;
using System.Text;

namespace MobileTracking.Core.Queries
{
    public class GetPrecisionReportQuery
    {
        public int? LocaleId { get; set; }

        public int Neighbours { get; set; }

        public bool IgnoreNotDetectedSignals { get; set; }

        public DateTime? AfterDate { get; set; }

        public DateTime? BeforeDate { get; set; }
    }
}
