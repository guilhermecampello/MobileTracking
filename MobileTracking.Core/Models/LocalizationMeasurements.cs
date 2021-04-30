namespace WebApplication.Models
{
    public class LocalizationMeasurement : Measurement
    {
        public int Id { get; set; }

        public int UserLocalizationId { get; set; }
    }
}