using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileTracking.Core.Models
{
    public class Calibration : Measurement
    {
        public Calibration() { }

        public Calibration(int positionId, Measurement measurement)
        {
            this.PositionId = positionId;
            this.SignalId = measurement.SignalId;
            this.SignalType = measurement.SignalType;
            this.Strength = measurement.Strength;
            this.X = measurement.X;
            this.Y = measurement.Y;
            this.Z = measurement.Z;
            this.DateTime = measurement.DateTime;
        }

        public int Id { get; set; }

        public int PositionId { get; set; }

        public Position? Position { get; set; }
    }
}
