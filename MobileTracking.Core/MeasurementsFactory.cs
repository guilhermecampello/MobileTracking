using MobileTracking.Core.Models;
using System;
using System.Numerics;

namespace MobileTracking.Core
{
    public class MeasurementsFactory
    {
        public static Measurement CreateWifiMeasurement(string signalId, int rssi)
        {
            return new Measurement()
            {
                SignalId = signalId,
                SignalType = SignalType.Wifi,
                Strength = rssi,
                DateTime = DateTime.Now
            };
        }

        public static Measurement CreateBluetoothMeasurement(string signalId, int rssi)
        {
            return new Measurement()
            {
                SignalId = signalId,
                SignalType = SignalType.Bluetooth,
                Strength = rssi,
                DateTime = DateTime.Now
            };
        }

        public static Measurement CreateMagnetometerMeasurement(Vector3 intensitiesVector)
        {
            return new Measurement()
            {
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
