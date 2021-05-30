using System;
using System.Collections.Generic;
using System.Text;

namespace MobileTracking.Core.Models
{
    public class SignalScore
    {
        public SignalScore(PositionData positionData, Measurement measurement)
        {
            this.PositionData = positionData;
            this.Measurement = measurement;
            CalculateScore();
        }
        public PositionData PositionData { get; set; }

        public Measurement Measurement { get; set; }

        public double Score { get; set; }

        private void CalculateScore()
        {
            if (Measurement.SignalType == SignalType.Magnetometer)
            {
                var distance = Math.Sqrt(Math.Pow(Measurement.Y - PositionData.Y, 2) + Math.Pow(Measurement.Z - PositionData.Z, 2));
                Score = (1 - distance) * 3;
                if (Score < 0.5)
                {
                    Score = 0.5;
                }
            }
            else
            {
                var proximityFactor = Measurement.Strength/PositionData.Strength;
                Score = (1 - Math.Abs(1 - proximityFactor)) * 3;
            }
        }
    }
}
