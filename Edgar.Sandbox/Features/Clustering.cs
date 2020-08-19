using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edgar.Legacy.Benchmarks;
using Edgar.Legacy.Benchmarks.AdditionalData;
using Edgar.Legacy.Benchmarks.GeneratorRunners;
using Edgar.Legacy.Benchmarks.Interfaces;
using Edgar.Legacy.Benchmarks.ResultSaving;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.MapDrawing;
using Edgar.Legacy.Utils.Statistics;
using Newtonsoft.Json;
using Sandbox.Utils;

namespace Sandbox.Features
{
    public class Clustering
    {
        public void Run()
        {
            var rectangularRoomTemplates = MapDescriptionUtils.GetRectangularRoomTemplates(new Vector2Int(1, 1));
            var basicRoomDescription = new BasicRoomDescription(rectangularRoomTemplates);

            var inputs = new List<DungeonGeneratorInput<int>>();
            inputs.AddRange(Program.GetMapDescriptionsSet(new Vector2Int(1, 1), false, null, true));
            inputs.AddRange(Program.GetMapDescriptionsSet(new Vector2Int(1, 1), false, null, true, basicRoomDescription: basicRoomDescription, suffix: "rect shapes"));
            inputs.AddRange(Program.GetMapDescriptionsSet(new Vector2Int(1, 1), true, new List<int>() { 2 }, false));

            inputs.Add(LoadInput("gungeon_1_1"));
            inputs.Add(LoadInput("gungeon_1_1", true));
            inputs.Add(LoadInput("gungeon_1_2"));
            inputs.Add(LoadInput("gungeon_1_2", true));
            inputs.Add(LoadInput("gungeon_2_1"));
            inputs.Add(LoadInput("gungeon_2_1", true));
            inputs.Add(LoadInput("gungeon_2_2"));
            inputs.Add(LoadInput("gungeon_2_2", true));

            foreach (var input in inputs)
            {
                var mapDescription = input.MapDescription;
                var graph = mapDescription.GetStageOneGraph();
                Console.WriteLine($"{input.Name} {graph.VerticesCount} {graph.Edges.Count()}");
            }

            if (true)
            {
                inputs.Sort((x1, x2) => string.Compare(x1.Name, x2.Name, StringComparison.Ordinal));
            }

            inputs = inputs.Where(x => !x.Name.StartsWith("Example 4")).ToList();

            var layoutDrawer = new SVGLayoutDrawer<int>();

            var benchmarkRunner = new BenchmarkRunner<IMapDescription<int>>();
            var benchmarkScenario = new BenchmarkScenario<IMapDescription<int>>("CorridorConfigurationSpaces", input =>
            {
                var dungeonGeneratorInput = (DungeonGeneratorInput<int>) input;
                var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, dungeonGeneratorInput.Configuration);
                layoutGenerator.InjectRandomGenerator(new Random(0));

                return new LambdaGeneratorRunner(() =>
                {
                    var simulatedAnnealingArgsContainer = new List<SimulatedAnnealingEventArgs>();
                    void SimulatedAnnealingEventHandler(object sender, SimulatedAnnealingEventArgs eventArgs)
                    {
                        simulatedAnnealingArgsContainer.Add(eventArgs);
                    }

                    layoutGenerator.OnSimulatedAnnealingEvent += SimulatedAnnealingEventHandler;
                    var layout = layoutGenerator.GenerateLayout();
                    layoutGenerator.OnSimulatedAnnealingEvent -= SimulatedAnnealingEventHandler;

                    var additionalData = new AdditionalRunData()
                    {
                        SimulatedAnnealingEventArgs = simulatedAnnealingArgsContainer,
                        GeneratedLayoutSvg = layoutDrawer.DrawLayout(layout, 800, forceSquare: true),
                        GeneratedLayout = layout,
                    };

                    var generatorRun = new GeneratorRun<AdditionalRunData>(layout != null, layoutGenerator.TimeTotal, layoutGenerator.IterationsCount, additionalData);

                    return generatorRun;
                });
            });

            var scenarioResult = benchmarkRunner.Run(benchmarkScenario, inputs, 100);

            // Clusters
            for (var i = 0; i < scenarioResult.BenchmarkResults.Count; i++)
            {
                var benchmarkResult = scenarioResult.BenchmarkResults[i];
                var layoutsClustering = new LayoutsClustering<int>();
                var input = inputs[i];
                var mapDescription = input.MapDescription;

                var layouts = benchmarkResult
                    .Runs
                    .Cast<IGeneratorRun<AdditionalRunData>>()
                    .Select(x => x.AdditionalData.GeneratedLayout)
                    .ToList();

                var layoutsForClustering = layouts.Take(Math.Min(layouts.Count, 500)).ToList();
                var layoutsMapping = layoutsForClustering.CreateIntMapping();


                var averageRoomTemplateSize = LayoutsDistance.GetAverageRoomTemplateSize(mapDescription);
                var positionOnlyClusters = layoutsClustering.GetClusters(layoutsMapping.Values.ToList(),
                    (x1, x2) => LayoutsDistance.PositionOnlyDistance(layoutsMapping.GetByValue(x1), layoutsMapping.GetByValue(x2)), averageRoomTemplateSize);
                var positionAndShapeClusters = layoutsClustering.GetClusters(layoutsMapping.Values.ToList(),
                    (x1, x2) => LayoutsDistance.PositionAndShapeDistance(layoutsMapping.GetByValue(x1), layoutsMapping.GetByValue(x2), averageRoomTemplateSize),
                    averageRoomTemplateSize);

                Console.WriteLine($"{input.Name} {positionOnlyClusters.Count}/{positionAndShapeClusters.Count}");

                benchmarkResult.AdditionalData["Clusters"] = new
                {
                    PositionOnlyClusters = positionOnlyClusters,
                    PositionAndShapeClusters = positionAndShapeClusters,
                };
            }

            // TODO: maybe create a factory method for cs generator?
            var configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());

            // Entropy
            for (var i = 0; i < scenarioResult.BenchmarkResults.Count; i++)
            {
                var benchmarkResult = scenarioResult.BenchmarkResults[i];
                var input = inputs[i];
                var mapDescription = input.MapDescription;

                var layouts = benchmarkResult
                    .Runs
                    .Cast<IGeneratorRun<AdditionalRunData>>()
                    .Select(x => x.AdditionalData.GeneratedLayout)
                    .ToList();

                var rooms = mapDescription.GetStageOneGraph().Vertices.ToList();
                var roomTemplateInstances = rooms
                    .SelectMany(x => mapDescription.GetRoomDescription(x).RoomTemplates)
                    .Distinct()
                    .SelectMany(x => configurationSpacesGenerator.GetRoomTemplateInstances(x))
                    .ToList();
                var roomTemplatesMapping = roomTemplateInstances.ToDictionary(x => x, x => x.Transformations.Contains(TransformationGrid2D.Identity) ? x.RoomTemplate.Name : $"{x.RoomTemplate.Name} {x.Transformations[0]}");
                var entropyCalculator = new EntropyCalculator();


                benchmarkResult.AdditionalData["Entropy"] = new
                {
                    RoomTemplates = roomTemplatesMapping.Values.Select(x => x.ToString()),
                    Entropy = entropyCalculator.ComputeAverageRoomTemplatesEntropy(mapDescription, layouts),
                    Distributions = rooms.Select(room =>
                    {
                        var distribution =
                            entropyCalculator.GetRoomTemplatesDistribution(mapDescription, layouts, room);

                        return new
                        {
                            Room = room.ToString(),
                            EntropyScore = entropyCalculator.ComputeEntropy(distribution, true),
                            Entropy = entropyCalculator.ComputeEntropy(distribution, false),
                            RoomTemplatesDistribution = roomTemplatesMapping.Keys.Select(x => distribution.ContainsKey(x) ? (double?) distribution[x] : null),
                        };
                    })
                };
            }

            var resultSaver = new BenchmarkResultSaver();
            resultSaver.SaveResultDefaultLocation(scenarioResult);

            var directory = $"CorridorConfigurationSpaces/{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}";
            Directory.CreateDirectory(directory);

            //var dataVisualization = new ChainStatsVisualization<GeneratorData>();
            //foreach (var inputResult in scenarioResult.BenchmarkResults)
            //{
            //    using (var file = new StreamWriter($"{directory}/{inputResult.InputName}.txt"))
            //    {
            //        var generatorEvaluation = new GeneratorEvaluation<AdditionalRunData<int>>(inputResult.Runs.Cast<IGeneratorRun<AdditionalRunData<int>>>().ToList()); // TODO: ugly
            //        dataVisualization.Visualize(generatorEvaluation, file);
            //    }
            //}

            Utils.BenchmarkUtils.IsEqualToReference(scenarioResult, "BenchmarkResults/1581884301_CorridorConfigurationSpaces_Reference.json");
        }

        private DungeonGeneratorInput<int> LoadInput(string name, bool transform = false)
        {
            var mapDescription = LoadMapDescription(name);

            if (transform)
            {
                var mapDescriptionNew = new MapDescription<int>();
                var roomTemplates = mapDescription.GetStageOneGraph().Vertices
                    .SelectMany(x => mapDescription.GetRoomDescription(x).RoomTemplates).Distinct();
                var newRoomDescription = new BasicRoomDescription(roomTemplates.ToList());

                foreach (var vertex in mapDescription.GetGraph().Vertices)
                {
                    var roomDescription = mapDescription.GetRoomDescription(vertex);

                    if (roomDescription is CorridorRoomDescription)
                    {
                        mapDescriptionNew.AddRoom(vertex, roomDescription);
                    }
                    else
                    {
                        mapDescriptionNew.AddRoom(vertex, newRoomDescription);
                    }
                }

                foreach (var edge in mapDescription.GetGraph().Edges)
                {
                    mapDescriptionNew.AddConnection(edge.From, edge.To);
                }

                mapDescription = mapDescriptionNew;
                name += "_transformed";
            }

            return new DungeonGeneratorInput<int>(name, mapDescription, new DungeonGeneratorConfiguration<int>()
            {
                RepeatModeOverride = RoomTemplateRepeatMode.NoRepeat,
            }, null);
        }

        private MapDescription<int> LoadMapDescription(string name)
        {
            var settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
            };

            var input = new GeneratorInput<MapDescription<int>>(
                "EnterTheGungeon",
                JsonConvert.DeserializeObject<MapDescription<int>>(File.ReadAllText($"Resources/MapDescriptions/{name}.json"), settings)
            );

            return input.MapDescription;
        }

        private void RunOld()
        {
            var input = Program.GetMapDescriptionsSet(1 * new Vector2Int(1, 1), false, new List<int>() { 2, 4 }, true)[1];
            var mapDescription = input.MapDescription;
            var averageSize = GetAverageRoomTemplateSize(mapDescription);

            Console.WriteLine($"average size {averageSize}");

            var layoutGenerator = new DungeonGenerator<int>(input.MapDescription, input.Configuration);
            layoutGenerator.InjectRandomGenerator(new Random(0));

            var layouts = new List<MapLayout<int>>();

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

        private void SaveGraphviz(List<MapLayout<int>> layouts, double minClusteringDistance)
        {
            var directory = $"Clusters/{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}/";
            Directory.CreateDirectory(directory);

            var layoutClustering = new LayoutsClustering<MapLayout<int>>();
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

        private double GetDistance(MapLayout<int> layout1, MapLayout<int> layout2)
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
                var shape1Center = nodeToRoom1[node].BoundingRectangle.Center - new Vector2Int(minX1, minY1);
                var shape2Center = nodeToRoom2[node].BoundingRectangle.Center - new Vector2Int(minX2, minY2);

                distances.Add(Vector2Int.ManhattanDistance(shape1Center, shape2Center) + (nodeToRoom1[node].Equals(nodeToRoom2[node]) ? 0 : 15));
            }

            return distances.Average();
        }
    }
}