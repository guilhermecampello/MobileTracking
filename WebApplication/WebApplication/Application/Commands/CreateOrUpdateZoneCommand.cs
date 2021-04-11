using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Application
{
    public class CreateOrUpdateZoneCommand
    {
        [Required]
        public int? LocaleId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? Floor { get; set; }
    }
}
