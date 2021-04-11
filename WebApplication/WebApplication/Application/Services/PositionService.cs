using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Infrastructure;
using WebApplication.Models;

namespace WebApplication.Application
{
    public class PositionService : IPositionService
    {
        private readonly DatabaseContext databaseContext;

        public PositionService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<Position> FindPositionById(int positionId, PositionQuery? query = null)
        {
            if (query == null)
            {
                query = new PositionQuery();
            }

            return await this.databaseContext.Positions
                .Include(query.IncludeCalibrations, positions => positions.Calibrations!)
                .Include(query.IncludeData, position => position.PositionData!)
                .FirstOrDefaultAsync(position => position.Id == positionId)
                ?? throw NotFoundException<Position>.ById(positionId);
        }

        public async Task<List<Position>> GetPositions(PositionQuery query)
        {
           return await this.databaseContext.Positions
                .AsNoTracking()
                .AsQueryable()
                .Where(query.LocaleId, localeId => position => position.Zone!.LocaleId == localeId)
                .Where(query.ZoneId, zoneId => position => position.ZoneId == zoneId)
                .Include(query.IncludeCalibrations, positions => positions.Calibrations!)
                .Include(query.IncludeData, position => position.PositionData!)
                .ToListAsync();
        }

        public async Task<Position> CreatePosition(CreateOrUpdatePositionCommand command)
        {
            var zone = await this.databaseContext.Locales.FindAsync(command.ZoneId);
            if (zone == null)
            {
                throw new InvalidParametersException("ZoneId", command.ZoneId, "An existing zoneId must be provided for position");
            }

            if (string.IsNullOrEmpty(command.Name))
            {
                throw new InvalidParametersException("Name", command.Name, "A name must be provided for position");
            }

            var position = new Position(command.ZoneId!.Value, command.Name, command.X ?? 0, command.Y ?? 0, command.Z ?? 0);
            this.databaseContext.Add(position);

            await this.databaseContext.SaveChangesAsync();

            return position;
        }

        public async Task<bool> DeletePosition(int positionId)
        {
            try
            {
                var position = this.FindPositionById(positionId);
                this.databaseContext.Remove(position);
                await this.databaseContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Position> UpdatePosition(int positionId, CreateOrUpdatePositionCommand command)
        {
            var position = await this.FindPositionById(positionId);
            position.ZoneId = command.ZoneId ?? position.ZoneId;
            position.Name = command.Name ?? position.Name;
            position.XCoordinate = command.X ?? position.XCoordinate;
            position.YCoordinate = command.X ?? position.YCoordinate;
            position.ZCoordinate = command.X ?? position.ZCoordinate;

            await this.databaseContext.SaveChangesAsync();
            return position;
        }
    }
}
