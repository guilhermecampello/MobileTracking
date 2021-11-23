namespace MobileTracking.Core.Models
{
    public class LocalizationMeasurement : Measurement
    {
        public LocalizationMeasurement() { }

        public LocalizationMeasurement(Measurement measurement)
        {
            this.SignalId = measurement.SignalId;
            this.SignalType = measurement.SignalType;
            this.Strength = measurement.Strength;
            this.X = measurement.X;
            this.Y = measurement.Y;
            this.Z = measurement.Z;
            this.DateTime = measurement.DateTime;
        }

        public int Id { get; set; }

        public int UserLocalizationId { get; set; }
    }
}