using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Application
{
    public class CalibrationsQuery
    {
        public int? LocaleId { get; set; }

        public int? ZoneId { get; set; }

        public int? PositionId { get; set; }

        public SignalType? SignalType { get; set; }

        public string? SignalId { get; set; } = string.Empty;
    }
}
