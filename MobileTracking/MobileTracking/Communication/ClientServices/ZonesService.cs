using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobileTracking.Communication.ClientServices
{
    public class ZonesService : IZoneService
    {
        private readonly Client client;

        private string zonesController = "zones";

        public ZonesService(Client client)
        {
            this.client = client;
        }

        public async Task<Zone> CreateZone(CreateOrUpdateZoneCommand command)
        {
            return await this.client.Post<Zone>(zonesController, command);
        }

        public async Task<bool> DeleteZone(int zoneId)
        {
            return await this.client.Delete<bool>(zonesController, zoneId.ToString());
        }

        public Task<Zone> FindZoneById(int zoneId, ZoneQuery query)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Zone>> GetZones(ZoneQuery query)
        {
            return await this.client.Get<List<Zone>>(zonesController, query);
        }

        public Task<Zone> UpdateZone(int zoneId, CreateOrUpdateZoneCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
