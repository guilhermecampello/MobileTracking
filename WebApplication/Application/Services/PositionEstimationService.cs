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
            var positionDatas = await this.databaseContext.PositionsData
                .Include(true, positionData => positionData.Position, position => position!.Zone)
                .Where(positionData => positionData.Position!.Zone!.LocaleId == command.LocaleId)
                .Where(positionData => signals.Keys.Contains(positionData.SignalId))
                .ToListAsync();

            var positionEstimations = new List<PositionEstimation>();
            positionDatas
                .Where(positionData => this.IsValidEstimation(positionData, signals[positionData.SignalId]))
                .ToList()
                .ForEach(data =>
            {
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

            return positionEstimations.OrderByDescending(estimation => estimation.Score).Take(5).ToList();
        }

        private bool IsValidEstimation(PositionData positionData, Measurement measurement)
        {
            var magneticFieldTolerance = 0.5;
            return (positionData.SignalType != SignalType.Magnetometer
                        && positionData.Min <= measurement.Strength
                        && positionData.Max >= measurement.Strength)
                   ||
                   (positionData.SignalType == SignalType.Magnetometer
                        && positionData.MinY - magneticFieldTolerance <= measurement.Y
                        && positionData.MaxY + magneticFieldTolerance >= measurement.Y
                        && positionData.MinZ - magneticFieldTolerance <= measurement.Z
                        && positionData.MaxZ + magneticFieldTolerance >= measurement.Z);
        }
    }
}
