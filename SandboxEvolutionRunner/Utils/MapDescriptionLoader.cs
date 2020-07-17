using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeneralAlgorithms.DataStructures.Graphs;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Utils;
using MapGeneration.Utils.ConfigParsing;
using Newtonsoft.Json;

namespace SandboxEvolutionRunner.Utils
{
    public class MapDescriptionLoader
    {
        protected readonly Options Options;
        private readonly ConfigLoader configLoader = new ConfigLoader();

        public MapDescriptionLoader(Options options)
        {
            Options = options;
        }

        public virtual List<NamedGraph> GetGraphs()
        {
            var allGraphs = new Dictionary<string, Tuple<string, IGraph<int>>>()
            {
                { "1", Tuple.Create("Example 1 (fig. 1)", GraphsDatabase.GetExample1()) },
                { "2", Tuple.Create("Example 2 (fig. 7 top)", GraphsDatabase.GetExample2()) },
                { "3", Tuple.Create("Example 3 (fig. 7 bottom)", GraphsDatabase.GetExample3()) },
                { "4", Tuple.Create("Example 4 (fig. 8)", GraphsDatabase.GetExample4()) },
                { "5", Tuple.Create("Example 5 (fig. 9)", GraphsDatabase.GetExample5()) },
            };

            var graphs = Options
                .Graphs
                .Select(x => new NamedGraph(allGraphs[x].Item2, allGraphs[x].Item1))
                .ToList();

            foreach (var graphSet in Options.GraphSets)
            {
                graphs.AddRange(GetGraphSet(graphSet, Options.GraphSetCount));
            }

            return graphs;
        }

        protected virtual List<NamedGraph> GetGraphSet(string name, int count)
        {
            var graphs = new List<NamedGraph>();

            for (int i = 0; i < count; i++)
            {
                var filename = $"Resources/RandomGraphs/{name}/{i}.txt";
                var lines = File.ReadAllLines(filename);

                var graph = new UndirectedAdjacencyListGraph<int>();

                // Add vertices
                var verticesCount = int.Parse(lines[0]);
                for (var vertex = 0; vertex < verticesCount; vertex++)
                {
                    graph.AddVertex(vertex);
                }

                // Add edges
                for (var j = 1; j < lines.Length; j++)
                {
                    var line = lines[j];
                    var edge = line.Split(' ').Select(int.Parse).ToList();
                    graph.AddEdge(edge[0], edge[1]);
                }

                graphs.Add(new NamedGraph(graph, $"{name} {i}"));
            }

            return graphs;
        }

        public virtual List<NamedMapDescription> GetMapDescriptions(List<NamedGraph> namedGraphs = null)
        {
            namedGraphs = namedGraphs ?? GetGraphs();
            var mapDescriptions = new List<NamedMapDescription>();

            foreach (var namedGraph in namedGraphs)
            {
                mapDescriptions.AddRange(GetMapDescriptions(namedGraph));
            }

            foreach (var mapDescriptionName in Options.MapDescriptions)
            {
                mapDescriptions.Add(GetMapDescription(mapDescriptionName));
            }

            return mapDescriptions;
        }

        protected virtual List<NamedMapDescription> GetMapDescriptions(NamedGraph namedGraph)
        {
            var mapDescriptions = new List<NamedMapDescription>();

            foreach (var corridorOffsets in GetCorridorOffsets())
            {
                mapDescriptions.AddRange(GetMapDescriptions(namedGraph, corridorOffsets));
            }

            return mapDescriptions;
        }

        protected virtual List<List<int>> GetCorridorOffsets()
        {
            return Options.CorridorOffsets.Select(x => x.Split(",").Select(int.Parse).ToList()).ToList();
        }

        protected virtual List<NamedMapDescription> GetMapDescriptions(NamedGraph namedGraph, List<int> corridorOffsets)
        {
            var withCorridors = corridorOffsets[0] != 0;
            var canTouch = Options.CanTouch || !withCorridors;
            var basicRoomDescription = GetBasicRoomDescription();
            var corridorRoomDescription = withCorridors ? GetCorridorRoomDescription(corridorOffsets) : null;
            var mapDescription = MapDescriptionUtils.GetBasicMapDescription(namedGraph.Graph, basicRoomDescription, corridorRoomDescription, withCorridors);
            var name = MapDescriptionUtils.GetInputName(namedGraph.Name, Options.Scale, withCorridors, corridorOffsets, canTouch);

            return new List<NamedMapDescription>()
            {
                new NamedMapDescription(mapDescription, name, withCorridors)
            };
        }

        protected virtual NamedMapDescription GetMapDescription(string name)
        {
            var settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
            };

            MapDescription<int> mapDescription = null;

            if (File.Exists($"Resources/MapDescriptions/{name}.json"))
            {
                mapDescription = JsonConvert.DeserializeObject<MapDescription<int>>(
                    File.ReadAllText($"Resources/MapDescriptions/{name}.json"), settings);
            }
            else
            {
                using (var sr = new StreamReader($"Resources/Maps/Thesis/{name}.yml"))
                {
                    mapDescription = configLoader.LoadMapDescription(sr);
                }
            }


            return new NamedMapDescription(mapDescription, name, mapDescription.GetGraph().VerticesCount != mapDescription.GetStageOneGraph().VerticesCount);
        }

        protected virtual IRoomDescription GetBasicRoomDescription()
        {
            var basicRoomTemplates = MapDescriptionUtils.GetBasicRoomTemplates(Options.Scale);
            var basicRoomDescription = new BasicRoomDescription(basicRoomTemplates);

            return basicRoomDescription;
        }

        protected virtual IRoomDescription GetCorridorRoomDescription(List<int> corridorOffsets, int width = 1)
        {
            var corridorRoomTemplates = MapDescriptionUtils.GetCorridorRoomTemplates(corridorOffsets, width);
            var corridorRoomDescription = new CorridorRoomDescription(corridorRoomTemplates);

            return corridorRoomDescription;
        }
    }
}