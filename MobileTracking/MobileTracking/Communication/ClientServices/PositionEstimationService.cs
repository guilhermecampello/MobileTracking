using MobileTracking.Core.Commands;
using MobileTracking.Core.Interfaces;
using MobileTracking.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobileTracking.Communication.ClientServices
{
    class PositionEstimationService : IPositionEstimationService
    {
        private readonly Client client;

        private string positionEstimationsController = "position-estimation";

        public PositionEstimationService(Client client)
        {
            this.client = client;
        }

        public Task<PositionEstimation> EstimatePosition(EstimatePositionCommand command)
        {
            return client.Post<PositionEstimation>(positionEstimationsController, command);
        }
    }
}
