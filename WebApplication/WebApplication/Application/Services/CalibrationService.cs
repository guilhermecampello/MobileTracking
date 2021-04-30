﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using WebApplication.Infrastructure;

namespace MobileTracking.Core.Application.Services
{
    public class CalibrationService : ICalibrationService
    {
        private readonly DatabaseContext databaseContext;

        public CalibrationService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<int> CreateCalibrations(CreateCalibrationsCommand command)
        {
            var positionId = command.PositionId;
            var dateTime = DateTime.Now;

            var position = await this.databaseContext.Positions
                .Include(position => position.Calibrations)
                .FirstOrDefaultAsync(position => position.Id == positionId);

            if (position == null)
            {
                throw new InvalidParametersException("PositionId", command.PositionId, "An existing positionId must be provided");
            }

            int count = 0;
            command.Measurements.ForEach(measurement =>
            {
                var calibration = new Calibration(positionId, measurement);
                calibration.DateTime = dateTime;
                position.Calibrations!.Add(calibration);
                count++;
            });

            await this.databaseContext.SaveChangesAsync();

            return count;
        }

        public async Task<bool> DeleteCalibration(int calibrationId)
        {
            var calibration = await this.databaseContext.Calibrations.FindAsync(calibrationId);

            if (calibration == null)
            {
                return false;
            }

            this.databaseContext.Remove(calibration);
            await this.databaseContext.SaveChangesAsync();

            return true;
        }

        public async Task<Calibration> FindCalibrationById(int calibrationId)
        {
            return await this.databaseContext.Calibrations.FindAsync(calibrationId)
                ?? throw NotFoundException<Calibration>.ById(calibrationId);
        }

        public async Task<List<Calibration>> GetCalibrations(CalibrationsQuery query)
        {
            return await this.databaseContext.Calibrations
                .Where(query.LocaleId, localeId => calibration => calibration.Position!.Zone!.LocaleId == localeId)
                .Where(query.ZoneId, zoneId => calibration => calibration.Position!.ZoneId == zoneId)
                .Where(query.PositionId, positionId => calibration => calibration.PositionId == positionId)
                .Where(query.SignalType, signalType => calibration => calibration.SignalType == signalType)
                .Where(query.SignalId, signalId => calibration => calibration.SignalId == signalId)
                .ToListAsync();
        }
    }
}
