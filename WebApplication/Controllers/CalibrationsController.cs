using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/calibrations")]
    public class CalibrationsController : ControllerBase
    {
        [HttpGet("{calibrationId}")]
        public async Task<ActionResult<Calibration>> GetCalibrationById(
            [FromServices] ICalibrationService calibrationService,
            [FromRoute] int calibrationId)
        {
            return await calibrationService.FindCalibrationById(calibrationId);
        }

        [HttpGet]
        public async Task<ActionResult<List<Calibration>>> GetCalibrations(
            [FromServices] ICalibrationService calibrationService,
            [FromQuery] CalibrationsQuery query)
        {
            return await calibrationService.GetCalibrations(query);
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteCalibrations(
            [FromServices] ICalibrationService calibrationService,
            [FromQuery] CalibrationsQuery query)
        {
            return await calibrationService.DeleteCalibrations(query);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateCalibrations(
            [FromServices] ICalibrationService calibrationService,
            [FromBody] CreateCalibrationsCommand command)
        {
            return await calibrationService.CreateCalibrations(command);
        }
    }
}
