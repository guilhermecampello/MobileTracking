using System.Collections.Generic;
using System.Threading.Tasks;
using MobileTracking.Core.Models;

namespace MobileTracking.Core.Application
{
    public interface IZoneService
    {
        Task<Zone> FindZoneById(int zoneId, ZoneQuery query = null);

        Task<List<Zone>> GetZones(ZoneQuery query);

        Task<Zone> CreateZone(CreateOrUpdateZoneCommand command);

        Task<Zone> UpdateZone(int zoneId, CreateOrUpdateZoneCommand command);

        Task<bool> DeleteZone(int zoneId);
    }
}
