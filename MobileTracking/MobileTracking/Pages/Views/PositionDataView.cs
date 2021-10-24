using MobileTracking.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MobileTracking.Pages.Views
{
    public class PositionSignalDataView
    {
        public PositionSignalDataView(PositionSignalData positionSignalData)
        {
            this.PositionSignalData = positionSignalData;
        }
        
        public PositionSignalData PositionSignalData { get; set; }

        public string Name {
            get
            {
                if (PositionSignalData.SignalType == SignalType.Magnetometer)
                {
                    return AppResources.Magnetic_field;
                }
                else
                {
                    return PositionSignalData.SignalId;
                }
            }
        }

        public string Samples { get => PositionSignalData.Samples.ToString(); }

        public string Type
        {
            get => PositionSignalData.SignalType.ToString();
        }

        public string Icon
        {
            get
            {
                switch (PositionSignalData.SignalType)
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
                switch (PositionSignalData.SignalType)
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
                if (PositionSignalData.SignalType == SignalType.Magnetometer)
                {
                    return $"X:{PositionSignalData.X.ToString("0.00")} \u00B1 {PositionSignalData.StandardDeviationX.ToString("0.00")} \n" +
                        $"Y:{PositionSignalData.Y.ToString("0.00")} \u00B1 {PositionSignalData.StandardDeviationY.ToString("0.00")} \n" +
                        $"Z:{PositionSignalData.Z.ToString("0.00")} \u00B1 {PositionSignalData.StandardDeviationZ.ToString("0.00")} \n" +
                        $"T:{PositionSignalData.Strength.ToString("0.00")}  \u00B1 {PositionSignalData.StandardDeviation.ToString("0.00")}";
                }
                else
                {
                    return $"{PositionSignalData.Strength.ToString("0.00")} \u00B1 {PositionSignalData.StandardDeviation.ToString("0.00")}";
                }
            }
        }

        public string StrengthAbsoluteInterval
        {
            get
            {
                if (PositionSignalData.SignalType == SignalType.Magnetometer)
                {
                    return $"X:{PositionSignalData.MinX.ToString("0.00")}; {PositionSignalData.MaxX.ToString("0.00")} \n" +
                        $"Y:{PositionSignalData.MinY.ToString("0.00")}; {PositionSignalData.MaxY.ToString("0.00")} \n" +
                        $"Z:{PositionSignalData.MinZ.ToString("0.00")};  {PositionSignalData.MaxZ.ToString("0.00")} \n" +
                        $"T:{PositionSignalData.Min.ToString("0.00")}; {PositionSignalData.Max.ToString("0.00")}";
                }
                else
                {
                    return $"{PositionSignalData.Min.ToString("0.00")}; {PositionSignalData.Max.ToString("0.00")}";
                }
            }
        }

        public string LastSeen { get => this.PositionSignalData.LastSeen.ToLocalTime().ToString(); }
    }
}
