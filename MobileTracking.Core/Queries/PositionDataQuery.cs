using MobileTracking.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileTracking.Core.Application
{
    public class PositionSignalDataQuery
    {
        public int? LocaleId { get; set; }

        public int? ZoneId { get; set; }

        public int? PositionId { get; set; }

        public string? SignalId { get; set; }

        public SignalType? SignalType { get; set; }

        public bool? NeedsUpdate { get; set; }

        public bool? IncludePosition { get; set; }

        public bool? IncludeZone { get; set; }
    }
}
