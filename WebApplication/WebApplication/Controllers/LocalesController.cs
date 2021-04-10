using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("locales")]
    public class LocalesController : ControllerBase
    {
        [HttpGet("{localeId}")]
        public Locale GetLocaleById(
            [FromServices] ILocaleService localeService,
            [FromRoute] int localeId)
        {
            return localeService.FindById(localeId);
        }

        [HttpDelete("{localeId}")]
        public bool DeleteLocale(
            [FromServices] ILocaleService localeService,
            [FromRoute] int localeId)
        {
            return localeService.DeleteLocale(localeId);
        }

        [HttpPatch("{localeId}")]
        public Locale CreateLocale(
            [FromServices] ILocaleService localeService,
            [FromRoute] int localeId,
            [FromBody] string? name,
            [FromBody] string? description,
            [FromBody] float? latitude,
            [FromBody] float? longitude)
        {
            return localeService.UpdateLocale(localeId, name, description, latitude, longitude);
        }

        [HttpPost]
        public Locale CreateLocale(
            [FromServices] ILocaleService localeService,
            [FromBody] string name,
            [FromBody] string description,
            [FromBody] float latitude,
            [FromBody] float longitude)
        {
            return localeService.CreateLocale(name, description, latitude, longitude);
        }
    }
}
