﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MobileTracking.Core.Models
{
    public class NeighbourPosition
    {
        public NeighbourPosition() { }

        public NeighbourPosition(Position position)
        {
            Position = position;
        }

        public Position Position { get; set; } = null!;

        public List<SignalScore> SignalScores { get; set; } = new List<SignalScore>();

        public double Score { get => SignalScores.Sum(signal => signal.Score); }

        public double Distance { get => Math.Max(SignalScores.Sum(signal => signal.Distance), 0.1); }
    }
}