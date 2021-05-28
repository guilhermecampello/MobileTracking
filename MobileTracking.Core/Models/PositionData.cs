using System;
using System.Collections.Generic;
using System.Linq;

namespace MobileTracking.Core.Models
{
    public class PositionData : Measurement
    {
        public int Id { get; set; }

        public int PositionId { get; set; }

        public int Samples { get; set; }

        public float StandardDeviation { get; set; }

        public float Max { get; set; }

        public float Min { get; set; }

        public float StandardDeviationX { get; set; }

        public float MaxX { get; set; }

        public float MinX { get; set; }

        public float StandardDeviationY { get; set; }

        public float MaxY { get; set; }

        public float MinY { get; set; }

        public float StandardDeviationZ { get; set; }

        public float MaxZ { get; set; }

        public float MinZ { get; set; }

        public string Name
        {
            get => this.Position?.Name != null && this.Position.Zone != null
                ? $"{this.Position.Zone.Name}-{this.Position.Name}"
                : this.PositionId.ToString();
        }

        public DateTime LastUpdate { get; set; }

        public Position? Position { get; set; }

        public void CalculateStandardDeviation(List<Calibration> calibrations)
        {
            var quadraticDiffSum = calibrations
                        .Sum(calibration => Math.Pow(calibration.Strength - Strength, 2));

            var diffDivision = quadraticDiffSum / calibrations.Count();
            StandardDeviation = (float)Math.Sqrt((double)diffDivision);

            if (SignalType == SignalType.Magnetometer)
            {
                var quadraticDiffSumX = calibrations
                    .Sum(calibration => Math.Pow(calibration.X - X, 2));

                var diffDividedX = quadraticDiffSumX / calibrations.Count();
                StandardDeviationX = (float)Math.Sqrt((double)diffDividedX);

                var quadraticDiffSumY = calibrations
                    .Sum(calibration => Math.Pow(calibration.Y - Y, 2));

                var diffDividedY = quadraticDiffSumY / calibrations.Count();
                StandardDeviationY = (float)Math.Sqrt((double)diffDividedY);

                var quadraticDiffSumZ = calibrations
                    .Sum(calibration => Math.Pow(calibration.Z - Z, 2));

                var diffDividedZ = quadraticDiffSumZ / calibrations.Count();
                StandardDeviationZ = (float)Math.Sqrt((double)diffDividedZ);
            }
        }
    }
}