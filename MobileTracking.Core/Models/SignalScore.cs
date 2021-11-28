using System;

namespace MobileTracking.Core.Models
{
    public class SignalScore
    {
        public SignalScore() { }

        public SignalScore(PositionSignalData positionSignalData, Measurement measurement)
        {
            PositionSignalData = positionSignalData;
            Measurement = measurement;
            CalculateScore();
        }

        public SignalScore(PositionSignalData positionSignalData, double weight)
        {
            PositionSignalData = positionSignalData;
            CalculateAbsentSignalScore(weight);
        }

        public PositionSignalData PositionSignalData { get; set; }

        public Measurement? Measurement { get; set; }

        public double Score { get; set; }

        private void CalculateScore()
        {
            if (Measurement!.SignalType == SignalType.Magnetometer)
            {
                var distance = Math.Sqrt(Math.Pow(Measurement.Y - PositionSignalData.Y, 2) + Math.Pow(Measurement.Z - PositionSignalData.Z, 2));
                Score = Math.Min(1 / distance, 1);
            }
            else
            {
                Score = Math.Min(1 / Math.Abs(PositionSignalData.Strength - Measurement.Strength), 1);
            }
        }

        private void CalculateAbsentSignalScore(double weight)
        {
            if (PositionSignalData.SignalType != SignalType.Magnetometer)
            {
                Score = 100 / PositionSignalData.Strength * weight;
            }
            else
            {
                Score = 0;
            }
        }
    }
}
