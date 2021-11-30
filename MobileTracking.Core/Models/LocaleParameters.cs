using System;
using System.Collections.Generic;
using System.Text;

namespace MobileTracking.Core.Models
{
    public class LocaleParameters
    {
        public int Id { get; set; }

        public int? LocaleId { get; set; }

        public double MeanError { get; set; }

        public int Missings { get; set; }

        public int Neighbours { get; set; }

        public double UnmatchedSignalsWeight { get; set; } = 0;

        public double BleWeight { get; set; } = 1;

        public double WifiWeight { get; set; } = 1;

        public double MagnetometerWeight { get; set; } = 1;

        public double StandardDeviationFactor { get; set; } = 0;

        public bool IsActive { get; set; }

        public bool UseDistance { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Locale? Locale { get; set; }
    }
}
