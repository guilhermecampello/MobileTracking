using System;

namespace WebApplication.Models
{
    public class PositionData : Measurement
    {
        public int Id { get; set; }

        public int PositionId { get; set; }

        public DateTime LastUpdate { get; set; }

        public Position? Position { get; set; }
    }
}