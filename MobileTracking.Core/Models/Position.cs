using System;
using System.Collections.Generic;

namespace MobileTracking.Core.Models
{
    public class Position
    {
        public Position() { }

        public Position(int zoneId, string name, int x, int y, int z)
        {
            this.ZoneId = zoneId;
            this.Name = name;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public int Id { get; set; }

        public int ZoneId { get; set; }

        public string Name { get; set; } = string.Empty;

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public Zone? Zone { get; set; }

        public bool DataNeedsUpdate { get; set; }

        public List<PositionData>? PositionData { get; set; }

        public List<Calibration>? Calibrations { get; set; }
    }
}
