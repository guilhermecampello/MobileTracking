using System;
using System.Collections.Generic;
using System.Text;

namespace MobileTracking
{
    public class Marker
    {
        public Marker(string name, double x, double y)
        {
            this.Name = name;
            this.X = x;
            this.Y = y;
        }

        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
