using System;

namespace MobileTracking.Core.Models
{
    public class SignalScore
    {
        public SignalScore() { }

        public SignalScore(PositionSignalData positionSignalData, Measurement measurement, double standardDeviationFactor)
        {
            PositionSignalData = positionSignalData;
            Measurement = measurement;
            CalculateScore(standardDeviationFactor);
        }

        public SignalScore(PositionSignalData positionSignalData, double weight, double standardDeviationFactor)
        {
            PositionSignalData = positionSignalData;
            CalculateAbsentSignalScore(weight, standardDeviationFactor);
        }

        public PositionSignalData PositionSignalData { get; set; }

        public Measurement? Measurement { get; set; }

        public double Score { get; set; }

        public double Distance { get; set; }

        private void CalculateScore(double standardDeviationFactor)
        {
            var samplesFactor = Math.Pow(PositionSignalData.Samples, 0.5);
            standardDeviationFactor = standardDeviationFactor != 0
                ? 1 + Math.Min(1 / PositionSignalData.StandardDeviation * samplesFactor * standardDeviationFactor, 1)
                : 1;

            if (Measurement!.SignalType == SignalType.Magnetometer)
            {
                Distance = Math.Sqrt(Math.Pow(Measurement.Y - PositionSignalData.Y, 2) + Math.Pow(Measurement.Z - PositionSignalData.Z, 2)) / standardDeviationFactor;
                Score = Math.Min(1 / Distance, 1 * standardDeviationFactor);
            }
            else
            {
                Distance = Math.Abs(PositionSignalData.Strength - Measurement.Strength) / standardDeviationFactor;
                Score = Math.Min(1 / Distance, 1 * standardDeviationFactor);
            }
        }

        private void CalculateAbsentSignalScore(double weight, double standardDeviationFactor)
        {
            var samplesFactor = Math.Pow(PositionSignalData.Samples, 0.5);
            standardDeviationFactor = standardDeviationFactor != 0
                ? 1 + Math.Min(1 / PositionSignalData.StandardDeviation * samplesFactor * standardDeviationFactor, 1)
                : 1;

            if (PositionSignalData.SignalType != SignalType.Magnetometer)
            {
                Distance = PositionSignalData.Strength - (-100) * standardDeviationFactor;
                Score = 100 / PositionSignalData.Strength * weight * standardDeviationFactor;
            }
            else
            {
                Score = 0;
            }
        }
    }
}
