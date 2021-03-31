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

        int count = 0;

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
            Vector3 vector;
            float num12 = rotation.X + rotation.X;
            float num2 = rotation.Y + rotation.Y;
            float num = rotation.Z + rotation.Z;
            float num11 = rotation.W * num12;
            float num10 = rotation.W * num2;
            float num9 = rotation.W * num;
            float num8 = rotation.X * num12;
            float num7 = rotation.X * num2;
            float num6 = rotation.X * num;
            float num5 = rotation.Y * num2;
            float num4 = rotation.Y * num;
            float num3 = rotation.Z * num;
            float num15 = ((value.X * ((1f - num5) - num3)) + (value.Y * (num7 - num9))) + (value.Z * (num6 + num10));
            float num14 = ((value.X * (num7 + num9)) + (value.Y * ((1f - num8) - num3))) + (value.Z * (num4 - num11));
            float num13 = ((value.X * (num6 - num10)) + (value.Y * (num4 + num11))) + (value.Z * ((1f - num8) - num5));
            vector.X = num15;
            vector.Y = num14;
            vector.Z = num13;
            return vector;
        }

        private void OrientationSensor_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            if (OrientationSensorData.Count >= 100)
            {
                OrientationSensorData.RemoveAt(0);
            }

            OrientationSensorData.Add((e.Reading.Orientation, DateTime.Now));
            var orientation = e.Reading.Orientation;
            if (count == 20)
            {
                //Console.WriteLine($"{orientation.X.ToString("0.00")}    {orientation.Y.ToString("0.00")}   {orientation.Z.ToString("0.00")}    {orientation.W.ToString("0.00")}");
                var vector = MagneticFieldVector;
                Console.WriteLine($"{vector.X.ToString("0.00")}    {vector.Y.ToString("0.00")}   {vector.Z.ToString("0.00")}");
                count = 0;
            }

            count++;
        }

        private void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            if (MagnetometerData.Count >= 100)
            {
                MagnetometerData.RemoveAt(0);
            }

            MagnetometerData.Add((e.Reading.MagneticField, DateTime.Now));
            var magnetic = e.Reading.MagneticField;
            if(count == 9)
            {
                //Console.WriteLine($"{magnetic.X.ToString("0.00")}     {magnetic.Y.ToString("0.00")}     {magnetic.Z.ToString("0.00")}");
            }
        }
    }
}
