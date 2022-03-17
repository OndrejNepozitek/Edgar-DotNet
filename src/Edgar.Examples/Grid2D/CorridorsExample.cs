using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Drawing;
using Edgar.Graphs;

namespace Edgar.Examples.Grid2D
{
    public class CorridorsExample : IExampleGrid2D<int>
    {
        #region no-clean

        public string Name => "Corridors";

        public string DocsFileName => "corridors";

        public string EntryPointMethod => nameof(GetLevelDescription);

        public ExampleOptions Options => new ExampleOptions()
        {
            Name = "Corridors",
            DocsFileName = "corridors",
            EntryPointMethod = nameof(GetLevelDescription),
        };

        #endregion

        /// <summary>
        /// Prepare level description.
        /// </summary>
        public LevelDescriptionGrid2D<int> GetLevelDescription()
        {
            //md By default, rooms in generated levels are connected directly - there are no corridors between them. If we want to use corridors, we have to add a corridor room between all pairs of neighboring rooms.

            //md > **Note:** As of now, the whole level geometry is fixed - the generator works only with the room templates that we create at design time. That means that we have to manually create all the shapes of corridor rooms. In the future, I would like to experiment with using path-finding for corridors instead of predefined room templates.

            //md ## Corridor room description
            //md First, we create the outline for the corridor room template. The performance of the generator is the best with rather short corridors, so we will use a 2x1 rectangle:

            var corridorOutline = PolygonGrid2D.GetRectangle(2, 1);

            //md The next step is to add doors. We can no longer use the simple door mode because we want to have exactly two door positions on the opposite sides of the corridor, which is not possible with the simple mode. With the manual mode, we have to specify all the door positions manually.

            var corridorDoors = new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                {
                    new DoorGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 1)),
                    new DoorGrid2D(new Vector2Int(2, 0), new Vector2Int(2, 1))
                }
            );

            //md Now we can create the corridor room template. We must not forget to allow the 90 degrees rotation because, otherwise we would not be able to connect rooms both vertically and horizontally.

            var corridorRoomTemplate = new RoomTemplateGrid2D(
                corridorOutline,
                corridorDoors,
                allowedTransformations: new List<TransformationGrid2D>()
                {
                    TransformationGrid2D.Identity,
                    TransformationGrid2D.Rotate90
                }
            );

            //md We can also add another corridor room template which is a little bit longer then the previous one:

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

            //md Below we can see a visualization of the two room templates:

            //md ![](./corridors/room_templates.png)

            //md And finally, we can create the corridor room description. We must not forget to set the `IsCorridor` flag to `true`.

            var corridorRoomDescription = new RoomDescriptionGrid2D
            (
                isCorridor: true,
                roomTemplates: new List<RoomTemplateGrid2D>() {corridorRoomTemplate, corridorRoomTemplateLonger}
            );

            //md ## Basic room description
            //md For non-corridor rooms, we will use three rectangular room templates - 6x6 square, 8x8 square and 6x10 rectangle. The full code is omitted for simplicity.

            var basicRoomDescription = GetBasicRoomDescription();

            //md ## Level description
            //md First, we create a level description.

            var levelDescription = new LevelDescriptionGrid2D<int>();

            //md Next, we create a graph of rooms. Instead of adding rooms and connections directly to the level description, it might be sometimes beneficial to first prepare the graph data structure itself and then go through individual rooms and connections and add them to the level description.

            var graph = new UndirectedAdjacencyListGraph<int>();

            graph.AddVerticesRange(0, 13);

            graph.AddEdge(0, 1);
            graph.AddEdge(0, 2);
            graph.AddEdge(0, 8);
            graph.AddEdge(1, 3);
            graph.AddEdge(1, 4);
            graph.AddEdge(1, 5);
            graph.AddEdge(2, 6);
            graph.AddEdge(2, 7);
            graph.AddEdge(5, 9);
            graph.AddEdge(6, 9);
            graph.AddEdge(8, 10);
            graph.AddEdge(8, 11);
            graph.AddEdge(8, 12);

            //md > **Note:** As we want to have corridors between all neighboring rooms, it is better to create the graph only with non-corridor rooms and then add corridor rooms programatically.

            //md When we have the graph ready, we can add non-corridor rooms:

            foreach (var room in graph.Vertices)
            {
                levelDescription.AddRoom(room, basicRoomDescription);
            }

            //md Before we add corridor rooms, we have to figure out how to identify them. As we use integers, probably the easiest way is to number the corridor rooms and keep track which was the last used number:

            var corridorCounter = graph.VerticesCount;

            //md Now we can add corridor rooms. For each edge of the original graph, we create a corridor room and connect it to the two non-corridor rooms:

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

        /// <summary>
        /// Run the generator and export the result.
        /// </summary>
        public void Run()
        {
            var levelDescription = GetLevelDescription();
            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
            var layout = generator.GenerateLayout();

            var drawer = new DungeonDrawer<int>();
            var bitmap = drawer.DrawLayout(layout, new DungeonDrawerOptions()
            {
                Width = 1000,
                Height = 1000,
            });
            bitmap.Save("corridors.png");

            #region no-clean

            var roomTemplates = levelDescription
                .GetGraph().Vertices
                .Select(levelDescription.GetRoomDescription)
                .Where(x => x.IsCorridor)
                .SelectMany(x => x.RoomTemplates)
                .Distinct()
                .ToList();
            var roomTemplatesDrawer = new RoomTemplateDrawer();
            var roomTemplatesBitmap = roomTemplatesDrawer.DrawRoomTemplates(roomTemplates, new DungeonDrawerOptions()
            {
                Width = 1200,
                Height = 350,
                FontSize = 0.7f,
                PaddingAbsolute = 135,
                ShowRoomNames = false,
                EnableHatching = false,
            });
            roomTemplatesBitmap.Save(ExamplesGenerator.AssetsFolder + "/room_templates.png");

            #endregion
        }

        #region no-clean

        public IEnumerable<LevelDescriptionGrid2D<int>> GetResults()
        {
            //md Below you can see some of the results generated from this example:

            yield return GetLevelDescription();
        }

        #endregion

        private RoomDescriptionGrid2D GetBasicRoomDescription()
        {
            var doors = new SimpleDoorModeGrid2D(doorLength: 1, cornerDistance: 1);
            var transformations = new List<TransformationGrid2D>()
            {
                TransformationGrid2D.Identity,
                TransformationGrid2D.Rotate90
            };

            var squareRoom1 = new RoomTemplateGrid2D(
                PolygonGrid2D.GetSquare(8),
                doors,
                name: "Square 8x8",
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
                roomTemplates: new List<RoomTemplateGrid2D>()
                {
                    squareRoom1,
                    squareRoom2,
                    rectangleRoom
                }
            );
        }
    }
}