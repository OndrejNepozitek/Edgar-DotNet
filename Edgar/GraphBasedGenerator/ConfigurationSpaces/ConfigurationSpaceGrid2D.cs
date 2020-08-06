using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Core.Doors;
using MapGeneration.Utils;

namespace Edgar.GraphBasedGenerator.ConfigurationSpaces
{
    public class ConfigurationSpaceGrid2D : IConfigurationSpace<IntVector2>
    {
        private readonly List<OrthogonalLine> lines;

        public IReadOnlyList<OrthogonalLine> Lines { get; }

        private readonly List<Tuple<OrthogonalLine, DoorLine>> reverseDoors;

        public IReadOnlyList<Tuple<OrthogonalLine, DoorLine>> ReverseDoors { get; }

        public ConfigurationSpaceGrid2D(List<OrthogonalLine> lines, List<Tuple<OrthogonalLine, DoorLine>> reverseDoors = null)
        {
            this.lines = lines;
            this.reverseDoors = reverseDoors;
            Lines = lines.AsReadOnly();

            if (reverseDoors != null)
            {
                this.reverseDoors = reverseDoors;
                ReverseDoors = reverseDoors.AsReadOnly();
            }
        }

        public IntVector2 GetRandomPosition(Random random)
        {
            var line = lines.GetWeightedRandom(x => x.Length + 1, random);
            return line.GetNthPoint(random.Next(line.Length + 1));
        }

        public IEnumerable<IntVector2> ShuffleAndSamplePositions(int maxPointsPerLine, Random random)
        {
            var intersection = lines.ToList();
            intersection.Shuffle(random);

            foreach (var intersectionLine in intersection)
            {
                if (intersectionLine.Length > maxPointsPerLine)
                {
                    var mod = intersectionLine.Length / maxPointsPerLine - 1;

                    for (var i = 0; i < maxPointsPerLine; i++)
                    {
                        var position = intersectionLine.GetNthPoint(i != maxPointsPerLine - 1 ? i * mod : intersectionLine.Length + 1);
                        yield return position;
                    }
                }
                else
                {
                    var points = intersectionLine.GetPoints();
                    points.Shuffle(random);

                    foreach (var position in points)
                    {
                        yield return position;
                    }
                }
            }
        }
    }
}