using System;
using System.Collections.Generic;
using System.Text;

namespace MobileTracking
{
    public class Configuration
    {
        public string Hostname { get; set; } = "https://mobilelocalization.azurewebsites.net";

        public int SamplesPerPosition { get; set; } = 50;

        public int DataAquisitionInterval { get; set; } = 5;

        public int K { get; set; } = 3;
    }
}
