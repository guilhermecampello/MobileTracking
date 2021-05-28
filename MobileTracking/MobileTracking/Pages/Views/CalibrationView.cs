using MobileTracking.Core.Models;
using System.Drawing;

namespace MobileTracking.Pages.Views
{
    public class CalibrationView
    {
        private readonly Calibration calibration;

        public CalibrationView(Calibration calibration)
        {
            this.calibration = calibration;
        }

        public int Id { get => calibration.Id; }

        public string Name
        {
            get
            {
                if (calibration.SignalType == SignalType.Magnetometer)
                {
                    return AppResources.Magnetic_field;
                }
                else
                {
                    return calibration.SignalId;
                }
            }
        }

        public string Type
        {
            get => calibration.SignalType.ToString();
        }

        public string Icon
        {
            get
            {
                switch (calibration.SignalType)
                {
                    case SignalType.Wifi:
                        return "\uf1eb";
                    case SignalType.Bluetooth:
                        return "\uf032";
                    default:
                        return "\uf076";
                }
            }
        }

        public Color IconColor
        {
            get
            {
                switch (calibration.SignalType)
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

        public string Strength
        {
            get
            {
                if (calibration.SignalType == SignalType.Magnetometer)
                {
                    return $"X:{calibration.X.ToString("0.00")}\n" +
                        $"Y:{calibration.Y.ToString("0.00")}\n" +
                        $"Z:{calibration.Z.ToString("0.00")}\n" +
                        $"T:{calibration.Strength.ToString("0.00")}";
                }
                else
                {
                    return $"{calibration.Strength.ToString("0.00")}";
                }
            }
        }

        public string Date { get => this.calibration.DateTime.ToLocalTime().ToString(); }
    }
}
