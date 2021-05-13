using System;
using System.Collections.Generic;
using System.Text;

namespace MobileTracking
{
    public class Configuration
    {
        public string Hostname { get; set; } = "192.168.1.6";

        public int SamplesPerPosition { get; set; } = 50;

        public int DataAquisitionInterval { get; set; } = 5;
    }
}
