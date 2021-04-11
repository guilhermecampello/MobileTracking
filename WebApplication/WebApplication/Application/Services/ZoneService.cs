using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Infrastructure;
using WebApplication.Models;

namespace WebApplication.Application
{
    public class ZoneService : IZoneService
    {
        private readonly DatabaseContext databaseContext;

        public ZoneService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<Zone> FindZoneById(int zoneId, ZoneQuery? query = null)
        {
            if (query == null)
            {
                query = new ZoneQuery();
            }

            return await this.databaseContext.Zones
                .Include(query.IncludePositions, zone => zone.Positions)
                .Include(query.IncludePositionsCalibrations, zone => zone.Positions!, positions => positions.Calibrations!)
                .Include(query.IncludePositionsData, zone => zone.Positions!, position => position.PositionData!)
                .FirstOrDefaultAsync(zone => zone.Id == zoneId)
                ?? throw NotFoundException<Zone>.ById(zoneId);
        }

        public async Task<List<Zone>> GetZones(ZoneQuery query)
        {
           return await this.databaseContext.Zones
                .AsNoTracking()
                .AsQueryable()
                .Where(query.LocaleId, localeId => zone => zone.LocaleId == localeId)
                .Where(query.Floor, floor => zone => zone.Floor == floor)
                .Include(query.IncludePositions, zone => zone.Positions)
                .Include(query.IncludePositionsCalibrations, zone => zone.Positions!, positions => positions.Calibrations!)
                .Include(query.IncludePositionsData, zone => zone.Positions!, position => position.PositionData!)
                .ToListAsync();
        }

        public async Task<Zone> CreateZone(CreateOrUpdateZoneCommand command)
        {
            var locale = await this.databaseContext.Locales.FindAsync(command.LocaleId);
            if (locale == null)
            {
                throw new InvalidParametersException("LocaleId", command.LocaleId, "An existing localeId must be provided for zone");
            }

            if (string.IsNullOrEmpty(command.Name))
            {
                throw new InvalidParametersException("Name", command.Name, "A name must be provided for zone");
            }

            var zone = new Zone(command.LocaleId!.Value, command.Name, command.Description, command.Floor ?? 0);
            this.databaseContext.Add(zone);

            await this.databaseContext.SaveChangesAsync();

            return zone;
        }

        public async Task<bool> DeleteZone(int zoneId)
        {
            try
            {
                var zone = this.FindZoneById(zoneId);
                this.databaseContext.Remove(zone);
                await this.databaseContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Zone> UpdateZone(int zoneId, CreateOrUpdateZoneCommand command)
        {
            var zone = await this.FindZoneById(zoneId);
            zone.LocaleId = command.LocaleId ?? zone.LocaleId;
            zone.Name = command.Name ?? zone.Name;
            zone.Description = command.Description ?? zone.Description;
            zone.Floor = command.Floor ?? zone.Floor;

            await this.databaseContext.SaveChangesAsync();
            return zone;
        }
    }
}
