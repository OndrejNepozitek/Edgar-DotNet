using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Interfaces.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.MapLayouts;
using MapGeneration.Utils;
using MapGeneration.Utils.MapDrawing;
using MapGeneration.Utils.Statistics;

namespace Sandbox.Features
{
    public class Clustering
    {
        public void Run()
        {
            var input = CorridorConfigurationSpaces.GetMapDescriptionsSet(1 * new IntVector2(1, 1), false, new List<int>() { 2, 4 }, true)[1];
            var mapDescription = input.MapDescription;
            var averageSize = GetAverageRoomTemplateSize(mapDescription);

            Console.WriteLine($"average size {averageSize}");

            var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, input.Configuration, input.Offsets);
            layoutGenerator.InjectRandomGenerator(new Random(0));

            var layouts = new List<IMapLayout<int>>();

            while (layouts.Count < 5000)
            {
                layouts.Add(layoutGenerator.GenerateLayout());
            }

            Console.WriteLine("Layouts generated");

            SaveGraphviz(layouts, averageSize);
        }

        private double GetAverageRoomTemplateSize(IMapDescription<int> mapDescription)
        {
            var roomTemplates = mapDescription
                .GetGraph()
                .Vertices
                .SelectMany(x => mapDescription.GetRoomDescription(x).RoomTemplates)
                .Distinct()
                .ToList();

            var averageSize = roomTemplates
                .Select(x => x.Shape.BoundingRectangle.Width + x.Shape.BoundingRectangle.Height)
                .Average();

            return averageSize;
        }

        private void SaveGraphviz(List<IMapLayout<int>> layouts, double minClusteringDistance)
        {
            var directory = $"Clusters/{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}/";
            Directory.CreateDirectory(directory);

            var layoutClustering = new LayoutsClustering<IMapLayout<int>>();
            var clusters = layoutClustering.GetClusters(layouts, GetDistance, minClusteringDistance);

            var output = "";
            output += "graph G {\n";
            output += "  node[shape=point];\n";
            output += "  edge[color=invis,penwidth=0.25];\n";
            output += "  outputorder=\"edgesfirst\";\n";

            var distances = new double[layouts.Count][];
            var allDistances = new List<double>();
            for (int i = 0; i < layouts.Count - 1; i++)
            {
                distances[i] = new double[layouts.Count];

                for (int j = i + 1; j < layouts.Count; j++)
                {
                    var distance = GetDistance(layouts[i], layouts[j]);
                    distances[i][j] = distance;
                    allDistances.Add(distance);
                }
            }

            var averageDistance = allDistances.Average();
            var normalizationFactor = averageDistance;
            allDistances = allDistances.Select(x => x / normalizationFactor).ToList();

            // Normalize distances
            for (int i = 0; i < layouts.Count - 1; i++)
            {
                for (int j = i + 1; j < layouts.Count; j++)
                {
                    distances[i][j] /= normalizationFactor;
                }
            }

            averageDistance = allDistances.Average();
            var drawer = new SVGLayoutDrawer<int>();

            var layoutToIntMapping = layouts.CreateIntMapping();
            for (int i = 0; i < clusters.Count; i++)
            {
                var cluster = clusters[i];
                Directory.CreateDirectory(directory + i);

                for (var index = 0; index < cluster.Count; index++)
                {
                    var layout = cluster[index];
                    var svg = drawer.DrawLayout(layout, 800);
                    File.WriteAllText($"{directory}{i}/{index}.svg", svg);

                    if (i < 10)
                    {
                        output += $"  {layoutToIntMapping[layout]} [color=\"/paired10/{i + 1}\"]\n";
                    }
                }
            }

            output += "\n";

            allDistances.Sort();
            var smallest25 = allDistances[(int)(allDistances.Count * 0.25)];
            var smallest15 = allDistances[(int)(allDistances.Count * 0.15)];
            var smallest10 = allDistances[(int)(allDistances.Count * 0.10)];
            var smallest5 = allDistances[(int)(allDistances.Count * 0.05)];

            for (int i = 0; i < layouts.Count - 1; i++)
            {
                for (int j = i + 1; j < layouts.Count; j++)
                {
                    var distance = distances[i][j];

                    if (distance < smallest5)
                    {
                        output += $"  {i} -- {j} [len={distance:F},weight={100 * 1/distance:F},color=\"{(distance > averageDistance ? "black" : "black")}\"];\n";
                    }
                }
            }

            output += "}";

            File.WriteAllText($"{directory}graphviz.txt", output);
        }

        private double GetDistance(IMapLayout<int> layout1, IMapLayout<int> layout2)
        {
            var nodeToRoom1 = layout1.Rooms.ToDictionary(x => x.Node, x => x.Shape + x.Position);
            var nodeToRoom2 = layout2.Rooms.ToDictionary(x => x.Node, x => x.Shape + x.Position);
            var distances = new List<double>();

            var minX1 = nodeToRoom1.Values.Min(x => x.BoundingRectangle.A.X);
            var minY1 = nodeToRoom1.Values.Min(x => x.BoundingRectangle.A.Y);
            var minX2 = nodeToRoom2.Values.Min(x => x.BoundingRectangle.A.X);
            var minY2 = nodeToRoom2.Values.Min(x => x.BoundingRectangle.A.Y);

            foreach (var node in nodeToRoom1.Keys)
            {
                var shape1Center = nodeToRoom1[node].BoundingRectangle.Center - new IntVector2(minX1, minY1);
                var shape2Center = nodeToRoom2[node].BoundingRectangle.Center - new IntVector2(minX2, minY2);

                distances.Add(IntVector2.ManhattanDistance(shape1Center, shape2Center) + (nodeToRoom1[node].Equals(nodeToRoom2[node]) ? 0 : 15));
            }

            return distances.Average();
        }
    }
}