using WebApplication.Models;

namespace WebApplication.Services
{

    public interface ILocaleService
    {
        Locale FindById(int localeId);

        Locale FindByCoordinates(float latitude, float longitude);

        Locale CreateLocale(string name, string? description, float latitude, float longitude);

        Locale UpdateLocale(int localeId, string? name, string? description, float? latitude, float? longitude);

        bool DeleteLocale(int localeId);
    }
}
