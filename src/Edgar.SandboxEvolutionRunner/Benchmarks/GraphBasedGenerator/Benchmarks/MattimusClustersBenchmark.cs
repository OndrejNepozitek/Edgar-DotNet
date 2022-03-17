using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.Graphs;
using Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Generators;
using SandboxEvolutionRunner.Utils;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Benchmarks
{
    public class MattimusClustersBenchmark : Benchmark
    {
        private IGraph<Room> LoadGraph()
        {
            var filename = "MattimusGraph.asset";

            var rooms = new HashSet<Room>();
            var connections = new List<Edge<Room>>();

            var currentId = default(string);
            var isRoom = false;
            var zoneNumber = 0;
            var from = default(string);
            var to = default(string);
            foreach (var line in File.ReadLines(filename))
            {
                if (line.StartsWith("--- !u!114 &"))
                {
                    if (isRoom)
                    {
                        // Console.WriteLine($"{currentId} {zoneNumber}");
                        rooms.Add(new Room(currentId, zoneNumber));
                    }

                    var id = line.Replace("--- !u!114 &", "");

                    currentId = id;
                    isRoom = false;
                    from = null;
                    to = null;
                }

                if (line.StartsWith("  Zone: "))
                {
                    isRoom = true;
                    zoneNumber = int.Parse(line.Replace("  Zone: ", ""));
                }

                if (line.StartsWith("  From: {fileID: "))
                {
                    from = line.Replace("  From: {fileID: ", "");
                    from = from.Replace("}", "");
                }

                if (line.StartsWith("  To: {fileID: "))
                {
                    to = line.Replace("  To: {fileID: ", "");
                    to = to.Replace("}", "");
                }

                if (from != null && to != null)
                {
                    connections.Add(new Edge<Room>(new Room(from), new Room(to)));
                    from = null;
                    to = null;
                }
            }

            if (isRoom)
            {
                rooms.Add(new Room(currentId, zoneNumber));
            }

            var graph = new UndirectedAdjacencyListGraph<Room>();

            foreach (var room in rooms)
            {
                graph.AddVertex(room);
            }

            foreach (var connection in connections)
            {
                graph.AddEdge(connection.From, connection.To);
            }

            foreach (var zone in rooms.Select(x => x.Zone).Distinct().ToList())
            {
                var counter = 0;

                foreach (var room in rooms.Where(x => x.Zone == zone))
                {
                    room.DisplayName = $"{counter++}";
                }
            }

            return graph;
        }

        private BenchmarkScenario<Room> GetIndividualClustersScenario()
        {
            var fullGraph = LoadGraph();
            var namedGraphs = new List<NamedGraph<Room>>();
            var zones = fullGraph.Vertices.Select(x => x.Zone).Distinct().OrderBy(x => x).ToList();

            foreach (var zone in zones)
            {
                if (zone == 6)
                {
                    continue;
                }

                var graph = GraphAlgorithms.GetInducedSubgraph(fullGraph,
                    fullGraph.Vertices.Where(x => x.Zone == zone).ToHashSet(),
                    new UndirectedAdjacencyListGraph<Room>());
                var namedGraph = new NamedGraph<Room>(graph, $"Zone {zone}");
                namedGraphs.Add(namedGraph);
            }

            var levelDescriptionLoader = new MattimusLevelDescriptionLoader<Room>(RoomTemplatesSet.Medium,
                new Vector2Int(1, 1), getCorridorNameFunc: x => null);
            var levelDescriptions = levelDescriptionLoader.GetLevelDescriptions(namedGraphs, new List<int>() {0});

            return new BenchmarkScenario<Room>($"Individual zones", levelDescriptions);
        }

        private BenchmarkScenario<Room> GetFullScenario()
        {
            var namedGraphs = new List<NamedGraph<Room>>();
            var fullGraph = LoadGraph();
            var zones = fullGraph.Vertices.Select(x => x.Zone).Distinct().OrderBy(x => x).ToList();
            var neighborsLists = new List<List<int>>()
            {
                //new List<int>() {0},
                //new List<int>() {0, 3},
                //new List<int>() {3, 2},
                new List<int>() {0, 3, 2},
                new List<int>() {3, 0, 2},
                new List<int>() {3, 0, 2, 4},
                new List<int>() {3, 0, 2, 4, 1},
                new List<int>() {3, 0, 2, 4, 1, 5},
                new List<int>() {3, 2, 0, 4, 1},
                new List<int>() {3, 2, 0, 4, 1, 5},
            };

            foreach (var neighbors in neighborsLists)
            {
                var graph = GraphAlgorithms.GetInducedSubgraph(fullGraph,
                    fullGraph.Vertices.Where(x => neighbors.Contains(x.Zone)).ToHashSet(),
                    new UndirectedAdjacencyListGraph<Room>());
                var namedGraph = new NamedGraph<Room>(graph, string.Join(", ", neighbors));
                namedGraphs.Add(namedGraph);
            }

            var levelDescriptionLoader = new MattimusLevelDescriptionLoader<Room>(RoomTemplatesSet.Medium,
                new Vector2Int(1, 1), getCorridorNameFunc: x => null);
            var levelDescriptions = levelDescriptionLoader.GetLevelDescriptions(namedGraphs, new List<int>() {0});

            for (var i = 0; i < levelDescriptions.Count; i++)
            {
                var neighbors = neighborsLists[i];
                var levelDescription = levelDescriptions[i];
                var graph = levelDescription.GetGraphWithoutCorridors();
                var clusters = new Dictionary<int, List<Room>>();

                foreach (var vertex in graph.Vertices)
                {
                    var zone = vertex.Zone;

                    if (zone == 6)
                    {
                        zone = 5;
                    }

                    if (clusters.ContainsKey(zone) == false)
                    {
                        clusters[zone] = new List<Room>();
                    }

                    clusters[zone].Add(vertex);
                }

                levelDescription.Clusters = neighbors.Select(x => clusters[x]).ToList();
            }

            return new BenchmarkScenario<Room>($"Full graph", levelDescriptions);
        }

        protected RecursiveGeneratorFactory<TNode> GetRecursiveGenerator<TNode>(BenchmarkOptions options,
            bool withInit = false, bool optimizeCorridorConstraints = false, string name = null)
        {
            return new RecursiveGeneratorFactory<TNode>(new GraphBasedGeneratorConfiguration<TNode>()
            {
                EarlyStopIfTimeExceeded = options.EarlyStopTime != null
                    ? TimeSpan.FromMilliseconds(options.EarlyStopTime.Value)
                    : default(TimeSpan?),
                OptimizeCorridorConstraints = optimizeCorridorConstraints,
            }, withInit, name);
        }

        protected override void Run()
        {
            var options = new BenchmarkOptions()
            {
                EarlyStopTime = 60000,
            };

            var scenarios = new List<BenchmarkScenario<Room>>()
            {
                GetFullScenario(),
                //GetIndividualClustersScenario(),
            };

            var generators = new List<ILevelGeneratorFactory<Room>>()
            {
                GetRecursiveGenerator<Room>(options, name: "Recursive generator"),
                GetNewGenerator<Room>(options, name: "Default generator"),
            };

            RunBenchmark(scenarios, generators);
        }

        public class Room
        {
            public string Id { get; }

            public int Zone { get; }

            public string DisplayName { get; set; }

            public Room(string id, int zone = -1)
            {
                Id = id;
                Zone = zone;
            }

            public override string ToString()
            {
                return $"{Zone}_{DisplayName}";
            }

            protected bool Equals(Room other)
            {
                return Id == other.Id;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Room) obj);
            }

            public override int GetHashCode()
            {
                return (Id != null ? Id.GetHashCode() : 0);
            }

            public static bool operator ==(Room left, Room right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Room left, Room right)
            {
                return !Equals(left, right);
            }
        }
    }
}