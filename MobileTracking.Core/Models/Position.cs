using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Position
    {
        public Position(int zoneId, string name, int x, int y, int z)
        {
            this.ZoneId = zoneId;
            this.Name = name;
            this.XCoordinate = x;
            this.YCoordinate = y;
            this.ZCoordinate = z;
        }

        public int Id { get; set; }

        public int ZoneId { get; set; }

        public string Name { get; set; } = string.Empty;

        public float XCoordinate { get; set; }

        public float YCoordinate { get; set; }

        public float ZCoordinate { get; set; }

        public Zone? Zone { get; set; }

        public List<PositionData>? PositionData { get; set; }

        public List<Calibration>? Calibrations { get; set; }
    }
}
