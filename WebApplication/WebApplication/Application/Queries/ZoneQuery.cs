using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Application
{
    public class ZoneQuery
    {
        public int? LocaleId { get; set; }

        public int? Floor { get; set; }

        public bool IncludePositions { get; set; }

        public bool IncludePositionsData { get; set; }

        public bool IncludePositionsCalibrations { get; set; }
    }
}
