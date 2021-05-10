﻿using System;

namespace MobileTracking.Core.Application
{
    public class CreateOrUpdateLocaleCommand
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}
