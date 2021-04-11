using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Application;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/positions")]
    public class PositionsController : ControllerBase
    {
        [HttpGet("{positionId}")]
        public async Task<ActionResult<Position>> GetPositionById(
            [FromServices] IPositionService positionService,
            [FromRoute] int positionId)
        {
            return await positionService.FindPositionById(positionId);
        }

        [HttpDelete("{positionId}")]
        public async Task<ActionResult<bool>> DeletePosition(
            [FromServices] IPositionService positionService,
            [FromRoute] int positionId)
        {
            return await positionService.DeletePosition(positionId);
        }

        [HttpPatch("{positionId}")]
        public async Task<ActionResult<Position>> UpdatePosition(
            [FromServices] IPositionService positionService,
            [FromRoute] int positionId,
            [FromBody] CreateOrUpdatePositionCommand command)
        {
            return await positionService.UpdatePosition(positionId, command);
        }

        [HttpPost]
        public async Task<ActionResult<Position>> CreatePosition(
            [FromServices] IPositionService positionService,
            [FromBody] CreateOrUpdatePositionCommand command)
        {
            return await positionService.CreatePosition(command);
        }
    }
}
