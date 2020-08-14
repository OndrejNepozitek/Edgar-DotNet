using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Core.Configurations;
using Edgar.Legacy.Core.Configurations.EnergyData;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.LayoutOperations;
using Edgar.Legacy.Core.Layouts;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using NUnit.Framework;

namespace MapGeneration.IntegrationTests.Core.LayoutOperations
{
    [TestFixture]
    public class RoomShapesHandlerTests
    {
        private ConfigurationSpacesGenerator configurationSpacesGenerator;

        [SetUp]
        public void SetUp()
        {
            configurationSpacesGenerator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());
        }

        [Test]
        public void AllowRepeat()
        {
            var roomTemplate1 = GetRoomTemplate(RepeatMode.AllowRepeat);
            var roomTemplate2 = GetRoomTemplate(RepeatMode.AllowRepeat);
            var roomTemplate3 = GetRoomTemplate(RepeatMode.AllowRepeat);
            var roomDescription = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate1, roomTemplate2, roomTemplate3 });

            var mapDescription = GetMapDescription(roomDescription);
            var configurationSpaces = GetConfigurationSpaces(mapDescription);

            var roomShapesHandler = new RoomShapesHandler<int, Configuration<CorridorsData>>(
                configurationSpaces,
                configurationSpaces.GetIntAliasMapping(),
                mapDescription
            );

            var roomShapes = configurationSpaces.GetIntAliasMapping().Values.ToList();
            var layout = new Layout<Configuration<CorridorsData>>(mapDescription.GetGraph());
            SetConfiguration(layout, 0, roomShapes[0]);
            SetConfiguration(layout, 1, roomShapes[1]);
            SetConfiguration(layout, 2, roomShapes[2]);

            foreach (var node in mapDescription.GetGraph().Vertices)
            {
                var shapes = roomShapesHandler.GetPossibleShapesForNode(layout, node);
                var expectedShapes = configurationSpaces.GetShapesForNode(node);
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }
        }

        [Test]
        public void DifferentTransformationsProperlyHandled()
        {
            var roomTemplate1 = GetRoomTemplate(RepeatMode.NoRepeat, TransformationGrid2DHelper.GetAllTransformationsOld().ToList());
            var roomDescription = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate1 });

            var mapDescription = GetMapDescription(roomDescription);
            var configurationSpaces = GetConfigurationSpaces(mapDescription);

            var roomShapesHandler = new RoomShapesHandler<int, Configuration<CorridorsData>>(
                configurationSpaces,
                configurationSpaces.GetIntAliasMapping(),
                mapDescription
            );

            var roomShapes = configurationSpaces.GetIntAliasMapping().Values.ToList();
            var layout = new Layout<Configuration<CorridorsData>>(mapDescription.GetGraph());
            SetConfiguration(layout, 0, roomShapes[0]);

            {
                var shapes = roomShapesHandler.GetPossibleShapesForNode(layout, 1);
                Assert.That(shapes.Count, Is.Zero);
            }
        }

        [Test]
        public void AllowRepeatOverride()
        {
            var roomTemplate1 = GetRoomTemplate(RepeatMode.NoRepeat);
            var roomTemplate2 = GetRoomTemplate(RepeatMode.NoRepeat);
            var roomTemplate3 = GetRoomTemplate(RepeatMode.NoRepeat);
            var roomDescription = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate1, roomTemplate2, roomTemplate3 });

            var mapDescription = GetMapDescription(roomDescription);
            var configurationSpaces = GetConfigurationSpaces(mapDescription);

            var roomShapesHandler = new RoomShapesHandler<int, Configuration<CorridorsData>>(
                configurationSpaces,
                configurationSpaces.GetIntAliasMapping(),
                mapDescription,
                RepeatMode.AllowRepeat
            );

            var roomShapes = configurationSpaces.GetIntAliasMapping().Values.ToList();
            var layout = new Layout<Configuration<CorridorsData>>(mapDescription.GetGraph());
            SetConfiguration(layout, 0, roomShapes[0]);
            SetConfiguration(layout, 1, roomShapes[1]);
            SetConfiguration(layout, 2, roomShapes[2]);

            foreach (var node in mapDescription.GetGraph().Vertices)
            {
                var shapes = roomShapesHandler.GetPossibleShapesForNode(layout, node);
                var expectedShapes = configurationSpaces.GetShapesForNode(node);
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }
        }

        [Test]
        public void NoRepeat()
        {
            var roomTemplate1 = GetRoomTemplate(RepeatMode.NoRepeat);
            var roomTemplate2 = GetRoomTemplate(RepeatMode.NoRepeat);
            var roomTemplate3 = GetRoomTemplate(RepeatMode.NoRepeat);
            var roomDescription = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate1, roomTemplate2, roomTemplate3 });

            var mapDescription = GetMapDescription(roomDescription);
            var configurationSpaces = GetConfigurationSpaces(mapDescription);

            var roomShapesHandler = new RoomShapesHandler<int, Configuration<CorridorsData>>(
                configurationSpaces,
                configurationSpaces.GetIntAliasMapping(),
                mapDescription
            );

            var roomShapes = configurationSpaces.GetIntAliasMapping().Values.ToList();
            var layout = new Layout<Configuration<CorridorsData>>(mapDescription.GetGraph());
            SetConfiguration(layout, 0, roomShapes[0]);
            SetConfiguration(layout, 1, roomShapes[1]);
            SetConfiguration(layout, 2, roomShapes[2]);

            foreach (var node in mapDescription.GetGraph().Vertices)
            {
                var shapes = roomShapesHandler
                    .GetPossibleShapesForNode(layout, node);
                var expectedShapes = new List<IntAlias<PolygonGrid2D>>() { roomShapes[node] };
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }
        }

        [Test]
        public void NoRepeatOverride()
        {
            var roomTemplate1 = GetRoomTemplate(RepeatMode.AllowRepeat);
            var roomTemplate2 = GetRoomTemplate(RepeatMode.AllowRepeat);
            var roomTemplate3 = GetRoomTemplate(RepeatMode.AllowRepeat);
            var roomDescription = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate1, roomTemplate2, roomTemplate3 });

            var mapDescription = GetMapDescription(roomDescription);
            var configurationSpaces = GetConfigurationSpaces(mapDescription);

            var roomShapesHandler = new RoomShapesHandler<int, Configuration<CorridorsData>>(
                configurationSpaces,
                configurationSpaces.GetIntAliasMapping(),
                mapDescription,
                RepeatMode.NoRepeat
            );

            var roomShapes = configurationSpaces.GetIntAliasMapping().Values.ToList();
            var layout = new Layout<Configuration<CorridorsData>>(mapDescription.GetGraph());
            SetConfiguration(layout, 0, roomShapes[0]);
            SetConfiguration(layout, 1, roomShapes[1]);
            SetConfiguration(layout, 2, roomShapes[2]);

            foreach (var node in mapDescription.GetGraph().Vertices)
            {
                var shapes = roomShapesHandler
                    .GetPossibleShapesForNode(layout, node);
                var expectedShapes = new List<IntAlias<PolygonGrid2D>>() { roomShapes[node] };
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }
        }

        [Test]
        public void NoImmediate()
        {
            var roomTemplate1 = GetRoomTemplate(RepeatMode.NoImmediate);
            var roomTemplate2 = GetRoomTemplate(RepeatMode.NoImmediate);
            var roomTemplate3 = GetRoomTemplate(RepeatMode.NoImmediate);
            var roomDescription = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate1, roomTemplate2, roomTemplate3 });

            var mapDescription = GetMapDescription(roomDescription);
            var configurationSpaces = GetConfigurationSpaces(mapDescription);

            var roomShapesHandler = new RoomShapesHandler<int, Configuration<CorridorsData>>(
                configurationSpaces,
                configurationSpaces.GetIntAliasMapping(),
                mapDescription
            );

            var roomShapes = configurationSpaces.GetIntAliasMapping().Values.ToList();
            var layout = new Layout<Configuration<CorridorsData>>(mapDescription.GetGraph());
            SetConfiguration(layout, 0, roomShapes[0]);
            SetConfiguration(layout, 1, roomShapes[1]);
            SetConfiguration(layout, 2, roomShapes[2]);

            {
                // Node 0
                var shapes = roomShapesHandler.GetPossibleShapesForNode(layout, 0);
                var expectedShapes = new List<IntAlias<PolygonGrid2D>>() { roomShapes[0], roomShapes[2] };
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }

            {
                // Node 1
                var shapes = roomShapesHandler.GetPossibleShapesForNode(layout, 1);
                var expectedShapes = new List<IntAlias<PolygonGrid2D>>() { roomShapes[1] };
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }

            {
                // Node 2
                var shapes = roomShapesHandler.GetPossibleShapesForNode(layout, 2);
                var expectedShapes = new List<IntAlias<PolygonGrid2D>>() { roomShapes[0], roomShapes[2] };
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }
        }

        [Test]
        public void NoImmediateWithCorridors()
        {
            var roomTemplate1 = GetRoomTemplate(RepeatMode.NoImmediate);
            var roomTemplate2 = GetRoomTemplate(RepeatMode.NoImmediate);
            var roomTemplate3 = GetRoomTemplate(RepeatMode.NoImmediate);
            var roomDescription = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate1, roomTemplate2, roomTemplate3 });
            var corridorRoomDescription = new CorridorRoomDescription(new List<RoomTemplate>() { roomTemplate1, roomTemplate2, roomTemplate3 });

            var mapDescription = GetMapDescription(roomDescription, corridorRoomDescription);
            var configurationSpaces = GetConfigurationSpaces(mapDescription);

            var roomShapesHandler = new RoomShapesHandler<int, Configuration<CorridorsData>>(
                configurationSpaces,
                configurationSpaces.GetIntAliasMapping(),
                mapDescription
            );

            var roomShapes = configurationSpaces.GetIntAliasMapping().Values.ToList();
            var layout = new Layout<Configuration<CorridorsData>>(mapDescription.GetGraph());
            SetConfiguration(layout, 0, roomShapes[0]);
            SetConfiguration(layout, 1, roomShapes[1]);
            SetConfiguration(layout, 2, roomShapes[2]);

            {
                // Node 0
                var shapes = roomShapesHandler.GetPossibleShapesForNode(layout, 0);
                var expectedShapes = new List<IntAlias<PolygonGrid2D>>() { roomShapes[0], roomShapes[2] };
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }

            {
                // Node 1
                var shapes = roomShapesHandler.GetPossibleShapesForNode(layout, 1);
                var expectedShapes = new List<IntAlias<PolygonGrid2D>>() { roomShapes[1] };
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }

            {
                // Node 2
                var shapes = roomShapesHandler.GetPossibleShapesForNode(layout, 2);
                var expectedShapes = new List<IntAlias<PolygonGrid2D>>() { roomShapes[0], roomShapes[2] };
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }

            foreach (var corridorRoom in mapDescription.GetGraph().Vertices.Where(x => mapDescription.GetRoomDescription(x) is CorridorRoomDescription))
            {
                var shapes = roomShapesHandler.GetPossibleShapesForNode(layout, corridorRoom);
                var expectedShapes = configurationSpaces.GetShapesForNode(corridorRoom);
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }
        }

        [Test]
        public void TryToFixEmpty_NoRepeatToNoImmediate()
        {
            var roomTemplate1 = GetRoomTemplate(RepeatMode.NoRepeat);
            var roomTemplate2 = GetRoomTemplate(RepeatMode.NoRepeat);
            var roomDescription = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate1, roomTemplate2 });

            var mapDescription = GetMapDescription(roomDescription);
            var configurationSpaces = GetConfigurationSpaces(mapDescription);

            var roomShapesHandler = new RoomShapesHandler<int, Configuration<CorridorsData>>(
                configurationSpaces,
                configurationSpaces.GetIntAliasMapping(),
                mapDescription
            );

            var roomShapes = configurationSpaces.GetIntAliasMapping().Values.ToList();
            var layout = new Layout<Configuration<CorridorsData>>(mapDescription.GetGraph());
            SetConfiguration(layout, 0, roomShapes[0]);
            SetConfiguration(layout, 1, roomShapes[1]);

            {
                // Node 0
                var shapes = roomShapesHandler.GetPossibleShapesForNode(layout, 0, true);
                var expectedShapes = new List<IntAlias<PolygonGrid2D>>() { roomShapes[0] };
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }

            {
                // Node 1
                var shapes = roomShapesHandler.GetPossibleShapesForNode(layout, 1, true);
                var expectedShapes = new List<IntAlias<PolygonGrid2D>>() { roomShapes[1] };
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }

            {
                // Node 2
                var shapes = roomShapesHandler.GetPossibleShapesForNode(layout, 2, true);
                var expectedShapes = new List<IntAlias<PolygonGrid2D>>() { roomShapes[0] };
                Assert.That(shapes, Is.EquivalentTo(expectedShapes));
            }
        }

        private void SetConfiguration(Layout<Configuration<CorridorsData>> layout, int node, IntAlias<PolygonGrid2D> alias)
        {
            layout.SetConfiguration(node, new Configuration<CorridorsData>(alias, new Vector2Int(0, 0), new CorridorsData(), node));
        }

        private ConfigurationSpaces<Configuration<CorridorsData>> GetConfigurationSpaces(MapDescription<int> mapDescription)
        {
            return configurationSpacesGenerator.GetConfigurationSpaces<Configuration<CorridorsData>>(mapDescription);
        }

        private MapDescription<int> GetMapDescription(BasicRoomDescription roomDescription, CorridorRoomDescription corridorRoomDescription = null)
        {
            var mapDescription = new MapDescription<int>();

            var graph = new UndirectedAdjacencyListGraph<int>();
            graph.AddVertex(0);
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);

            foreach (var vertex in graph.Vertices)
            {
                mapDescription.AddRoom(vertex, roomDescription);
            }

            var corridorCounter = graph.VerticesCount;
            foreach (var edge in graph.Edges)
            {
                if (corridorRoomDescription != null)
                {
                    mapDescription.AddRoom(corridorCounter, corridorRoomDescription);
                    mapDescription.AddConnection(corridorCounter, edge.From);
                    mapDescription.AddConnection(corridorCounter, edge.To);
                    corridorCounter++;
                }
                else
                {
                    mapDescription.AddConnection(edge.From, edge.To);
                }
            }

            return mapDescription;
        }

        private RoomTemplate GetRoomTemplate(RepeatMode repeatMode, List<TransformationGrid2D> transformations = null)
        {
            return new RoomTemplate(
                PolygonGrid2D.GetRectangle(10, 20),
                new SimpleDoorMode(1, 0),
                transformations,
                repeatMode
            );
        }
    }
}