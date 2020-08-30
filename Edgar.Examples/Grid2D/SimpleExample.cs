using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Drawing;

namespace Edgar.Examples.Grid2D
{
    public class SimpleExample : IExampleGrid2D<int>
    {
        public string Name => "Simple";

        public string DocsFileName => "simple";

        public string EntryPointMethod => nameof(Run);

        public IEnumerable<LevelDescriptionGrid2D<int>> GetResults()
        {
            yield break;
        }

        public void Run()
        {
            // Create square room template
            var squareRoomTemplate = new RoomTemplateGrid2D(
                PolygonGrid2D.GetSquare(8),
                new SimpleDoorModeGrid2D(doorLength: 1, cornerDistance: 1)
            );

            // Create rectangle room template
            var rectangleRoomTemplate = new RoomTemplateGrid2D(
                PolygonGrid2D.GetRectangle(6, 10),
                new SimpleDoorModeGrid2D(doorLength: 1, cornerDistance: 1)
            );

            // Create a room description which says that the room is not a corridor and that it can use the two room templates
            var roomDescription = new RoomDescriptionGrid2D(
                isCorridor: false,
                roomTemplates: new List<RoomTemplateGrid2D>() { squareRoomTemplate, rectangleRoomTemplate }
            );

            // Create an instance of the level description
            var levelDescription = new LevelDescriptionGrid2D<int>();

            // Add 4 rooms to the level, use the room description that we created beforehand
            levelDescription.AddRoom(0, roomDescription);
            levelDescription.AddRoom(1, roomDescription);
            levelDescription.AddRoom(2, roomDescription);
            levelDescription.AddRoom(3, roomDescription);

            // Add connections between the rooms - the level graph will be a cycle with 4 vertices
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(0, 3);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            // Create an instance of the generate and generate a layout
            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
            var layout = generator.GenerateLayout();

            // Export the resulting layout as PNG
            var drawer = new DungeonDrawer<int>();
            drawer.DrawLayoutAndSave(layout, "simple_layout.png", new DungeonDrawerOptions()
            {
                Width = 2000,
                Height = 2000,
            });

            var layout2 = generator.GenerateLayout();
            // Export the resulting layout as PNG
            drawer.DrawLayoutAndSave(layout, "simple_layout_2.png", new DungeonDrawerOptions()
            {
                Width = 2000,
                Height = 2000,
            });
        }
    }
}