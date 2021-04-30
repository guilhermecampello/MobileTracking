using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class UserLocalization
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int CalculatedPositionId { get; set; }

        public DateTime DateTime { get; set; }

        public Position? CalculatedPosition { get; set; }

        public List<LocalizationMeasurement>? LocalizationMeasurements { get; set; }
    }
}
