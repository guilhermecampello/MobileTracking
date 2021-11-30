using System;
using System.Collections.Generic;

namespace MobileTracking.Core.Models
{
    public class Locale
    {
        public Locale() { }

        public Locale(string name, string? description, double latitude, double longitude)
        {
            this.Name = name;
            this.Description = description;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<Zone>? Zones { get; set; }

        public List<LocaleParameters>? Parameters { get;set; }
    }
}
