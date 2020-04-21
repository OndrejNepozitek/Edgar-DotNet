using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.DataStructures.Graphs;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Utils;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public class DifferentRoomTemplates : Scenario
    {
        private DungeonGeneratorConfiguration<int> GetConfiguration(NamedMapDescription namedMapDescription, RepeatMode repeatMode = RepeatMode.AllowRepeat)
        {
            var configuration = GetBasicConfiguration(namedMapDescription);
            configuration.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration()
            {
                MaxIterationsWithoutSuccess = 100,
                HandleTreesGreedily = true,
            });
            configuration.RepeatModeOverride = repeatMode;

            return configuration;
        }

        protected override void Run()
        {
            Run(Mode.Smart);
            Run(Mode.Smart, RepeatMode.NoImmediate);
            Run(Mode.Smart, RepeatMode.NoImmediate, true);
            Run(Mode.SmallAndMedium);
            Run(Mode.SmallAndMedium, RepeatMode.NoImmediate);
            Run(Mode.SmallAndMedium, RepeatMode.NoImmediate, true);
            Run(Mode.Medium);
            RunBasic();
        }

        private void Run(Mode mode, RepeatMode repeatMode = RepeatMode.AllowRepeat, bool enhanceRoomTemplates = false)
        {
            var loader = new CustomMapDescriptionLoader(Options, mode, enhanceRoomTemplates);
            var mapDescriptions = loader.GetMapDescriptions();

            RunBenchmark(mapDescriptions, x => GetConfiguration(x, repeatMode), Options.FinalEvaluationIterations, $"{mode}_{repeatMode}_{(enhanceRoomTemplates ? "Enhance" : "NoEnhance")}");
        }

        private void RunBasic()
        {
            var mapDescriptions = GetMapDescriptions();

            RunBenchmark(mapDescriptions, x => GetConfiguration(x), Options.FinalEvaluationIterations, "Basic");
        }

        public enum Mode
        {
            SmallAndMedium, Medium, Smart
        }

        public class CustomMapDescriptionLoader : MapDescriptionLoader
        {
            private readonly List<RoomTemplate> roomTemplatesSmall;
            private readonly List<RoomTemplate> roomTemplatesMedium;
            private readonly Mode mode;
            private readonly bool enhanceRoomTemplates;

            public CustomMapDescriptionLoader(Options options, Mode mode, bool enhanceRoomTemplates) : base(options)
            {
                this.mode = mode;
                this.enhanceRoomTemplates = enhanceRoomTemplates;
                roomTemplatesSmall = GetSmallRoomTemplates();
                roomTemplatesMedium = GetMediumRoomTemplates();
            }

            private List<RoomTemplate> GetSmallRoomTemplates()
            {
                var roomTemplates = new List<RoomTemplate>();
                var doorMode = new SimpleDoorMode(2, 1);

                roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(6), doorMode, name: "Square 6x6"));
                roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(8), doorMode, name: "Square 8x8"));
                roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(6, 8), doorMode, name: "Rectangle 6x8"));

                if (enhanceRoomTemplates)
                {
                    roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(7), doorMode, name: "Square 7x7"));
                    roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(5, 7), doorMode, name: "Rectangle 5x7"));
                }

                return roomTemplates;
            }

            private List<RoomTemplate> GetMediumRoomTemplates()
            {
                var roomTemplates = new List<RoomTemplate>();
                var doorMode = new SimpleDoorMode(2, 2);

                roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(12), doorMode, name: "Square 12x12"));
                roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(14), doorMode, name: "Square 14x14"));
                roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(10, 14), doorMode, name: "Rectangle 10x14"));
                roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(12, 15), doorMode, name: "Rectangle 12x15"));

                if (enhanceRoomTemplates)
                {
                    roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(13), doorMode, name: "Square 13x13"));
                    roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(10, 16), doorMode, name: "Rectangle 10x16"));
                }

                return roomTemplates;
            }

            protected override List<NamedMapDescription> GetMapDescriptions(NamedGraph namedGraph, List<int> corridorOffsets)
            {
                var withCorridors = corridorOffsets[0] != 0;
                var canTouch = Options.CanTouch || !withCorridors;
                var corridorRoomDescription = withCorridors ? GetCorridorRoomDescription(corridorOffsets, 2) : null;

                var mapDescription = new MapDescription<int>();
                var graph = namedGraph.Graph;

                foreach (var room in graph.Vertices)
                {
                    var basicRoomDescription = GetBasicRoomDescription(graph, room);
                    mapDescription.AddRoom(room, basicRoomDescription);
                }

                var counter = graph.VerticesCount;

                foreach (var connection in graph.Edges)
                {
                    if (withCorridors)
                    {
                        mapDescription.AddRoom(counter, corridorRoomDescription);
                        mapDescription.AddConnection(connection.From, counter);
                        mapDescription.AddConnection(connection.To, counter);
                        counter++;
                    }
                    else
                    {
                        mapDescription.AddConnection(connection.From, connection.To);
                    }
                }
                
                var name = MapDescriptionUtils.GetInputName(namedGraph.Name, Options.Scale, withCorridors, corridorOffsets, canTouch);

                return new List<NamedMapDescription>()
                {
                    new NamedMapDescription(mapDescription, name, withCorridors)
                };
            }

            private IRoomDescription GetBasicRoomDescription(IGraph<int> graph, int vertex)
            {
                var roomTemplates = new List<IRoomTemplate>();

                switch (mode)
                {
                    case Mode.Medium:
                        roomTemplates.AddRange(roomTemplatesMedium);
                        break;

                    case Mode.SmallAndMedium:
                        roomTemplates.AddRange(roomTemplatesSmall);
                        roomTemplates.AddRange(roomTemplatesMedium);
                        break;
                    case Mode.Smart:
                    {
                        if (graph.GetNeighbours(vertex).Count() <= 2)
                        {
                            roomTemplates.AddRange(roomTemplatesSmall);
                        }
                        roomTemplates.AddRange(roomTemplatesMedium);
                        break;
                    }

                }

                return new BasicRoomDescription(roomTemplates);
            }
        }
    }
}