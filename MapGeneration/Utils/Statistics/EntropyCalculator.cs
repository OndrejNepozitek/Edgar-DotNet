using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.MapLayouts;

namespace MapGeneration.Utils.Statistics
{
    public class EntropyCalculator
    {
        public double ComputeAverageRoomTemplatesEntropy<TNode>(IMapDescription<TNode> mapDescription, List<IMapLayout<TNode>> layouts, bool normalize = true)
        {
            return mapDescription
                .GetGraph()
                .Vertices
                .Where(x => mapDescription.GetRoomDescription(x).RoomTemplates.Count > 1)
                .Select(x => ComputeRoomTemplatesEntropy(mapDescription, layouts, x, normalize))
                .Average();
        }

        public double ComputeRoomTemplatesEntropy<TNode>(IMapDescription<TNode> mapDescription, List<IMapLayout<TNode>> layouts, TNode node, bool normalize = true)
        {
            var roomDescription = mapDescription.GetRoomDescription(node);
            var availableRoomTemplates = roomDescription.RoomTemplates;
            var data = layouts
                .Select(x => x.Rooms.Single(y => y.Node.Equals(node)))
                .Select(x => x.RoomTemplate)
                .ToList();

            var distribution = GetRoomTemplatesDistribution(data, availableRoomTemplates);
            var entropy = ComputeEntropy(distribution, normalize);

            return entropy;
        }

        public Dictionary<IRoomTemplate, double> GetRoomTemplatesDistribution(List<IRoomTemplate> data, List<IRoomTemplate> availableRoomTemplates)
        {
            var counts = new Dictionary<IRoomTemplate, int>();

            foreach (var roomTemplate in availableRoomTemplates)
            {
                counts.Add(roomTemplate, 0);
            }

            foreach (var roomTemplate in data)
            {
                counts[roomTemplate] += 1;
            }

            return counts
                .Keys
                .ToDictionary(x => x, x => (double) counts[x] / data.Count);
        }

        public double ComputeEntropy<TElement>(Dictionary<TElement, double> distribution, bool normalize = false)
        {
            if (distribution == null) throw new ArgumentNullException(nameof(distribution));

            if (distribution.Count == 0)
            {
                throw new ArgumentException("Distribution must not be empty", nameof(distribution));
            }

            var entropy = -1 * 
                distribution
                .Values
                .Where(x => x != 0)
                .Sum(x => x * Math.Log(x, 2));

            if (normalize && distribution.Count != 1)
            {
                var maximumEntropy = Math.Log(distribution.Count, 2);
                var normalizedEntropy = entropy / maximumEntropy;

                return normalizedEntropy;
            }

            return entropy;
        }
    }
}