﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/locales")]
    public class LocalesController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Locale>>> GetLocales(
            [FromServices] ILocaleService localeService,
            [FromQuery] LocaleQuery query)
        {
            return await localeService.GetLocales(query);
        }

        [HttpGet("coordinates")]
        public async Task<ActionResult<List<Locale>>> GetLocalesByCoordinates(
            [FromServices] ILocaleService localeService,
            [FromQuery] LocaleQuery query)
        {
            return await localeService.FindLocalesByCoordinates(query);
        }

        [HttpGet("{localeId}")]
        public async Task<ActionResult<Locale>> GetLocaleById(
            [FromServices] ILocaleService localeService,
            [FromQuery] LocaleQuery query,
            [FromRoute] int localeId)
        {
            return await localeService.FindLocaleById(localeId, query);
        }

        [HttpGet("{localeId}/parameters")]
        public async Task<ActionResult<List<LocaleParameters>>> GetLocaleParameters(
            [FromServices] ILocaleService localeService,
            [FromRoute] int localeId)
        {
            return await localeService.GetLocaleParameters(localeId);
        }

        [HttpDelete("{localeId}")]
        public async Task<ActionResult<bool>> DeleteLocale(
            [FromServices] ILocaleService localeService,
            [FromRoute] int localeId)
        {
            return await localeService.DeleteLocale(localeId);
        }

        [HttpPatch("{localeId}")]
        public async Task<ActionResult<Locale>> UpdateLocale(
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
