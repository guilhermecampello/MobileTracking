using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/position-datas")]
    public class PositionDatasController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<PositionData>>> GetPositionDatas(
            [FromServices] IPositionDataService positionDataService,
            [FromQuery] PositionDataQuery query)
        {
            return await positionDataService.GetPositionDatas(query);
        }

        [HttpPut("recalculate")]
        public async Task<ActionResult<bool>> RecalculatePositionData(
            [FromServices] IPositionDataService positionDataService,
            [FromRoute] PositionDataQuery positionDataQuery)
        {
            return await positionDataService.RecalculatePositionData(positionDataQuery);
        }
    }
}
