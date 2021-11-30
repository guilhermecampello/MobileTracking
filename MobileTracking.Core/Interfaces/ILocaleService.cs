using System.Collections.Generic;
using System.Threading.Tasks;
using MobileTracking.Core.Models;

namespace MobileTracking.Core.Application
{
    public interface ILocaleService
    {
        Task<List<Locale>> GetLocales(LocaleQuery query);

        Task<Locale> FindLocaleById(int localeId, LocaleQuery? query);

        Task<List<Locale>> FindLocalesByCoordinates(LocaleQuery query);

        Task<List<LocaleParameters>> GetLocaleParameters(int localeId);

        Task<Locale> CreateLocale(CreateOrUpdateLocaleCommand command);

        Task<Locale> UpdateLocale(int localeId, CreateOrUpdateLocaleCommand command);

        Task<bool> DeleteLocale(int localeId);
    }
}
