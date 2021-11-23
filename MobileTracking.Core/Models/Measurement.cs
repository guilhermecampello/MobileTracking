using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileTracking.Core.Models
{
    public class Measurement
    {
        public Measurement() { }

        public Measurement(LocalizationMeasurement localizationMeasurement)
        {
            this.SignalType = localizationMeasurement.SignalType;
            this.SignalId = localizationMeasurement.SignalId;
            this.Strength = localizationMeasurement.Strength;
            this.DateTime = localizationMeasurement.DateTime;
            this.X = localizationMeasurement.X;
            this.Y = localizationMeasurement.Y;
            this.Z = localizationMeasurement.Z;
        }

        public Measurement(string signalId, int strength, SignalType signalType, DateTime dateTime, float x = 0, float y = 0, float z = 0)
        {
            this.SignalType = signalType;
            this.SignalId = signalId;
            this.Strength = strength;
            this.DateTime = dateTime;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public string SignalId { get; set; } = string.Empty;

        public SignalType SignalType { get; set; }

        public float Strength { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
