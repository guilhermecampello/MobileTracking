using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/zones")]
    public class ZoneesController : ControllerBase
    {
        [HttpGet("{zoneId}")]
        public async Task<ActionResult<Zone>> GetZoneById(
            [FromServices] IZoneService zoneService,
            [FromRoute] int zoneId)
        {
            return await zoneService.FindZoneById(zoneId);
        }

        [HttpGet]
        public async Task<ActionResult<List<Zone>>> GetZones(
            [FromServices] IZoneService zoneService,
            [FromQuery] ZoneQuery query)
        {
            return await zoneService.GetZones(query);
        }

        [HttpDelete("{zoneId}")]
        public async Task<ActionResult<bool>> DeleteZone(
            [FromServices] IZoneService zoneService,
            [FromRoute] int zoneId)
        {
            return await zoneService.DeleteZone(zoneId);
        }

        [HttpPatch("{zoneId}")]
        public async Task<ActionResult<Zone>> UpdateZone(
            [FromServices] IZoneService zoneService,
            [FromRoute] int zoneId,
            [FromBody] CreateOrUpdateZoneCommand command)
        {
            return await zoneService.UpdateZone(zoneId, command);
        }

        [HttpPost]
        public async Task<ActionResult<Zone>> CreateZone(
            [FromServices] IZoneService zoneService,
            [FromBody] CreateOrUpdateZoneCommand command)
        {
            return await zoneService.CreateZone(command);
        }
    }
}
