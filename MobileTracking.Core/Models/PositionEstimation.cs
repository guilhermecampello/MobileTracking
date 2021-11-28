using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileTracking.Core.Models
{
    public class PositionEstimation
    {
        public PositionEstimation()
        {

        }

        public PositionEstimation(List<NeighbourPosition> neighbourPositions)
        {
            NeighbourPositions = neighbourPositions.Where(position => position.Score > 0).ToList();
            var zonesScores = NeighbourPositions.GroupBy(neighbourPosition => neighbourPosition.Position.ZoneId)
                .Select(group => new { ZoneId = group.Key, Weight = group.Count() })
                .ToList();

            if (NeighbourPositions.Count > 0)
            {
                var totalScore = NeighbourPositions.Sum(neighbour => neighbour.Score * zonesScores.FirstOrDefault(zone => zone.ZoneId == neighbour.Position.ZoneId).Weight);
                NeighbourPositions.ForEach(position =>
                {
                    X += position.Position.X * position.Score * zonesScores.FirstOrDefault(zone => zone.ZoneId == position.Position.ZoneId).Weight;
                    Y += position.Position.Y * position.Score * zonesScores.FirstOrDefault(zone => zone.ZoneId == position.Position.ZoneId).Weight;
                });
                X = X / totalScore;
                Y = Y / totalScore;
            }

            NeighbourPositions.ForEach(neighbour => {
                neighbour.Position.Calibrations = null;
                neighbour.Position.PositionSignalData = null;
                neighbour.Position.Zone.Positions = null;
                neighbour.Position.Zone.Locale = null;
            });
        }

        public double X { get; set; } = 0;

        public double Y { get; set; } = 0;

        public List<NeighbourPosition> NeighbourPositions { get; set; }
    }
}