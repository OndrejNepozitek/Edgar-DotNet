using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Graphs;

namespace Edgar.Examples.Grid2D
{
    public class PathfindingExample : IExampleGrid2D<int>
    {
        #region no-clean

        public ExampleOptions Options => new ExampleOptions()
        {
            Name = "Pathfinding",
            DocsFileName = "pathfinding",
            EntryPointMethod = nameof(Run),
            IncludeResults = true,
            IncludeSourceCode = false,
        };

        #endregion

        public LevelDescriptionGrid2D<int> GetLevelDescription()
        {
            var corridorOutline = PolygonGrid2D.GetRectangle(2, 1);
            var corridorDoors = new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                {
                    new DoorGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 1)),
                    new DoorGrid2D(new Vector2Int(2, 0), new Vector2Int(2, 1))
                }
            );
            var corridorRoomTemplate = new RoomTemplateGrid2D(
                corridorOutline,
                corridorDoors,
                allowedTransformations: new List<TransformationGrid2D>()
                {
                    TransformationGrid2D.Identity,
                    TransformationGrid2D.Rotate90
                }
            );
            var corridorRoomTemplateLonger = new RoomTemplateGrid2D(
                PolygonGrid2D.GetRectangle(4, 1),
                new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                    {
                        new DoorGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 1)),
                        new DoorGrid2D(new Vector2Int(4, 0), new Vector2Int(4, 1))
                    }
                ),
                allowedTransformations: new List<TransformationGrid2D>()
                {
                    TransformationGrid2D.Identity,
                    TransformationGrid2D.Rotate90
                }
            );
            var corridorRoomDescription = new RoomDescriptionGrid2D
            (
                isCorridor: true,
                roomTemplates: new List<RoomTemplateGrid2D>() {corridorRoomTemplate, corridorRoomTemplateLonger}
            );
            var basicRoomDescription = GetBasicRoomDescription();
            var levelDescription = new LevelDescriptionGrid2D<int>();
            var graph = new UndirectedAdjacencyListGraph<int>();
            graph.AddVerticesRange(0, 2);
            graph.AddEdge(0, 1);

            foreach (var room in graph.Vertices)
            {
                levelDescription.AddRoom(room, basicRoomDescription);
            }

            var corridorCounter = graph.VerticesCount;

            foreach (var connection in graph.Edges)
            {
                // We manually insert a new room between each pair of neighboring rooms in the graph
                levelDescription.AddRoom(corridorCounter, corridorRoomDescription);
                // And instead of connecting the rooms directly, we connect them to the corridor room
                levelDescription.AddConnection(connection.From, corridorCounter);
                levelDescription.AddConnection(connection.To, corridorCounter);
                corridorCounter++;
            }
            return levelDescription;
        }

        private RoomDescriptionGrid2D GetBasicRoomDescription()
        {
            var doors = new SimpleDoorModeGrid2D(doorLength: 1, cornerDistance: 1);
            var transformations = new List<TransformationGrid2D>()
            {
                TransformationGrid2D.Identity,
                TransformationGrid2D.Rotate90
            };
            var squareRoom1 = new RoomTemplateGrid2D(
                PolygonGrid2D.GetSquare(3),
                doors,
                name: "Square 3x3",
                allowedTransformations: transformations
            );
            var squareRoom2 = new RoomTemplateGrid2D(
                PolygonGrid2D.GetSquare(6),
                doors,
                name: "Square 6x6",
                allowedTransformations: transformations
            );
            var rectangleRoom = new RoomTemplateGrid2D(
                PolygonGrid2D.GetRectangle(6, 10),
                doors,
                name: "Rectangle 6x10",
                allowedTransformations: transformations
            );
            return new RoomDescriptionGrid2D
            (
                isCorridor: false,
                roomTemplates: new List<RoomTemplateGrid2D>() {            
                    squareRoom1,
                }
            );
        }
       
        public IEnumerable<LevelDescriptionGrid2D<int>> GetResults()
        {
            var levelDescription = GetLevelDescription();
            levelDescription = new CorridorsExample().GetLevelDescription();
            levelDescription.UsePathfinding = true;

            yield return levelDescription;

            levelDescription = new CorridorsExample().GetLevelDescription();
            levelDescription.UsePathfinding = false;

            yield return levelDescription;
        }

        public void Run()
        {

        }
    }
}