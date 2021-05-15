using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using WebApplication.Infrastructure;

namespace WebApplication.Application.Services
{
    public class PositionDataService : IPositionDataService
    {
        private readonly DatabaseContext databaseContext;

        public PositionDataService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public Task<List<PositionData>> GetPositionDatas(PositionDataQuery query)
        {
            return this.databaseContext.PositionsData
                .AsNoTracking()
                .AsQueryable()
                .Where(query.LocaleId, localeId => data => data.Position!.Zone!.LocaleId == localeId)
                .Where(query.ZoneId, zoneId => data => data.Position!.ZoneId == zoneId)
                .Where(query.PositionId, positionId => data => data.PositionId == positionId)
                .ToListAsync();
        }

        public async Task<bool> RecalculatePositionData(PositionDataQuery query)
        {
            var positions = await this.databaseContext.Positions
                .AsQueryable()
                .Where(query.LocaleId, localeId => position => position.Zone!.LocaleId == localeId)
                .Where(query.ZoneId, zoneId => position => position.ZoneId == zoneId)
                .Where(query.PositionId, positionId => position => position.Id == positionId)
                .Where(query.NeedsUpdate, needsUpdate => position => position.DataNeedsUpdate == needsUpdate)
                .Include(position => position.Calibrations)
                .ToListAsync();

            positions.ForEach(position =>
            {
                this.RemoveOldData(position.Id);
                var positionDatas = position.Calibrations!
                .GroupBy(calibration => new { calibration.SignalId, calibration.SignalType })
                .Select(calibration => new PositionData()
                {
                    PositionId = position.Id,
                    SignalId = calibration.Key.SignalId,
                    SignalType = calibration.Key.SignalType,
                    Samples = calibration.Count(),
                    Strength = calibration.Average(calibration => calibration.Strength),
                    X = calibration.Average(calibration => calibration.X),
                    Y = calibration.Average(calibration => calibration.Y),
                    Z = calibration.Average(calibration => calibration.Z),
                    Min = calibration.Min(calibration => calibration.Strength),
                    Max = calibration.Max(calibration => calibration.Strength),
                    MinX = calibration.Min(calibration => calibration.X),
                    MaxX = calibration.Max(calibration => calibration.X),
                    MinY = calibration.Min(calibration => calibration.Y),
                    MaxY = calibration.Max(calibration => calibration.Y),
                    MinZ = calibration.Min(calibration => calibration.Z),
                    MaxZ = calibration.Max(calibration => calibration.Z)
                })
                .ToList();

                positionDatas.ForEach(data =>
                {
                    var calibrations = position.Calibrations!
                    .Where(calibration => calibration.SignalId == data.SignalId
                        && calibration.SignalType == calibration.SignalType)
                    .ToList();

                    data.CalculateStandardDeviation(calibrations);

                    data.LastUpdate = DateTime.Now;
                    this.databaseContext.Add(data);
                });

                position.DataNeedsUpdate = false;
            });

            await this.databaseContext.SaveChangesAsync();

            return true;
        }

        private void RemoveOldData(int positionId)
        {
            var data = this.databaseContext.PositionsData
                .AsQueryable()
                .Where(positionData =>
                    positionData.PositionId == positionId)
                .ToList();

            this.databaseContext.PositionsData.RemoveRange(data);
        }
    }
}
