using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobileTracking.Communication.ClientServices
{
    public class LocalesService : ILocaleService
    {
        private readonly Client client;

        private string localesController = "locales";

        public LocalesService(Client client)
        {
            this.client = client;
        }

        public Task<Locale> CreateLocale(CreateOrUpdateLocaleCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteLocale(int localeId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Locale>> FindLocalesByCoordinates(float latitude, float longitude)
        {
            var query = new LocaleQuery()
            {
                Latitude = latitude,
                Longitude = longitude,
                IncludeZones = true,
                IncludePositions = true
            };
            return client.Get<List<Locale>>(localesController, "coordinates", query);
        }

        public Task<Locale> FindLocaleById(int localeId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Locale>> GetLocales(LocaleQuery query)
        {
            return await this.client.Get<List<Locale>>(localesController, query);
        }

        public Task<Locale> UpdateLocale(int localeId, CreateOrUpdateLocaleCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
