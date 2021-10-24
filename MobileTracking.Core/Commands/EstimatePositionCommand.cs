using System;
using MobileTracking.Core.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MobileTracking.Core.Commands
{
    public class EstimatePositionCommand
    {
        public int LocaleId { get; set; }

        public int N { get; set; }

        [MinLength(1)]
        public List<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
}
