using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Graphs;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.LayoutGenerators.Interfaces;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapLayouts;
using MapGeneration.Simplified.Transactions;
using MapGeneration.Utils;
using MapGeneration.Utils.Interfaces;

namespace MapGeneration.Simplified
{
    public class SimpleDungeonGenerator :  IRandomInjectable, ICancellable, IObservableGenerator<MapLayout<int>>
    {
        private Random random = new Random(0);
        private CancellationToken? cancellationToken;
        private LevelGeometry<int> levelGeometry;
        private LevelBuilder<int> levelBuilder;
        private List<RoomTemplateInstance> roomTemplateInstances;
        private List<RoomTemplateInstance> corridorRoomTemplateInstances;
        private List<RoomTemplateInstance> extendedCorridorRoomTemplateInstances;
        private int roomCounter;

        public void InjectRandomGenerator(Random random)
        {
            this.random = random;
        }

        public void SetCancellationToken(CancellationToken? cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }

        private void TransactionTest()
        {
            var layout = new TransactionLayout<int>();

            using (var transaction1 = layout.BeginChanges())
            {
                layout.Graph.AddVertex(0);

                using (var transaction2 = layout.BeginChanges())
                {
                    layout.Graph.AddVertex(1);
                    transaction2.Commit();
                }

                using (var transaction3 = layout.BeginChanges())
                {
                    layout.Graph.AddVertex(2);
                    transaction3.Rollback();
                }

                using (var transaction4 = layout.BeginChanges())
                {
                    layout.Graph.AddVertex(3);
                }

                using (var transaction5 = layout.BeginChanges())
                {
                    layout.Graph.AddVertex(4);
                    transaction5.Commit();
                }

                transaction1.Commit();
            }

            Console.WriteLine(string.Join(" ", layout.Graph.Vertices));
        }

        public MapLayout<int> GenerateLayout()
        {
            levelGeometry = new LevelGeometry<int>();
            levelGeometry.InjectRandomGenerator(random);

            levelBuilder = new LevelBuilder<int>(levelGeometry);
            levelBuilder.InjectRandomGenerator(random);

            roomCounter = 0;

            var roomTemplates = new List<RoomTemplate>()
            {
                //new RoomTemplate(GridPolygon.GetSquare(10), new SimpleDoorMode(2, 2)),
                //new RoomTemplate(GridPolygon.GetRectangle(10, 15), new SimpleDoorMode(2, 2), allowedTransformations: TransformationHelper.GetAllTransformations().ToList()),
            };

            roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(6), new SimpleDoorMode(2, 1), name: "Square 6x6"));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(8), new SimpleDoorMode(2, 1), name: "Square 8x8"));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(6, 8), new SimpleDoorMode(2, 1), name: "Rectangle 6x8"));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(12), new SimpleDoorMode(2, 2), name: "Square 12x12"));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetSquare(14), new SimpleDoorMode(2, 2), name: "Square 14x14"));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(10, 14), new SimpleDoorMode(2, 2), name: "Rectangle 10x14"));
            roomTemplates.Add(new RoomTemplate(GridPolygon.GetRectangle(12, 15), new SimpleDoorMode(2, 2), name: "Rectangle 12x15"));

            roomTemplateInstances = roomTemplates
                .SelectMany(levelGeometry.GetRoomTemplateInstances)
                .ToList();

            var corridorRoomTemplates = MapDescriptionUtils.GetCorridorRoomTemplates(new List<int>() {2, 4, 6 }, 2);
            // corridorRoomTemplates.Add(GetLShapeCorridor());
            corridorRoomTemplateInstances = corridorRoomTemplates
                .SelectMany(levelGeometry.GetRoomTemplateInstances)
                .ToList();

            var transformations = TransformationHelper.GetAllTransformations().ToList();
            var extendedCorridorRoomTemplates = corridorRoomTemplates
                .Select(x => new RoomTemplate(x.Shape, new SimpleDoorMode(2, 0), transformations)).ToList();
            extendedCorridorRoomTemplateInstances = corridorRoomTemplateInstances
                .Select(x => new RoomTemplate(x.RoomShape, new SimpleDoorMode(2, 0)))
                .SelectMany(levelGeometry.GetRoomTemplateInstances)
                .ToList();

            // corridorRoomTemplateInstances = extendedCorridorRoomTemplateInstances;

            var layout = new SimpleLayout<int>();

            // TODO: weird that we have to both set the configuration and the graph vertex
            var initialRoom = CreateRoom();
            layout.AddRoom(initialRoom, new SimpleConfiguration<int>(initialRoom, new IntVector2(0, 0), roomTemplateInstances[0]));

            OnPartialValid?.Invoke(levelGeometry.ConvertLayout(layout));

            while (layout.Graph.VerticesCount < 500)
            {
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                {
                    return null;
                }

                ExpandLayout(layout);
            }

            OnValid?.Invoke(levelGeometry.ConvertLayout(layout));

            return levelGeometry.ConvertLayout(layout);
        }

        private void ExpandLayout(SimpleLayout<int> layout)
        {
            var corridorChance = 1d;
            var corridorInsteadOfRoomChance = 0.0d;
            var attempts = 0;

            while (true)
            {
                attempts++;

                using (var transaction = layout.BeginChanges())
                {
                    var newRoom = CreateRoom();
                    var verticesWithoutCurrentRoom = layout.Graph.Vertices.Where(x => x != newRoom).ToList();

                    layout.AddRoom(newRoom);

                    var connectionTargets = new List<ConnectionTarget<int>>();

                    var neighborRoom1 = verticesWithoutCurrentRoom.GetRandom(random);
                    var neighborRoom1Configuration = layout.GetConfiguration(neighborRoom1);

                    if (random.NextDouble() <= corridorChance)
                    {
                        //var corridorRoom = CreateRoom();
                        //layout.AddRoom(corridorRoom);
                        //layout.AddConnection(newRoom, corridorRoom);
                        //layout.AddConnection(neighborRoom1, corridorRoom);
                        //connectionTargets.Add(new ConnectionTarget<int>(neighborRoom1Configuration, corridorRoomTemplateInstances));

                        layout.AddCorridorConnection(newRoom, neighborRoom1, CreateRoom());
                        connectionTargets.Add(new ConnectionTarget<int>(neighborRoom1Configuration, corridorRoomTemplateInstances));
                    }
                    else
                    {
                        layout.AddConnection(newRoom, neighborRoom1);
                        connectionTargets.Add(new ConnectionTarget<int>(neighborRoom1Configuration));
                    }

                    if (layout.Graph.VerticesCount % 5 == 0 && attempts < 10)
                    {
                        var neighborRoom2 = verticesWithoutCurrentRoom.GetRandom(random);
                        var neighborRoom2Configuration = layout.GetConfiguration(neighborRoom2);

                        if (neighborRoom2 == neighborRoom1)
                        {
                            continue;
                        }

                        if (layout.Graph.HasEdge(neighborRoom1, neighborRoom2))
                        {
                            continue;
                        }

                        if (random.NextDouble() < corridorChance)
                        {
                            //var corridorRoom = CreateRoom();
                            //layout.AddRoom(corridorRoom);
                            //layout.AddConnection(newRoom, corridorRoom);
                            //layout.AddConnection(neighborRoom2, corridorRoom);
                            //connectionTargets.Add(new ConnectionTarget<int>(neighborRoom2Configuration, corridorRoomTemplateInstances));

                            layout.AddCorridorConnection(newRoom, neighborRoom2, CreateRoom());
                            connectionTargets.Add(new ConnectionTarget<int>(neighborRoom2Configuration, corridorRoomTemplateInstances));
                        }
                        else
                        {
                            layout.AddConnection(newRoom, neighborRoom2);
                            connectionTargets.Add(new ConnectionTarget<int>(neighborRoom2Configuration));
                        }
                    }

                    var roomTemplate = roomTemplateInstances.GetRandom(random);

                    if (random.NextDouble() < corridorInsteadOfRoomChance)
                    {
                        roomTemplate = extendedCorridorRoomTemplateInstances.GetRandom(random);
                    }

                    //if (levelBuilder.TryConnectRoomToNeighbors(layout, newRoom, roomTemplate, connectionTargets, 5))
                    //{
                    //    foreach (var neighbor in layout.Graph.GetNeighbours(newRoom))
                    //    {
                    //        if (layout.GetConfiguration(neighbor) == null)
                    //        {
                    //            if (!levelBuilder.TryAddCorridorRoom(layout, neighbor, corridorRoomTemplateInstances))
                    //            {
                    //                return;
                    //            }
                    //            else
                    //            {
                    //                var configuration = layout.GetConfiguration(neighbor);
                    //                var roomTemplateInstance = configuration.RoomTemplateInstance;
                    //                var index = corridorRoomTemplateInstances.IndexOf(roomTemplateInstance);
                    //                var newRoomTemplateInstance = extendedCorridorRoomTemplateInstances[index];
                    //                layout.SetConfiguration(neighbor, new SimpleConfiguration<int>(neighbor, configuration.Position, newRoomTemplateInstance));
                    //            }
                    //        }
                    //    }

                    //    OnPartialValid?.Invoke(levelGeometry.ConvertLayout(layout));

                    //    transaction.Commit();
                    //    return;
                    //}

                    if (levelBuilder.TryConnectRoomToNeighbors(layout, newRoom, roomTemplate, corridorRoomTemplateInstances, 5))
                    {
                        if (levelBuilder.TryAddCorridors(layout, newRoom, corridorRoomTemplateInstances))
                        {
                            OnPartialValid?.Invoke(levelGeometry.ConvertLayout(layout));

                            transaction.Commit();
                            return;
                        }
                    }
                }
            }
        }

        private RoomTemplate GetLShapeCorridor()
        {
            var transformations = TransformationHelper.GetAllTransformations().ToList();

            return new RoomTemplate(
                new GridPolygonBuilder()
                    .AddPoint(0, 0)
                    .AddPoint(0, 6)
                    .AddPoint(2, 6)
                    .AddPoint(2, 2)
                    .AddPoint(6, 2)
                    .AddPoint(6, 0)
                    .Build()
                , new ManualDoorMode(new List<OrthogonalLine>()
                {
                    new OrthogonalLine(new IntVector2(0, 6), new IntVector2(2, 6)),
                    new OrthogonalLine(new IntVector2(6, 2), new IntVector2(6, 0)),
                }), transformations, name: "L-shape");
        }

        private int CreateRoom()
        {
            return roomCounter++;
        }
        
        public event Action<MapLayout<int>> OnPerturbed;
        public event Action<MapLayout<int>> OnPartialValid;
        public event Action<MapLayout<int>> OnValid;
    }
}