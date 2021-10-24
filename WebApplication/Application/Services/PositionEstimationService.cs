using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MobileTracking.Core.Application;
using MobileTracking.Core.Commands;
using MobileTracking.Core.Interfaces;
using MobileTracking.Core.Models;
using WebApplication.Infrastructure;

namespace WebApplication.Application.Services
{
    public class PositionEstimationService : IPositionEstimationService
    {
        private readonly DatabaseContext databaseContext;

        public PositionEstimationService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<List<PositionEstimation>> EstimatePosition(EstimatePositionCommand command)
        {
            var signals = command.Measurements.ToDictionary(signal => signal.SignalId);
            var positionSignalDatas = await this.databaseContext.PositionsData
                .Include(true, positionSignalData => positionSignalData.Position, position => position!.Zone)
                .Where(positionSignalData => positionSignalData.Position!.Zone!.LocaleId == command.LocaleId)
                .Where(positionSignalData => signals.Keys.Contains(positionSignalData.SignalId))
                .ToListAsync();

            var positionEstimations = new List<PositionEstimation>();
            positionSignalDatas
                .Where(positionSignalData => this.IsValidEstimation(positionSignalData, signals[positionSignalData.SignalId]))
                .ToList()
                .ForEach(data =>
            {
                data.LastSeen = DateTime.Now;
                var measurement = signals[data.SignalId];
                var positionEstimation = positionEstimations
                    .FirstOrDefault(positionEstimation => positionEstimation.Position.Id == data.PositionId);
                if (positionEstimation == null)
                {
                    positionEstimation = new PositionEstimation(data.Position!);
                    positionEstimations.Add(positionEstimation);
                }

                positionEstimation.SignalScores.Add(new SignalScore(data, measurement));
            });

            _ = this.databaseContext.SaveChangesAsync();

            return positionEstimations.OrderByDescending(estimation => estimation.Score).Take(5).ToList();
        }

        private bool IsValidEstimation(PositionSignalData positionSignalData, Measurement measurement)
        {
            var magneticFieldTolerance = 0.5;
            var rssiTolerance = -20;
            return (positionSignalData.SignalType != SignalType.Magnetometer
                        && positionSignalData.Min - rssiTolerance <= measurement.Strength
                        && positionSignalData.Max + rssiTolerance >= measurement.Strength)
                   ||
                   (positionSignalData.SignalType == SignalType.Magnetometer
                        && positionSignalData.MinY - magneticFieldTolerance <= measurement.Y
                        && positionSignalData.MaxY + magneticFieldTolerance >= measurement.Y
                        && positionSignalData.MinZ - magneticFieldTolerance <= measurement.Z
                        && positionSignalData.MaxZ + magneticFieldTolerance >= measurement.Z);
        }
    }
}
