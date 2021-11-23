using System;
using System.Collections.Generic;

namespace MobileTracking.Core.Models
{
    public class UserLocalization
    {
        public int Id { get; set; }

        public int LocaleId { get; set; }

        public int UserId { get; set; }

        public float? RealX { get; set; }

        public float? RealY { get; set; }

        public DateTime DateTime { get; set; }

        public List<LocalizationMeasurement>? LocalizationMeasurements { get; set; }
    }
}
