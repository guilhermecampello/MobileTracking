using System;
using System.Collections.Generic;
using System.Text;
using MobileTracking.Core.Commands;
using MobileTracking.Core.Models;
using System.Threading.Tasks;

namespace MobileTracking.Core.Interfaces
{
    public interface IPositionEstimationService
    {
        Task<List<PositionEstimation>> EstimatePosition(EstimatePositionCommand command);
    }
}
