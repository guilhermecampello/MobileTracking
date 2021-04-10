using System;
using WebApplication.Infrastructure;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class LocaleService : ILocaleService
    {
        private readonly DatabaseContext databaseContext;

        public LocaleService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public Locale FindById(int localeId)
        {
            throw new NotImplementedException();
        }

        public Locale FindByCoordinates(float latitude, float longitude)
        {
            throw new NotImplementedException();
        }

        public bool DeleteLocale(int localeId)
        {
            throw new NotImplementedException();
        }

        public Locale CreateLocale(string name, string? description, float latitude, float longitude)
        {
            throw new NotImplementedException();
        }

        public Locale UpdateLocale(int localeId, string? name, string? description, float? latitude, float? longitude)
        {
            throw new NotImplementedException();
        }
    }
}
