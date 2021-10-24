using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;

namespace MobileTracking.Communication.Services
{
    class PositionSignalDatasService : IPositionSignalDataService
    {
        private readonly Client client;

        private string positionSignalDatasController = "position-datas";

        public PositionSignalDatasService(Client client)
        {
            this.client = client;
        }

        public Task<List<PositionSignalData>> GetPositionSignalDatas(PositionSignalDataQuery query)
        {
            return client.Get<List<PositionSignalData>>(positionSignalDatasController, query);
        }

        public Task<bool> RecalculatePositionSignalData(PositionSignalDataQuery query)
        {
            return client.Put<bool>(positionSignalDatasController, "recalculate", null, query);
        }
    }
}
