using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Application
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
