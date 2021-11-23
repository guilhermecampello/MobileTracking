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

        public async Task<PositionEstimation> EstimatePosition(EstimatePositionCommand command)
        {
            var signals = command.Measurements.ToDictionary(signal => signal.SignalId);
            var positionSignalDatas = await this.databaseContext.PositionsSignalsData
                .Include(true, positionSignalData => positionSignalData.Position, position => position!.Zone)
                .Where(positionSignalData => positionSignalData.Position!.Zone!.LocaleId == command.LocaleId)
                .ToListAsync();

            var neighbourPositions = new List<NeighbourPosition>();

            if (command.UnmatchedSignalsWeight != null)
            {
                var unmatchedSignals = positionSignalDatas
                    .Where(positionSignalData => !signals.ContainsKey(positionSignalData.SignalId))
                    .ToList();
                unmatchedSignals.ForEach(unmatchedSignal =>
                {
                    var neighbourPosition = neighbourPositions
                    .FirstOrDefault(neighbourPosition => neighbourPosition.Position.Id == unmatchedSignal.PositionId);
                    if (neighbourPosition == null)
                    {
                        neighbourPosition = new NeighbourPosition(unmatchedSignal.Position!);
                        neighbourPositions.Add(neighbourPosition);
                    }

                    neighbourPosition.SignalScores.Add(new SignalScore(unmatchedSignal, command.UnmatchedSignalsWeight.Value));
                });
            }

            positionSignalDatas
                .Where(positionSignalData => signals.ContainsKey(positionSignalData.SignalId))
                .Where(positionSignalData => this.IsValidEstimation(positionSignalData, signals[positionSignalData.SignalId]))
                .ToList()
                .ForEach(data =>
            {
                data.LastSeen = DateTime.Now;
                var measurement = signals[data.SignalId];
                var neighbourPosition = neighbourPositions
                    .FirstOrDefault(neighbourPosition => neighbourPosition.Position.Id == data.PositionId);
                if (neighbourPosition == null)
                {
                    neighbourPosition = new NeighbourPosition(data.Position!);
                    neighbourPositions.Add(neighbourPosition);
                }

                neighbourPosition.SignalScores.Add(new SignalScore(data, measurement));
            });

            if (command.RealX != null && command.RealY != null)
            {
                this.databaseContext.Add(new UserLocalization()
                {
                    UserId = 1,
                    LocaleId = command.LocaleId,
                    DateTime = DateTime.Now,
                    RealX = command.RealX,
                    RealY = command.RealY,
                    LocalizationMeasurements = command.Measurements.Select(measurement => new LocalizationMeasurement(measurement)).ToList()
                });

                await this.databaseContext.SaveChangesAsync();
            }

            return new PositionEstimation(neighbourPositions.OrderByDescending(estimation => estimation.Score).Take(command.Neighbours).ToList());
        }

        private bool IsValidEstimation(PositionSignalData positionSignalData, Measurement measurement)
        {
            var magneticFieldTolerance = 0.5;
            var rssiTolerance = 20;
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
