﻿using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobileTracking.Communication.ClientServices
{
    class PositionsService : IPositionService
    {
        private readonly Client client;

        private string positionsController = "positions";

        public PositionsService(Client client)
        {
            this.client = client;
        }

        public Task<Position> CreatePosition(CreateOrUpdatePositionCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePosition(int positionId)
        {
            throw new NotImplementedException();
        }

        public Task<Position> FindPositionById(int zoneId, PositionQuery query)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Position>> GetPositions(PositionQuery query)
        {
            return await this.client.Get<List<Position>>(positionsController, query);
        }

        public Task<Position> UpdatePosition(int positionId, CreateOrUpdatePositionCommand command)
        {
            throw new NotImplementedException();
        }
    }
}