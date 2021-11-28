using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileTracking.Core.Commands;
using MobileTracking.Core.Interfaces;
using MobileTracking.Core.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/position-estimation")]
    public class PositionEstimationController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<PositionEstimation>> EstimatePosition(
            [FromServices] IPositionEstimationService positionEstimationService,
            [FromBody] EstimatePositionCommand command)
        {
            return await positionEstimationService.EstimatePosition(command);
        }
    }
}
