using System;
using System.Collections.Generic;
using System.Text;

namespace MobileTracking.Core.Models
{
    public class SignalScore
    {
        public SignalScore(PositionSignalData positionSignalData, Measurement measurement)
        {
            this.PositionSignalData = positionSignalData;
            this.Measurement = measurement;
            CalculateScore();
        }
        public PositionSignalData PositionSignalData { get; set; }

        public Measurement Measurement { get; set; }

        public double Score { get; set; }

        private void CalculateScore()
        {
            if (Measurement.SignalType == SignalType.Magnetometer)
            {
                var distance = Math.Sqrt(Math.Pow(Measurement.Y - PositionSignalData.Y, 2) + Math.Pow(Measurement.Z - PositionSignalData.Z, 2));
                Score = (1 - distance) * 3;
                if (Score < 0.5)
                {
                    Score = 0.5;
                }
            }
            else
            {
                var proximityFactor = Measurement.Strength/PositionSignalData.Strength;
                Score = (1 - Math.Abs(1 - proximityFactor)) * 3;
            }
        }
    }
}
