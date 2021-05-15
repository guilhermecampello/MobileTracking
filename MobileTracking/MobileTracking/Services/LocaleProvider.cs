using MobileTracking.Communication.ClientServices;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Essentials;
using MobileTracking.Pages.Locales;

namespace MobileTracking.Services
{
    public class LocaleProvider
    {
        private readonly ILocaleService localesService;

        public LocaleProvider(ILocaleService localesService)
        {
            this.localesService = localesService;
            TryGetLocalesByCoordinates();
        }
        
        private Core.Models.Locale? locale;
        public Core.Models.Locale? Locale {
            get => locale; 
            set 
            {
                locale = value;
                LocaleView.Locale = value;
                LocaleView.IsSelected = true;
            } 
        }

        public LocaleView LocaleView { get; set; } = new LocaleView(null, false);

        public List<Core.Models.Locale> ClosestLocales { get; set; } = new List<Core.Models.Locale>();

        public async Task RefreshLocale()
        {
            if (this.Locale != null)
            {
                this.Locale = await localesService.FindLocaleById(this.Locale.Id, new LocaleQuery() { IncludeZones = true, IncludePositions = true }) ;
            }
        }

        public async Task<List<Core.Models.Locale>> TryGetLocalesByCoordinates()
        {
            if (LastLocation == null || DateTime.Now.Subtract(LastCoordinatesUpdate).TotalMinutes > 10)
            {
                LastLocation = await Geolocation.GetLocationAsync();
                LastCoordinatesUpdate = DateTime.Now;
            }
            try
            {
                ClosestLocales = await this.localesService.FindLocalesByCoordinates(
                    new LocaleQuery(LastLocation.Latitude, LastLocation.Longitude));
                Locale = ClosestLocales.FirstOrDefault();
                
                return ClosestLocales;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Core.Models.Locale>();
            }
        }

        public DateTime LastCoordinatesUpdate { get; set; }

        public Location? LastLocation { get; set; }
    }
}
