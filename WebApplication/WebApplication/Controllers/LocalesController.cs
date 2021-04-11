using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.Application;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/locales")]
    public class LocalesController : ControllerBase
    {
        [HttpGet("{localeId}")]
        public async Task<ActionResult<Locale>> GetLocaleById(
            [FromServices] ILocaleService localeService,
            [FromRoute] int localeId)
        {
            return await localeService.FindLocaleById(localeId);
        }

        [HttpDelete("{localeId}")]
        public async Task<ActionResult<bool>> DeleteLocale(
            [FromServices] ILocaleService localeService,
            [FromRoute] int localeId)
        {
            return await localeService.DeleteLocale(localeId);
        }

        [HttpPatch("{localeId}")]
        public async Task<ActionResult<Locale>> CreateLocale(
            [FromServices] ILocaleService localeService,
            [FromRoute] int localeId,
            [FromBody] CreateOrUpdateLocaleCommand command)
        {
            return await localeService.UpdateLocale(localeId, command);
        }

        [HttpPost]
        public async Task<ActionResult<Locale>> CreateLocale(
            [FromServices] ILocaleService localeService,
            [FromBody] CreateOrUpdateLocaleCommand command)
        {
            return await localeService.CreateLocale(command);
        }
    }
}
