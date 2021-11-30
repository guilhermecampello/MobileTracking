﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using WebApplication.Infrastructure;

namespace WebApplication.Application.Services
{
    public class PositionSignalDataService : IPositionSignalDataService
    {
        private readonly DatabaseContext databaseContext;

        public PositionSignalDataService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public Task<List<PositionSignalData>> GetPositionSignalDatas(PositionSignalDataQuery query)
        {
            return this.databaseContext.PositionsSignalsData
                .AsNoTracking()
                .AsQueryable()
                .Where(query.LocaleId, localeId => data => data.Position!.Zone!.LocaleId == localeId)
                .Where(query.ZoneId, zoneId => data => data.Position!.ZoneId == zoneId)
                .Where(query.PositionId, positionId => data => data.PositionId == positionId)
                .Where(query.SignalId, signalId => data => data.SignalId == signalId)
                .Where(query.SignalType, signalType => data => data.SignalType == signalType)
                .Include(query.IncludePosition, data => data.Position)
                .Include(query.IncludeZone, data => data.Position!, position => position.Zone!)
                .ToListAsync();
        }

        public async Task<bool> RecalculatePositionSignalData(PositionSignalDataQuery query)
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
                var positionSignalDatas = position.Calibrations!
                .GroupBy(calibration => new { calibration.SignalId, calibration.SignalType })
                .Select(calibration => new PositionSignalData()
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

                positionSignalDatas.ForEach(data =>
                {
                    var calibrations = position.Calibrations!
                    .Where(calibration => calibration.SignalId == data.SignalId
                        && calibration.SignalType == data.SignalType)
                    .ToList();

                    data.CalculateStandardDeviation(calibrations);
                    data.LastSeen = calibrations.Max(calibration => calibration.DateTime);
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
            var data = this.databaseContext.PositionsSignalsData
                .AsQueryable()
                .Where(positionSignalData =>
                    positionSignalData.PositionId == positionId)
                .ToList();

            this.databaseContext.PositionsSignalsData.RemoveRange(data);
        }
    }
}