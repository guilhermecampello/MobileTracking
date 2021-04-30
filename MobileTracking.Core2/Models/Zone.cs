using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileTracking.Core.Models
{
    public class Zone
    {
        public Zone(int localeId, string name, string? description, int floor)
        {
            this.LocaleId = localeId;
            this.Name = name;
            this.Description = description;
            this.Floor = floor;
        }

        public int Id { get; set; }

        public int LocaleId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Floor { get; set; }

        public Locale? Locale { get; set; }

        public List<Position>? Positions { get; set; }
    }
}
