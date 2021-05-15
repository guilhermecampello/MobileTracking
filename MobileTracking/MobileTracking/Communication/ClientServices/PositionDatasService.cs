using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;

namespace MobileTracking.Communication.Services
{
    class PositionDatasService : IPositionDataService
    {
        private readonly Client client;

        private string positionDatasController = "position-datas";

        public PositionDatasService(Client client)
        {
            this.client = client;
        }

        public Task<List<PositionData>> GetPositionDatas(PositionDataQuery query)
        {
            return client.Get<List<PositionData>>(positionDatasController, query);
        }

        public Task<bool> RecalculatePositionData(PositionDataQuery query)
        {
            return client.Put<bool>(positionDatasController, "recalculate", null, query);
        }
    }
}
