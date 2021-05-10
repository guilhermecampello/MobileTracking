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

        public async Task<Locale> CreateLocale(CreateOrUpdateLocaleCommand command)
        {
            return await this.client.Post<Locale>(localesController, command);
        }

        public Task<bool> DeleteLocale(int localeId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Locale>> FindLocalesByCoordinates(LocaleQuery query)
        {
            query.IncludeZones = true;
            query.IncludePositions = true;
            
            return client.Get<List<Locale>>(localesController, "coordinates", query);
        }

        public Task<Locale> FindLocaleById(int localeId, LocaleQuery? query)
        {
            return client.Get<Locale>(localesController, localeId.ToString(), query);
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
