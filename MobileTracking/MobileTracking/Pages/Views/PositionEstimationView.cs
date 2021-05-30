﻿using MobileTracking.Core.Models;
using System.Drawing;
using System.Linq;

namespace MobileTracking.Pages.Views
{
    public class PositionEstimationView
    {
        private readonly PositionEstimation positionEstimation;

        public PositionEstimationView(PositionEstimation positionEstimation)
        {
            this.positionEstimation = positionEstimation;
        }

        public PositionEstimation PositionEstimation { get => positionEstimation; }

        public string Name
        {
            get => $"{positionEstimation.Position!.Zone!.Name}-{positionEstimation.Position.Name}";
        }

        public string Score { get => positionEstimation.Score.ToString("0.00"); }

        public string SignalScoresDescription
        {
            get
            {
                var description = "";
                positionEstimation.SignalScores.OrderByDescending(signal => signal.Score)
                    .ToList()
                    .ForEach(score =>
                    {
                        if (score.Measurement.SignalType == SignalType.Magnetometer)
                        {
                            description += $"{AppResources.Magnetic_field}: " +
                            $"Y:{score.PositionData.Y.ToString("0.00")} " +
                            $"Z:{score.Measurement.Z.ToString("0.00")} \n" +
                            $"SCORE: {score.Score.ToString("0.00")}\n";
                        }
                        else
                        {
                            description += $"{score.Measurement.SignalId}: " +
                            $"{score.PositionData.Strength.ToString("0.00")} \n" +
                            $"SCORE: {score.Score.ToString("0.00")} \n";
                        }
                    });
                return description;
            }
        }
    }
}
