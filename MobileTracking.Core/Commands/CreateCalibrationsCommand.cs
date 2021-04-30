using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication.Models;

namespace WebApplication.Application
{
    public class CreateCalibrationsCommand
    {
        [Required]
        public int PositionId { get; set; }

        public List<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
}
