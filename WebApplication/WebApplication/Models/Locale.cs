using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public class Locale
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public List<Zone>? Zones { get; set; }
    }
}
