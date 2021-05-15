using MobileTracking.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MobileTracking.Pages.Views
{
    public class PositionDataView
    {
        private readonly PositionData positionData;

        public PositionDataView(PositionData positionData)
        {
            this.positionData = positionData;
        }

        public string Name {
            get
            {
                if (positionData.SignalType == SignalType.Magnetometer)
                {
                    return AppResources.Magnetic_field;
                }
                else
                {
                    return positionData.SignalId;
                }
            }
        }

        public string Type
        {
            get => positionData.SignalType.ToString();
        }

        public string Icon
        {
            get
            {
                switch (positionData.SignalType)
                {
                    case SignalType.Wifi:
                        return "\uf1eb";
                    case SignalType.Bluetooth:
                        return "\uf032";
                    default :
                        return "\uf076";
                }
            }
        }

        public Color IconColor
        {
            get
            {
                switch (positionData.SignalType)
                {
                    case SignalType.Wifi:
                        return Color.Black;
                    case SignalType.Bluetooth:
                        return Color.Blue;
                    default:
                        return Color.Gray;
                }
            }
        }

        public string ConfidenceInterval
        {
            get
            {
                if (positionData.SignalType == SignalType.Magnetometer)
                {
                    return $"X:{positionData.X.ToString("0.00")} \u00B1 {positionData.StandardDeviationX.ToString("0.00")} \n" +
                        $"Y:{positionData.Y.ToString("0.00")} \u00B1 {positionData.StandardDeviationY.ToString("0.00")} \n" +
                        $"Z:{positionData.Z.ToString("0.00")} \u00B1 {positionData.StandardDeviationZ.ToString("0.00")} \n" +
                        $"T:{positionData.Strength.ToString("0.00")}  \u00B1 {positionData.StandardDeviation.ToString("0.00")}";
                }
                else
                {
                    return $"{positionData.Strength.ToString("0.00")} \u00B1 {positionData.StandardDeviation.ToString("0.00")}";
                }
            }
        }

        public string StrengthAbsoluteInterval
        {
            get
            {
                if (positionData.SignalType == SignalType.Magnetometer)
                {
                    return $"X:{positionData.MinX.ToString("0.00")}; {positionData.MaxX.ToString("0.00")} \n" +
                        $"Y:{positionData.MinY.ToString("0.00")}; {positionData.MaxY.ToString("0.00")} \n" +
                        $"Z:{positionData.MinZ.ToString("0.00")};  {positionData.MaxZ.ToString("0.00")} \n" +
                        $"T:{positionData.Min.ToString("0.00")}; {positionData.Max.ToString("0.00")}";
                }
                else
                {
                    return $"{positionData.Min.ToString("0.00")}; {positionData.Max.ToString("0.00")}";
                }
            }
        }

        public string LastUpdate { get => this.positionData.LastUpdate.ToString(); }
    }
}
