using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileTracking.Core.Application
{
    public class LocaleQuery
    {
        public float? Latitude { get; set; }

        public float? Longitude { get; set; }

        public bool? IncludeZones { get; set; }

        public bool? IncludePositions { get; set; }

        public bool? IncludePositionsData { get; set; }

        public bool? IncludePositionsCalibrations { get; set; }
    }
}
