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

        public double BleWeight { get; set; } = 1;

        public double WifiWeight { get; set; } = 1;

        public double MagnetometerWeight { get; set; } = 1;

        public bool UseBestParameters { get; set; }

        public bool UseDistance { get; set; }

        public double StandardDeviationFactor { get; set; } = 0;

        [MinLength(1)]
        public List<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
}
