using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Calibration : Measurement
    {
        public int Id { get; set; }

        public int PositionId { get; set; }
    }
}
