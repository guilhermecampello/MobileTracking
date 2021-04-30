using System;
using System.ComponentModel.DataAnnotations;

namespace MobileTracking.Core.Application
{
    public class CreateOrUpdatePositionCommand
    {
        [Required]
        public int? ZoneId { get; set; }

        public string? Name { get; set; }

        public int? X { get; set; }

        public int? Y { get; set; }

        public int? Z { get; set; }
    }
}
