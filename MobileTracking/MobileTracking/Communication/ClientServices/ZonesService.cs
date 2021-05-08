using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobileTracking.Communication.ClientServices
{
    class ZonesService : IZoneService
    {
        private readonly Client client;

        private string zonesController = "zones";

        public ZonesService(Client client)
        {
            this.client = client;
        }

        public Task<Zone> CreateZone(CreateOrUpdateZoneCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteZone(int positionId)
        {
            throw new NotImplementedException();
        }

        public Task<Zone> FindZoneById(int zoneId, ZoneQuery query)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Zone>> GetZones(ZoneQuery query)
        {
            return await this.client.Get<List<Zone>>(zonesController, query);
        }

        public Task<Zone> UpdateZone(int positionId, CreateOrUpdateZoneCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
