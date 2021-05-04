using System;

namespace MobileTracking.Core.Application
{
    public class CreateOrUpdateLocaleCommand
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public float? Latitude { get; set; }

        public float? Longitude { get; set; }
    }
}
