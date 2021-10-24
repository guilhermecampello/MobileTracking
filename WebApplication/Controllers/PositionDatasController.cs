using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/position-datas")]
    public class PositionSignalDatasController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<PositionSignalData>>> GetPositionSignalDatas(
            [FromServices] IPositionSignalDataService positionSignalDataService,
            [FromQuery] PositionSignalDataQuery query)
        {
            return await positionSignalDataService.GetPositionSignalDatas(query);
        }

        [HttpPut("recalculate")]
        public async Task<ActionResult<bool>> RecalculatePositionSignalData(
            [FromServices] IPositionSignalDataService positionSignalDataService,
            [FromRoute] PositionSignalDataQuery positionSignalDataQuery)
        {
            return await positionSignalDataService.RecalculatePositionSignalData(positionSignalDataQuery);
        }
    }
}
