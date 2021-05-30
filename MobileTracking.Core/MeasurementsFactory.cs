using MobileTracking.Core.Models;
using System;
using System.Numerics;

namespace MobileTracking.Core
{
    public class MeasurementsFactory
    {
        public static Measurement CreateMagnetometerMeasurement(Vector3 intensitiesVector)
        {
            return new Measurement()
            {
                SignalId = "Magnetic Field",
                SignalType = SignalType.Magnetometer,
                Strength = intensitiesVector.Length(),
                X = intensitiesVector.X,
                Y = intensitiesVector.Y,
                Z = intensitiesVector.Z,
                DateTime = DateTime.Now
            };
        }
    }
}
