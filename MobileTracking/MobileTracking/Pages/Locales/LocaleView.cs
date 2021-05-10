using MobileTracking.Core.Models;

namespace MobileTracking.Pages.Locales
{
    public class LocaleView : Locale
    {
        public LocaleView(Locale locale, bool selected)
        {
            this.Id = locale.Id;
            this.Latitude = locale.Latitude;
            this.Longitude = locale.Longitude;
            this.Name = locale.Name;
            this.Description = locale.Description;
            this.Zones = locale.Zones;
            this.IsSelected = selected;
        }

        public bool IsSelected { get; set; }
    }
}
