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

        private List<PositionSignalData>? cachedPositionSignalData;

        public PositionEstimationService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<PositionEstimation> EstimatePosition(EstimatePositionCommand command)
        {
            if (command.UseBestParameters)
            {
                var localeParameters = await this.databaseContext.LocaleParameters
                    .OrderBy(parameters => parameters.Missings)
                    .ThenBy(parameter => parameter.MeanError)
                    .FirstOrDefaultAsync(parameter => parameter.LocaleId == command.LocaleId && parameter.IsActive);

                if (localeParameters == null)
                {
                    localeParameters = await this.databaseContext.LocaleParameters
                    .OrderBy(parameters => parameters.Missings)
                    .ThenBy(parameter => parameter.MeanError)
                    .FirstOrDefaultAsync(parameter => parameter.LocaleId == command.LocaleId);
                }

                if (localeParameters != null)
                {
                    command.BleWeight = localeParameters.BleWeight;
                    command.MagnetometerWeight = localeParameters.MagnetometerWeight;
                    command.Neighbours = localeParameters.Neighbours;
                    command.UnmatchedSignalsWeight = localeParameters.UnmatchedSignalsWeight;
                    command.WifiWeight = localeParameters.WifiWeight;
                }
            }

            var signals = command.Measurements.ToDictionary(signal => signal.SignalId);
            var positionSignalDatas = this.cachedPositionSignalData ?? await this.databaseContext.PositionsSignalsData
                .AsNoTracking()
                .Include(true, positionSignalData => positionSignalData.Position, position => position!.Zone)
                .Where(positionSignalData => positionSignalData.Position!.Zone!.LocaleId == command.LocaleId)
                .Where(positionSignalData => positionSignalData.LastSeen.AddDays(30) > command.Measurements.First().DateTime &&
                    positionSignalData.LastSeen.AddDays(-30) < command.Measurements.First().DateTime)
                .ToListAsync();

            this.cachedPositionSignalData = positionSignalDatas;

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

                    neighbourPosition.SignalScores.Add(new SignalScore(unmatchedSignal, command.UnmatchedSignalsWeight.Value, command.StandardDeviationFactor));
                });
            }

            positionSignalDatas
                .Where(positionSignalData => signals.ContainsKey(positionSignalData.SignalId))
                .Where(positionSignalData => this.IsValidEstimation(positionSignalData, signals[positionSignalData.SignalId]))
                .ToList()
                .ForEach(data =>
            {
                var measurement = signals[data.SignalId];
                var neighbourPosition = neighbourPositions
                    .FirstOrDefault(neighbourPosition => neighbourPosition.Position.Id == data.PositionId);
                if (neighbourPosition == null)
                {
                    neighbourPosition = new NeighbourPosition(data.Position!);
                    neighbourPositions.Add(neighbourPosition);
                }

                neighbourPosition.SignalScores.Add(new SignalScore(data, measurement, command.StandardDeviationFactor));
            });

            if (command.RealX != null && command.RealY != null)
            {
                this.databaseContext.Add(new UserLocalization()
                {
                    UserId = 1,
                    LocaleId = command.LocaleId,
                    DateTime = DateTime.UtcNow,
                    RealX = command.RealX,
                    RealY = command.RealY,
                    LocalizationMeasurements = command.Measurements.Select(measurement => new LocalizationMeasurement(measurement)).ToList()
                });

                await this.databaseContext.SaveChangesAsync();
            }

            this.ApplyWeights(neighbourPositions, command);

            var nearestNeighbours = !command.UseDistance
                    ? neighbourPositions.OrderByDescending(estimation => estimation.Score).Take(command.Neighbours).ToList()
                    : neighbourPositions.OrderBy(estimation => estimation.Distance).Take(command.Neighbours).ToList();

            if (!command.UseDistance)
            {
                int i = 0;
                while (nearestNeighbours.Any(neighbour => neighbour.Score < 0) && i < 40)
                {
                    neighbourPositions.ForEach(neighbour =>
                    {
                        neighbour.SignalScores.ForEach(signalScore =>
                        {
                            if (signalScore.Score < 0)
                            {
                                signalScore.Score *= 0.8;
                            }
                        });
                    });
                    nearestNeighbours = neighbourPositions.OrderByDescending(estimation => estimation.Score).Take(command.Neighbours).ToList();
                    i++;
                }
            }

            return new PositionEstimation(nearestNeighbours, command.UseDistance);
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

        private void ApplyWeights(List<NeighbourPosition> neighbourPositions, EstimatePositionCommand estimatePositionCommand)
        {
            neighbourPositions.ForEach(neighbour =>
            {
                neighbour.SignalScores.ForEach(signalScore =>
                {
                    if (signalScore.PositionSignalData.SignalType == SignalType.Wifi)
                    {
                        signalScore.Score *= estimatePositionCommand.WifiWeight;
                        signalScore.Distance /= estimatePositionCommand.WifiWeight;
                    }
                    else if (signalScore.PositionSignalData.SignalType == SignalType.Bluetooth)
                    {
                        signalScore.Score *= estimatePositionCommand.BleWeight;
                        signalScore.Distance /= estimatePositionCommand.BleWeight;
                    }
                    else if (signalScore.PositionSignalData.SignalType == SignalType.Magnetometer)
                    {
                        signalScore.Score *= estimatePositionCommand.MagnetometerWeight;
                        signalScore.Distance /= estimatePositionCommand.MagnetometerWeight;
                    }
                });
            });
        }
    }
}
