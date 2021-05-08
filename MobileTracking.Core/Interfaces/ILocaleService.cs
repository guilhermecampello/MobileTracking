using System.Collections.Generic;
using System.Threading.Tasks;
using MobileTracking.Core.Models;

namespace MobileTracking.Core.Application
{
    public interface ILocaleService
    {
        Task<List<Locale>> GetLocales(LocaleQuery query);

        Task<Locale> FindLocaleById(int localeId);

        Task<List<Locale>> FindLocalesByCoordinates(float latitude, float longitude);

        Task<Locale> CreateLocale(CreateOrUpdateLocaleCommand command);

        Task<Locale> UpdateLocale(int localeId, CreateOrUpdateLocaleCommand command);

        Task<bool> DeleteLocale(int localeId);
    }
}
