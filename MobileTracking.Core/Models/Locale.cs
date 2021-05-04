using System;
using System.Collections.Generic;

namespace MobileTracking.Core.Models
{
    public class Locale
    {
        public Locale(string name, string? description, float latitude, float longitude)
        {
            this.Name = name;
            this.Description = description;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<Zone>? Zones { get; set; }
    }
}
