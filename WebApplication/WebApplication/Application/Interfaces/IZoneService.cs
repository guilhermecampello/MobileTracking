using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Application
{
    public interface IZoneService
    {
        Task<Zone> FindZoneById(int zoneId);

        Task<List<Zone>> GetZones(ZoneQuery query);

        Task<Zone> CreateZone(CreateOrUpdateZoneCommand command);

        Task<Zone> UpdateZone(int zoneId, CreateOrUpdateZoneCommand command);

        Task<bool> DeleteZone(int zoneId);
    }
}
