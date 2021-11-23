using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileTracking.Core.Models
{
    public class PositionEstimation
    {
        public PositionEstimation(List<NeighbourPosition> neighbourPositions)
        {
            NeighbourPositions = neighbourPositions.Where(position => position.Score > 0).ToList();
            var totalScore = NeighbourPositions.Sum(neighbour => neighbour.Score);
            neighbourPositions.ForEach(position =>
            {
                X += position.Position.X * position.Score;
                Y += position.Position.Y * position.Score;
            });
            X = X / totalScore;
            Y = Y / totalScore;
        }

        public double X { get; set; } = 0;

        public double Y { get; set; } = 0;

        public List<NeighbourPosition> NeighbourPositions { get; set; }
    }
}