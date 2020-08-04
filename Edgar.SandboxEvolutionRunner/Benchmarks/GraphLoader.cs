using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeneralAlgorithms.DataStructures.Graphs;
using SandboxEvolutionRunner.Utils;

namespace Edgar.SandboxEvolutionRunner.Benchmarks
{
    public static class GraphLoader
    {
        public static List<NamedGraph<int>> GetRandomGraphsVariety(int count)
        {
            var randomGraphSets = new List<List<NamedGraph<int>>>();

            for (int nonTreeEdges = 0; nonTreeEdges < 5; nonTreeEdges++)
            {
                for (int vertices = 10; vertices <= 40; vertices+=10)
                {
                    // We do not need that many graphs
                    randomGraphSets.Add(GetRandomGraphs(count, vertices, nonTreeEdges));
                }
            }

            var graphs = new List<NamedGraph<int>>();

            var counter = 0;
            var graphsPerSet = Math.Max(1, count / randomGraphSets.Count);
            var everyNth = graphsPerSet >= 10 ? 1 : 10 / graphsPerSet;

            while (graphs.Count < count)
            {
                foreach (var randomGraphSet in randomGraphSets)
                {
                    graphs.Add(randomGraphSet[counter * everyNth]);

                    if (graphs.Count >= count)
                    {
                        break;
                    }
                }

                counter++;
            }

            return graphs;
        }

        public static List<NamedGraph<int>> GetRandomGraphs(int count, int vertices, int nonTreeEdges)
        {
            var name = $"e_{nonTreeEdges}_{nonTreeEdges}_v_{vertices}_{vertices + 9}";
            var filename = $"Resources/RandomGraphs/{name}.txt";

            if (!File.Exists(filename))
            {
                throw new ArgumentException("Incorrect combination of vertices and edges!");
            }

            var lines = File.ReadAllLines(filename);
            var graphs = new List<NamedGraph<int>>();

            var i = 0;
            var lineCounter = 0;
            while (graphs.Count < count)
            {
                if (lineCounter >= lines.Length)
                {
                    break;
                }

                var graph = new UndirectedAdjacencyListGraph<int>();

                // Add vertices
                var verticesCount = int.Parse(lines[lineCounter++]);
                for (var vertex = 0; vertex < verticesCount; vertex++)
                {
                    graph.AddVertex(vertex);
                }

                // Add edges
                while (true)
                {
                    var line = lines[lineCounter++];

                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }

                    var edge = line.Split(' ').Select(int.Parse).ToList();
                    graph.AddEdge(edge[0], edge[1]);
                }

                graphs.Add(new NamedGraph(graph, $"{name} {i}"));

                i++;
            }

            return graphs;
        }
    }
}