using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Doors;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Graphs;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Examples.Grid2D
{
    public class RealLifeExample : IExampleGrid2D<RealLifeExample.Room>
    {
        public string Name => "Real-life example";

        public string DocsFileName => "real-life";

        public LevelDescriptionGrid2D<Room> GetLevelDescription() 
        {
            //md In this example, we will create a level description that should be close to what we could use in a game. We will cover the following:
            //md - create a room type enum for individual types of rooms - spawn, boss, shop, reward, etc.
            //md - create a custom room class to identify rooms in the level graph
            //md - assign room templates based on the type of the room
            //md - use corridors with different lengths

            //sc enum:RoomType

            //sc class:Room

            //md ## Room templates
            //sc method:GetRoomTemplates

            #region Test 2

            var levelDescription = new LevelDescriptionGrid2D<Room>();
            levelDescription.MinimumRoomDistance = 2;
            levelDescription.RoomTemplateRepeatModeDefault = RoomTemplateRepeatMode.NoImmediate;
            var graph = GetGraph();

            #region Test

            var roomTemplates = GetRoomTemplates();
            var corridorRoomDescription = new RoomDescriptionGrid2D()
            {
                IsCorridor = true,
                RoomTemplates = GetCorridorRoomTemplates(),
            };

            #endregion

            foreach (var room in graph.Vertices)
            {
                levelDescription.AddRoom(room, new RoomDescriptionGrid2D()
                {
                    IsCorridor = false,
                    RoomTemplates = GetRoomTemplatesForRoom(room.Type, roomTemplates),
                });
            }

            foreach (var edge in graph.Edges)
            {
                var corridorRoom = new Room(RoomType.Corridor);

                levelDescription.AddRoom(corridorRoom, corridorRoomDescription);
                levelDescription.AddConnection(edge.From, corridorRoom);
                levelDescription.AddConnection(edge.To, corridorRoom);
            }

            return levelDescription;

            #endregion
        }

        private List<RoomTemplateGrid2D> GetRoomTemplatesForRoom(RoomType type, Dictionary<string, RoomTemplateGrid2D> roomTemplates)
        {
            switch (type)
            {
                case RoomType.Spawn:
                    return new List<RoomTemplateGrid2D>()
                    {
                        roomTemplates["Spawn"],
                    };

                case RoomType.Boss:
                    return new List<RoomTemplateGrid2D>()
                    {
                        roomTemplates["Boss"],
                    };

                case RoomType.Exit:
                    return new List<RoomTemplateGrid2D>()
                    {
                        roomTemplates["Exit"],
                    };

                case RoomType.Reward:
                    return new List<RoomTemplateGrid2D>()
                    {
                        roomTemplates["Reward"],
                    };

                case RoomType.Shop:
                    return new List<RoomTemplateGrid2D>()
                    {
                        roomTemplates["Shop"],
                    };

                case RoomType.Hub:
                    return new List<RoomTemplateGrid2D>()
                    {
                        roomTemplates["Hub 1"],
                    };

                case RoomType.Normal:
                    return new List<RoomTemplateGrid2D>()
                    {
                        roomTemplates["Normal 1"],
                        roomTemplates["Normal 2"],
                        roomTemplates["Normal 3"],
                        roomTemplates["Normal 4"],
                        roomTemplates["Normal 5"],
                        roomTemplates["Normal 6"],
                        roomTemplates["Normal 7"],
                    };

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private Dictionary<string, RoomTemplateGrid2D> GetRoomTemplates()
        {
            return new Dictionary<string, RoomTemplateGrid2D>()
            {
                {"Normal 1", new RoomTemplateGrid2D(
                    PolygonGrid2D.GetRectangle(15, 18),
                    new SimpleDoorModeGrid2D(1, 2),
                    allowedTransformations: TransformationGrid2DHelper.GetRotations()
                )},
                {"Normal 2", new RoomTemplateGrid2D(
                    PolygonGrid2D.GetRectangle(13, 15),
                    new SimpleDoorModeGrid2D(1, 2),
                    allowedTransformations: TransformationGrid2DHelper.GetRotations()
                )},
                {"Normal 3", new RoomTemplateGrid2D(
                    new PolygonGrid2DBuilder()
                        .AddPoint(-11, 6).AddPoint(-5, 6).AddPoint(-5, 5).AddPoint(-3, 5)
                        .AddPoint(-3, 6).AddPoint(2, 6).AddPoint(2, 5).AddPoint(4, 5)
                        .AddPoint(4, 6).AddPoint(10, 6).AddPoint(10, -1).AddPoint(4, -1)
                        .AddPoint(4, 0).AddPoint(2, 0).AddPoint(2, -1).AddPoint(-3, -1)
                        .AddPoint(-3, 0).AddPoint(-5, 0).AddPoint(-5, -1).AddPoint(-11, -1)
                        .Build(),
                    new SimpleDoorModeGrid2D(1, 2),
                    repeatMode: RoomTemplateRepeatMode.NoRepeat,
                    allowedTransformations: TransformationGrid2DHelper.GetRotations()
                )},
                #region hidden:Other room templates
                {"Normal 4", new RoomTemplateGrid2D(
                    new PolygonGrid2DBuilder()
                        .AddPoint(-39, 1).AddPoint(-37, 1).AddPoint(-37, 10).AddPoint(-39, 10)
                        .AddPoint(-39, 15).AddPoint(-26, 15).AddPoint(-26, 10).AddPoint(-28, 10)
                        .AddPoint(-28, 1).AddPoint(-26, 1).AddPoint(-26, -4).AddPoint(-39, -4)
                        .Build(),
                    new SimpleDoorModeGrid2D(1, 2),
                    allowedTransformations: TransformationGrid2DHelper.GetRotations()
                )},
                {"Normal 5", new RoomTemplateGrid2D(
                    new PolygonGrid2DBuilder()
                        .AddPoint(-14, 3).AddPoint(0, 3).AddPoint(0, 5).AddPoint(-14, 5)
                        .AddPoint(-14, 12).AddPoint(8, 12).AddPoint(8, -4).AddPoint(-6, -4)
                        .AddPoint(-6, -6).AddPoint(8, -6).AddPoint(8, -13).AddPoint(-14, -13)
                        .Build(),
                    new SimpleDoorModeGrid2D(1, 2),
                    repeatMode: RoomTemplateRepeatMode.NoRepeat
                )},
                {"Normal 6", new RoomTemplateGrid2D(
                    PolygonGrid2D.GetSquare(15), 
                    new SimpleDoorModeGrid2D(1, 2)
                )},
                {"Spawn", new RoomTemplateGrid2D(
                    PolygonGrid2D.GetSquare(11), 
                    new SimpleDoorModeGrid2D(1, 2)
                )},
                {"Hub 1", new RoomTemplateGrid2D(
                    PolygonGrid2D.GetRectangle(16, 20), 
                    new SimpleDoorModeGrid2D(1, 4),
                    allowedTransformations: TransformationGrid2DHelper.GetRotations()
                )},
                {"Boss", new RoomTemplateGrid2D(
                    PolygonGrid2D.GetRectangle(22, 22),
                    new SimpleDoorModeGrid2D(1, 4),
                    allowedTransformations: TransformationGrid2DHelper.GetRotations()
                )},
                {"Reward", new RoomTemplateGrid2D(
                    new PolygonGrid2DBuilder()
                        .AddPoint(-8, 7).AddPoint(-7, 7).AddPoint(-7, 8).AddPoint(3, 8)
                        .AddPoint(3, 7).AddPoint(4, 7).AddPoint(4, -3).AddPoint(3, -3)
                        .AddPoint(3, -4).AddPoint(-7, -4).AddPoint(-7, -3).AddPoint(-8, -3)
                        .Build(),
                    new SimpleDoorModeGrid2D(1, 2)
                )},
                {"Normal 7", new RoomTemplateGrid2D(
                    PolygonGrid2D.GetRectangle(12, 17), 
                    new SimpleDoorModeGrid2D(1, 3),
                    allowedTransformations: TransformationGrid2DHelper.GetRotations()
                )},
                {"Exit", new RoomTemplateGrid2D(
                    new PolygonGrid2DBuilder()
                        .AddPoint(-3, 4).AddPoint(4, 4).AddPoint(4, -1).AddPoint(-3, -1)
                        .Build(),
                    new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                        {
                        new DoorGrid2D(new Vector2Int(4, 2), new Vector2Int(4, 1)),
                        new DoorGrid2D(new Vector2Int(-3, 2), new Vector2Int(-3, 1)),
                        new DoorGrid2D(new Vector2Int(0, 4), new Vector2Int(1, 4)),
                        new DoorGrid2D(new Vector2Int(0, -1), new Vector2Int(1, -1)),
                        }
                    )
                )},
                {"Shop", new RoomTemplateGrid2D(
                    new PolygonGrid2DBuilder()
                        .AddPoint(-8, 7).AddPoint(-7, 7).AddPoint(-7, 8).AddPoint(3, 8)
                        .AddPoint(3, 7).AddPoint(4, 7).AddPoint(4, -3).AddPoint(3, -3)
                        .AddPoint(3, -4).AddPoint(-7, -4).AddPoint(-7, -3).AddPoint(-8, -3)
                        .Build(),
                    new SimpleDoorModeGrid2D(1, 2)
                )},
                {"Secret", new RoomTemplateGrid2D(
                    PolygonGrid2D.GetSquare(9), 
                    new SimpleDoorModeGrid2D(1, 2)
                )},
                #endregion
            };
        }

        private List<RoomTemplateGrid2D> GetCorridorRoomTemplates()
        {
            return new List<RoomTemplateGrid2D>()
            {
                new RoomTemplateGrid2D(
                    new PolygonGrid2DBuilder()
                        .AddPoint(-4, 1).AddPoint(0, 1).AddPoint(0, -2).AddPoint(-4, -2)
                        .Build(),
                    new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                        {
                        new DoorGrid2D(new Vector2Int(-4, 0), new Vector2Int(-4, -1)),
                        new DoorGrid2D(new Vector2Int(0, 0), new Vector2Int(0, -1)),
                        }
                    )
                ),
                new RoomTemplateGrid2D(
                    new PolygonGrid2DBuilder()
                        .AddPoint(-4, 1).AddPoint(2, 1).AddPoint(2, -2).AddPoint(-4, -2)
                        .Build(),
                    new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                        {
                        new DoorGrid2D(new Vector2Int(2, 0), new Vector2Int(2, -1)),
                        new DoorGrid2D(new Vector2Int(-4, 0), new Vector2Int(-4, -1)),
                        }
                    )
                ),
                new RoomTemplateGrid2D(
                    new PolygonGrid2DBuilder()
                        .AddPoint(-4, 1).AddPoint(4, 1).AddPoint(4, -2).AddPoint(-4, -2)
                        .Build(),
                    new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                        {
                        new DoorGrid2D(new Vector2Int(4, 0), new Vector2Int(4, -1)),
                        new DoorGrid2D(new Vector2Int(-4, 0), new Vector2Int(-4, -1)),
                        }
                    )
                ),
                new RoomTemplateGrid2D(
                    new PolygonGrid2DBuilder()
                        .AddPoint(-3, 2).AddPoint(0, 2).AddPoint(0, -2).AddPoint(-3, -2)
                        .Build(),
                    new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                        {
                        new DoorGrid2D(new Vector2Int(-2, -2), new Vector2Int(-1, -2)),
                        new DoorGrid2D(new Vector2Int(-2, 2), new Vector2Int(-1, 2)),
                        }
                    )
                ),
                new RoomTemplateGrid2D(
                    new PolygonGrid2DBuilder()
                        .AddPoint(-3, 4).AddPoint(0, 4).AddPoint(0, -2).AddPoint(-3, -2)
                        .Build(),
                    new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                        {
                        new DoorGrid2D(new Vector2Int(-2, -2), new Vector2Int(-1, -2)),
                        new DoorGrid2D(new Vector2Int(-2, 4), new Vector2Int(-1, 4)),
                        }
                    )
                ),
                new RoomTemplateGrid2D(
                    new PolygonGrid2DBuilder()
                        .AddPoint(-3, 6).AddPoint(0, 6).AddPoint(0, -2).AddPoint(-3, -2)
                        .Build(),
                    new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                        {
                        new DoorGrid2D(new Vector2Int(-2, -2), new Vector2Int(-1, -2)),
                        new DoorGrid2D(new Vector2Int(-2, 6), new Vector2Int(-1, 6)),
                        }
                    )
                ),
            };;
        }

        private IGraph<Room> GetGraph2()
        {
            var roomTemplateNames = new List<string>()
            {
                "Normal 1",
                "Normal 1 - duplicate",
                "Normal 2",
                "Normal 3",
                "Normal 4",
                "Normal 5",
                "Normal 6",
                "Spawn",
                "Hub 1",
                "Hub 3 - duplicate 1",
                "Hub 3 - duplicate 2",
                "Reward",
                "Normal 7 small",
                "Normal 8",
                "Normal 7 small - duplicate",
                "Reward - duplicate",
                "Secret",
            };

            var rooms = new List<Room>();

            foreach (var roomName in roomTemplateNames)
            {
                rooms.Add(new Room(RoomType.Normal));
                // rooms.Last().Name = roomName;
            }

            var graph = new UndirectedAdjacencyListGraph<Room>();

            foreach (var room in rooms)
            {
                graph.AddVertex(room);
            }

            for (int i = 1; i < roomTemplateNames.Count; i++)
            {
                graph.AddEdge(rooms[i - 1], rooms[i]);
            }

            return graph;
        }

        private IGraph<Room> GetGraph()
        {
            var rooms = new Dictionary<string, Room>
            {
                {"Spawn", new Room(RoomType.Spawn)},
                {"Normal 1", new Room(RoomType.Normal)},
                {"Normal 2", new Room(RoomType.Normal)},
                {"Normal 3", new Room(RoomType.Normal)},
                {"Normal 4", new Room(RoomType.Normal)},
                {"Boss", new Room(RoomType.Boss)},
                {"Exit", new Room(RoomType.Exit)},

                {"Normal 5", new Room(RoomType.Normal)},
                {"Shop 1", new Room(RoomType.Shop)},
                {"Normal 6", new Room(RoomType.Normal)},
                {"Reward 1", new Room(RoomType.Reward)},

                {"Hub 1", new Room(RoomType.Hub)},
                {"Reward 2", new Room(RoomType.Reward)},
                {"Normal 7", new Room(RoomType.Normal)},
                {"Normal 8", new Room(RoomType.Normal)},
                
            };

            var graph = new UndirectedAdjacencyListGraph<Room>();

            foreach (var room in rooms.Values)
            {
                graph.AddVertex(room);
            }

            // Main path to boss room
            graph.AddEdge(rooms["Spawn"], rooms["Normal 1"]);
            graph.AddEdge(rooms["Normal 1"], rooms["Normal 2"]);
            graph.AddEdge(rooms["Normal 2"], rooms["Normal 3"]);
            graph.AddEdge(rooms["Normal 3"], rooms["Normal 4"]);
            graph.AddEdge(rooms["Normal 4"], rooms["Boss"]);
            graph.AddEdge(rooms["Boss"], rooms["Exit"]);

            // Branch 1
            graph.AddEdge(rooms["Normal 2"], rooms["Normal 5"]);
            graph.AddEdge(rooms["Normal 5"], rooms["Shop 1"]);
            graph.AddEdge(rooms["Shop 1"], rooms["Normal 6"]);
            graph.AddEdge(rooms["Normal 6"], rooms["Reward 1"]);
            graph.AddEdge(rooms["Reward 1"], rooms["Normal 5"]);

            // Branch 2
            graph.AddEdge(rooms["Normal 3"], rooms["Hub 1"]);
            graph.AddEdge(rooms["Hub 1"], rooms["Reward 2"]);
            graph.AddEdge(rooms["Reward 2"], rooms["Normal 7"]);
            graph.AddEdge(rooms["Normal 7"], rooms["Normal 8"]);
            graph.AddEdge(rooms["Normal 8"], rooms["Hub 1"]);


            return graph;
        }

        public IEnumerable<LevelDescriptionGrid2D<Room>> GetResults()
        {
            yield return GetLevelDescription();
        }

        /// <summary>
        /// Test
        /// </summary>
        public enum RoomType
        {
            Normal, Spawn, Boss, Corridor, Exit, Reward, Shop, Hub
        }

        public class Room
        {
            public RoomType Type { get; }

            public Room(RoomType type)
            {
                Type = type;
            }

            public override string ToString()
            {
                return Type.ToString();
            }
        }
    }
}