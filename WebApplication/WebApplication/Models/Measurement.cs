using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Measurement
    {
        public string SignalId { get; set; } = string.Empty;

        public SignalType SignalType { get; set; }

        public float Strength { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
