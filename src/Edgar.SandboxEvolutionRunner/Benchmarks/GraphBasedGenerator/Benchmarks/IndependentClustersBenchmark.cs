using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Drawing;
using Edgar.GraphBasedGenerator.RecursiveGrid2D;
using Edgar.Graphs;
using Edgar.Legacy.Utils;
using Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Generators;
using SandboxEvolutionRunner.Utils;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Benchmarks
{
    public class IndependentClustersBenchmark : Benchmark
    {
        private IGraph<int> GetPathGraph(int numberOfVertices)
        {
            var graph = new UndirectedAdjacencyListGraph<int>();

            for (int i = 0; i < numberOfVertices; i++)
            {
                graph.AddVertex(i);

                if (i > 0)
                {
                    graph.AddEdge(i -1, i);
                }
            }

            return graph;
        }

        private IGraph<int> GetRandomGraph(int numberOfVertices, int maxDegree, Random random, int nonTreeEdges)
        {
            var graph = new UndirectedAdjacencyListGraph<int>();
            graph.AddVertex(0);

            for (int i = 1; i < numberOfVertices; i++)
            {
                var validNeighbors = graph.Vertices.Where(x => graph.GetNeighbors(x).Count() < maxDegree).ToList();
                var neighbor = validNeighbors.GetRandom(random);
                var newVertex = graph.VerticesCount;

                graph.AddVertex(newVertex);
                graph.AddEdge(neighbor, newVertex);
            }

            var vertices = graph.Vertices.ToList();
            for (int i = 0; i < nonTreeEdges; i++)
            {
                while (true)
                {
                    var vertex1 = vertices.GetRandom(random);
                    var vertex2 = vertices.GetRandom(random);

                    if (vertex1 != vertex2 && graph.HasEdge(vertex1, vertex2) == false)
                    {
                        graph.AddEdge(vertex1, vertex2);
                        break;
                    }
                }
            }

            return graph;
        }

        private NamedGraph<string> GetGraph(int numberOfClusters, int verticesPerCluster, Random random, int maxDegree = 4, int nonTreeEdges = 0)
        {
            var graph = new UndirectedAdjacencyListGraph<string>();
            var highLevelStructure = GetPathGraph(numberOfClusters);
            var clusters =
                highLevelStructure.Vertices.ToDictionary(x => x,
                    x => GetRandomGraph(verticesPerCluster, maxDegree, random, nonTreeEdges));
            // TODO:
            //var clusters =
            //    highLevelStructure.Vertices.ToDictionary(x => x,
            //        x => GetPathGraph(verticesPerCluster));

            foreach (var pair in clusters)
            {
                var clusterNumber = pair.Key;
                var cluster = pair.Value;

                foreach (var vertex in cluster.Vertices)
                {
                    graph.AddVertex(GetVertexAlias(clusterNumber, vertex));
                }

                foreach (var edge in cluster.Edges)
                {
                    graph.AddEdge(GetVertexAlias(clusterNumber, edge.From), GetVertexAlias(clusterNumber, edge.To));
                }
            }

            foreach (var edge in highLevelStructure.Edges)
            {
                var cluster1 = clusters[edge.From];
                var cluster2 = clusters[edge.To];

                //var vertex1 = cluster1.Vertices.ToList().GetRandom(random);
                //var vertex2 = cluster2.Vertices.ToList().GetRandom(random);
                // TODO:
                var vertex1 = cluster1.Vertices.Last();
                var vertex2 = cluster2.Vertices.First();

                graph.AddEdge(GetVertexAlias(edge.From, vertex1), GetVertexAlias(edge.To, vertex2));
            }

            return new NamedGraph<string>(graph, $"{numberOfClusters} {verticesPerCluster} ({graph.VerticesCount} verts, {nonTreeEdges} nte)");
        }

        private string GetVertexAlias(int clusterNumber, int vertex)
        {
            return $"{clusterNumber}_{vertex}";
        }

        private BenchmarkScenario<string> GetScenario(int numberOfClusters, int verticesPerCluster, int nonTreeEdges)
        {
            var namedGraphs = new List<NamedGraph<string>>();

            for (int i = 0; i < numberOfClusters; i++)
            {
                namedGraphs.Add(GetGraph(i + 1, verticesPerCluster, new Random(0), 4, nonTreeEdges));
            }

            var levelDescriptionLoader = new LevelDescriptionLoader<string>(RoomTemplatesSet.Medium, new Vector2Int(1, 1), getCorridorNameFunc: x => $"c_{x}");
            var levelDescriptions = levelDescriptionLoader.GetLevelDescriptions(namedGraphs, new List<int>() { 0 });

            foreach (var levelDescription in levelDescriptions)
            {
                var graph = levelDescription.GetGraphWithoutCorridors();
                var clusters = new Dictionary<string, List<string>>();

                foreach (var vertex in graph.Vertices)
                {
                    var prefix = vertex.Split('_')[0];

                    if (clusters.ContainsKey(prefix) == false)
                    {
                        clusters[prefix] = new List<string>();
                    }

                    clusters[prefix].Add(vertex);
                }

                levelDescription.Clusters = clusters.Select(x => x.Value).ToList();
            }

            return new BenchmarkScenario<string>($"{numberOfClusters} {verticesPerCluster} {nonTreeEdges}", levelDescriptions);
        }

        protected RecursiveGeneratorFactory<TNode> GetRecursiveGenerator<TNode>(BenchmarkOptions options, bool withInit = false, bool optimizeCorridorConstraints = false, string name = null)
        {
            return new RecursiveGeneratorFactory<TNode>(new GraphBasedGeneratorConfiguration<TNode>()
            {
                EarlyStopIfTimeExceeded = options.EarlyStopTime != null ? TimeSpan.FromMilliseconds(options.EarlyStopTime.Value) : default(TimeSpan?),
                OptimizeCorridorConstraints = optimizeCorridorConstraints,
            }, withInit, name);
        }

        protected override void Run()
        {
            var options = new BenchmarkOptions()
            {
                EarlyStopTime = 50000,
            };

            var scenarios = new List<BenchmarkScenario<string>>()
            {
                // GetScenario(5, 15, 0),
                GetScenario(5, 15, 1),
            };

            //foreach (var benchmarkScenario in scenarios)
            //{
            //    foreach (var levelDescription in benchmarkScenario.LevelDescriptions)
            //    {
            //        var generator = new GraphBasedGeneratorGrid2D<string>(levelDescription);
            //        var layout = generator.GenerateLayout();
            //        var dungeonDrawer = new DungeonDrawer<string>();
            //        dungeonDrawer.DrawLayoutAndSave(layout, $"{levelDescription.Name}.png", new DungeonDrawerOptions()
            //        {
            //            Width = 1000,
            //            Height = 1000,
            //        });
            //    }
            //}

            var generators = new List<ILevelGeneratorFactory<string>>()
            {
                // GetNewGenerator<string>(options, name: "Default generator"),
                GetRecursiveGenerator<string>(options, name: "Recursive generator"),
            };

            RunBenchmark(scenarios, generators);
        }
    }
}