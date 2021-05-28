using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileTracking.Core.Models;

namespace MobileTracking.Core.Application
{
    public class CalibrationsQuery
    {
        public int? LocaleId { get; set; }

        public int? ZoneId { get; set; }

        public int? PositionId { get; set; }

        public SignalType? SignalType { get; set; }

        public string? SignalId { get; set; }

        public int[]? CalibrationIds { get; set; }
    }
}
