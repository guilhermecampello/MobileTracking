using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileTracking.Core.Application
{
    public class PositionDataQuery
    {
        public int? LocaleId { get; set; }

        public int? ZoneId { get; set; }

        public int? PositionId { get; set; }

        public bool? NeedsUpdate { get; set; }
    }
}
