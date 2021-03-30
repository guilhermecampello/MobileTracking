using System;
using System.Collections.Generic;
using System.Linq;
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
            OrientationSensor.Start(SensorSpeed.Game);
            OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
            Magnetometer.Start(SensorSpeed.Game);
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

                        var sampleVector = Vector3.Transform(magneticFieldSample, Matrix4x4.CreateFromQuaternion(orientation));
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
                MagnetometerData.RemoveAt(0);
            }

            MagnetometerData.Add((e.Reading.MagneticField, DateTime.Now));
        }
    }
}
