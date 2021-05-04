using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using Xamarin.Essentials;

namespace MobileTracking.Services.MagneticField
{
    public class MagneticFieldSensor
    {
        private List<(Vector3, DateTime)> MagnetometerData { get; set; } = new List<(Vector3, DateTime)>();

        private List<(Quaternion, DateTime)> OrientationSensorData { get; set; } = new List<(Quaternion, DateTime)>();

        public MagneticFieldSensor()
        {
        }

        public void Start()
        {
            OrientationSensor.Start(SensorSpeed.Fastest);
            OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
            Magnetometer.Start(SensorSpeed.Fastest);
            Magnetometer.ReadingChanged += Magnetometer_ReadingChanged;
        }

        public Vector3 MagneticFieldVector
        {
            get
            {
                var now = DateTime.Now;
                var vector = new Vector3(0);
                var n = 0;
                OrientationSensorData.ForEach(orientationSample =>
                {
                    var sampleTime = orientationSample.Item2;
                    if (now.Subtract(sampleTime).TotalSeconds < 2)
                    {
                        var orientation = orientationSample.Item1;
                        var magneticFieldSample = MagnetometerData
                        .OrderBy(sample => Math.Abs(sample.Item2.Subtract(sampleTime).TotalMilliseconds))
                        .FirstOrDefault()
                        .Item1;

                        var sampleVector = Transform(magneticFieldSample, orientation);
                        vector += sampleVector;
                        n += 1;
                    }
                });

                if (n > 0)
                {
                    vector.X = vector.X / n;
                    vector.Y = vector.Y / n;
                    vector.Z = vector.Z / n;
                }

                return vector;
                
            }
        }

        public Vector3 Transform(Vector3 value, Quaternion rotation)
        {
            float x2 = rotation.X + rotation.X;
            float y2 = rotation.Y + rotation.Y;
            float z2 = rotation.Z + rotation.Z;

            float wx2 = rotation.W * x2;
            float wy2 = rotation.W * y2;
            float wz2 = rotation.W * z2;
            float xx2 = rotation.X * x2;
            float xy2 = rotation.X * y2;
            float xz2 = rotation.X * z2;
            float yy2 = rotation.Y * y2;
            float yz2 = rotation.Y * z2;
            float zz2 = rotation.Z * z2;

            return new Vector3(
                value.X * (1.0f - yy2 - zz2) + value.Y * (xy2 - wz2) + value.Z * (xz2 + wy2),
                value.X * (xy2 + wz2) + value.Y * (1.0f - xx2 - zz2) + value.Z * (yz2 - wx2),
                value.X * (xz2 - wy2) + value.Y * (yz2 + wx2) + value.Z * (1.0f - xx2 - yy2));
        }

        private void OrientationSensor_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            if (OrientationSensorData.Count >= 100)
            {
                OrientationSensorData.RemoveAt(0);
            }

            OrientationSensorData.Add((e.Reading.Orientation, DateTime.Now));
        }

        private void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            if (MagnetometerData.Count >= 100)
            {
                var data = "";
                try
                {
                    MagnetometerData.ForEach(sample => {
                        var orientation = OrientationSensorData.Last().Item1;
                        sample.Item1 = Transform(sample.Item1, orientation);
                        data += $"{sample.Item2.Ticks}; {sample.Item1.X}; {sample.Item1.Y}; {sample.Item1.Z} \n";
                        });
         
                    MagnetometerData.Clear();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            MagnetometerData.Add((e.Reading.MagneticField, DateTime.Now));
        }
    }
}
