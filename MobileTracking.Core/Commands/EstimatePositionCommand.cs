using System;
using MobileTracking.Core.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MobileTracking.Core.Commands
{
    public class EstimatePositionCommand
    {
        public int LocaleId { get; set; }

        public float? RealX { get; set; }

        public float? RealY { get; set; }

        public int Neighbours { get; set; }

        public double? UnmatchedSignalsWeight { get; set; }

        [MinLength(1)]
        public List<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
}
