using System;
using System.Collections.Generic;
using System.Text;

namespace MobileTracking.Core.Models
{
    public class PrecisionCalculation
    {
        public PrecisionCalculation() { }

        public PrecisionCalculation(UserLocalization userLocalization, PositionEstimation positionEstimation)
        {
            LocaleId = userLocalization.LocaleId;
            CalculatedX = positionEstimation.X;
            CalculatedY = positionEstimation.Y;
            RealX = userLocalization.RealX ?? 0;
            RealY = userLocalization.RealY ?? 0;
            Error = Math.Sqrt(Math.Pow((double)(positionEstimation.X! - userLocalization.RealX!), 2) +
                    Math.Pow((double)(positionEstimation.Y! - userLocalization.RealY!), 2));
            LocalizationDate = userLocalization.DateTime;
        }

        public int LocaleId { get; set; }

        public double CalculatedX { get; set; }

        public double CalculatedY { get; set; }

        public double Error { get; set; }

        public float RealX { get; set; }

        public float RealY { get; set; }

        public DateTime LocalizationDate { get; set; }

    }
}
