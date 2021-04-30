using System.Threading.Tasks;
using MobileTracking.Core.Models;

namespace MobileTracking.Core.Application
{
    public interface ILocaleService
    {
        Task<Locale> FindLocaleById(int localeId);

        Task<Locale> FindLocaleByCoordinates(float latitude, float longitude);

        Task<Locale> CreateLocale(CreateOrUpdateLocaleCommand command);

        Task<Locale> UpdateLocale(int localeId, CreateOrUpdateLocaleCommand command);

        Task<bool> DeleteLocale(int localeId);
    }
}
