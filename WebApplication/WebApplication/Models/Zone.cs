using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Zone
    {
        public int Id { get; set; }

        public int LocaleId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Floor { get; set; }

        public Locale? Locale { get; set; }

        public List<Position>? Positions { get; set; }
    }
}
