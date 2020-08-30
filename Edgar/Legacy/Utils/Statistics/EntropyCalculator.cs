using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;

namespace Edgar.Legacy.Utils.Statistics
{
    public class EntropyCalculator
    {
        private readonly ConfigurationSpacesGenerator configurationSpacesGenerator;

        public EntropyCalculator()
        {
            configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());
        }

        public double ComputeAverageRoomTemplatesEntropy<TNode>(IMapDescription<TNode> mapDescription, List<MapLayout<TNode>> layouts, bool normalize = true)
        {
            return mapDescription
                .GetGraph()
                .Vertices
                .Where(x => mapDescription.GetRoomDescription(x).RoomTemplates.Count > 1)
                .Select(x => ComputeRoomTemplatesEntropy(mapDescription, layouts, x, normalize))
                .Average();
        }

        public double ComputeRoomTemplatesEntropy<TNode>(IMapDescription<TNode> mapDescription, List<MapLayout<TNode>> layouts, TNode node, bool normalize = true)
        {
            var distribution = GetRoomTemplatesDistribution(mapDescription, layouts, node);
            var entropy = ComputeEntropy(distribution, normalize);

            return entropy;
        }

        public Dictionary<RoomTemplateInstance, double> GetRoomTemplatesDistribution<TNode>(IMapDescription<TNode> mapDescription, List<MapLayout<TNode>> layouts, TNode node)
        {
            var roomDescription = mapDescription.GetRoomDescription(node);
            var availableRoomTemplateInstances = roomDescription.RoomTemplates.SelectMany(x => configurationSpacesGenerator.GetRoomTemplateInstances(x)).ToList();
            var data = layouts
                .Select(x => x.Rooms.Single(y => y.Node.Equals(node)))
                .Select(x => x.RoomTemplateInstance)
                .ToList();

            return GetProbabilityDistribution(data, availableRoomTemplateInstances);
        }

        public Dictionary<TElement, double> GetProbabilityDistribution<TElement>(List<TElement> data, List<TElement> allElements)
        {
            var counts = new Dictionary<TElement, int>();

            foreach (var element in allElements)
            {
                counts.Add(element, 0);
            }

            foreach (var element in data)
            {
                counts[element] += 1;
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