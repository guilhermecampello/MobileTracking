using MobileTracking.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MobileTracking.Pages.Views
{
    public class PositionDataView
    {
        public PositionDataView(PositionData positionData)
        {
            this.PositionData = positionData;
        }
        
        public PositionData PositionData { get; set; }

        public string Name {
            get
            {
                if (PositionData.SignalType == SignalType.Magnetometer)
                {
                    return AppResources.Magnetic_field;
                }
                else
                {
                    return PositionData.SignalId;
                }
            }
        }

        public string Samples { get => PositionData.Samples.ToString(); }

        public string Type
        {
            get => PositionData.SignalType.ToString();
        }

        public string Icon
        {
            get
            {
                switch (PositionData.SignalType)
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
                switch (PositionData.SignalType)
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
                if (PositionData.SignalType == SignalType.Magnetometer)
                {
                    return $"X:{PositionData.X.ToString("0.00")} \u00B1 {PositionData.StandardDeviationX.ToString("0.00")} \n" +
                        $"Y:{PositionData.Y.ToString("0.00")} \u00B1 {PositionData.StandardDeviationY.ToString("0.00")} \n" +
                        $"Z:{PositionData.Z.ToString("0.00")} \u00B1 {PositionData.StandardDeviationZ.ToString("0.00")} \n" +
                        $"T:{PositionData.Strength.ToString("0.00")}  \u00B1 {PositionData.StandardDeviation.ToString("0.00")}";
                }
                else
                {
                    return $"{PositionData.Strength.ToString("0.00")} \u00B1 {PositionData.StandardDeviation.ToString("0.00")}";
                }
            }
        }

        public string StrengthAbsoluteInterval
        {
            get
            {
                if (PositionData.SignalType == SignalType.Magnetometer)
                {
                    return $"X:{PositionData.MinX.ToString("0.00")}; {PositionData.MaxX.ToString("0.00")} \n" +
                        $"Y:{PositionData.MinY.ToString("0.00")}; {PositionData.MaxY.ToString("0.00")} \n" +
                        $"Z:{PositionData.MinZ.ToString("0.00")};  {PositionData.MaxZ.ToString("0.00")} \n" +
                        $"T:{PositionData.Min.ToString("0.00")}; {PositionData.Max.ToString("0.00")}";
                }
                else
                {
                    return $"{PositionData.Min.ToString("0.00")}; {PositionData.Max.ToString("0.00")}";
                }
            }
        }

        public string LastUpdate { get => this.PositionData.LastUpdate.ToLocalTime().ToString(); }
    }
}
