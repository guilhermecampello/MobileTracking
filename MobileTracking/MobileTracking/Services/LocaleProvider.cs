using MobileTracking.Communication.ClientServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MobileTracking.Services
{
    public class LocaleProvider
    {
        private readonly LocalesService localesService;

        public LocaleProvider(LocalesService localesService)
        {
            this.localesService = localesService;
            TryGetLocalesByCoordinates();
        }
        
        public Core.Models.Locale? Locale { get; set; }

        public List<Core.Models.Locale> ClosestLocales { get; set; } = new List<Core.Models.Locale>();

        public async Task<List<Core.Models.Locale>> TryGetLocalesByCoordinates()
        {
            var coordinates = await Geolocation.GetLocationAsync();
            try
            {
                ClosestLocales = await this.localesService.FindLocalesByCoordinates(
                    (float)coordinates.Latitude, (float)coordinates.Longitude);
                
                return ClosestLocales;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Core.Models.Locale>();
            }
        }
    }
}
