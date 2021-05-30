using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileTracking.Core.Models
{
    public class Measurement
    {
        public Measurement() { }

        public Measurement(string signalId, int strength, SignalType signalType, DateTime dateTime)
        {
            this.SignalType = signalType;
            this.SignalId = signalId;
            this.Strength = strength;
            this.DateTime = dateTime;
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
