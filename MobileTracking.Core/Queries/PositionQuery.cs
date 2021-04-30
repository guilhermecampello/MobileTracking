using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Application
{
    public class PositionQuery
    {
        public int? LocaleId { get; set; }

        public int? ZoneId { get; set; }

        public bool IncludeData { get; set; }

        public bool IncludeCalibrations { get; set; }
    }
}
