using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Doors;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Examples.Grid2D
{
    public class BasicExample : IExampleGrid2D
    {
        public string Name => "Basics";

        public string DocsFileName => "basic";

        public LevelDescriptionGrid2D<int> GetLevelDescription()
        {
            //md In this examples, we will generate a very simple level consisting of 4 rooms with rectangular shapes.

            //md ## Room templates
            //md First, we will create our room templates. We must create an instance of the `RoomTemplateGrid2D` class for each room template. To do that, we need to create a *polygon* that defines the outline of the room template and also provide a list of possible door positions.

            //md ### Outline
            //md In the Grid 2D setting, the outline of a room template is an orthogonal polygon where each point has integer coordinates. In other words, it is a polygon that we can draw on an integer grid using 1x1 square tiles.
            
            //md The first outline that we create is an 8x8 square:

            var squareRoomOutline = new PolygonGrid2DBuilder()
                .AddPoint(0, 0)
                .AddPoint(0, 8)
                .AddPoint(8, 8)
                .AddPoint(8, 0)
                .Build();

            //md > **Note:** Orthogonal (or rectilinear) polygon is a polygon of whose edge intersections are at right angles. When on an integer grid, each side of an orthogonal polygon is aligned with one of the axes.

            //md > **Note:** There are several ways of constructing polygons:
            //md    - `PolygonGrid2D.GetSquare(width)` for squares
            //md    - `PolygonGrid2D.GetRectangle(width, height)` for rectangles
            //md    - `PolygonGrid2DBuilder` with the `.AddPoint(x, y)` method
            //md    - or the `PolygonGrid2D(IEnumerable<Vector2Int> points)` constructor

            //md ### Doors
            //md `IDoorModeGrid2D` is an interface that specifies door positions of a given room template. The most simple *door mode* is the `SimpleDoorModeGrid2D` - it lets us specify the length of doors and how far from corners they must be. In this tutorial, we will use doors with length of 1 tile and at least 1 tile away from corners.
            
            var doorMode = new SimpleDoorModeGrid2D(doorLength: 1, cornerDistance: 1);

            //md > **Note:** There is also an additional door mode available - `ManualDoorModeGrid2D`. This mode lets you specify exactly which door positions are available. It is useful for example when we want to have doors only on the two opposite sides of a corridor.

            //md ### (Optional) Allowed transformations
            //md Optionally, it is also possible to let the algorithm apply some transformations to the room, e.g. rotate it by 90 degrees or mirror it by the X axis. The algorithm then also handles all the door positions automatically.

            var transformations = new List<TransformationGrid2D>()
            {
                TransformationGrid2D.Identity,
                TransformationGrid2D.Rotate90
            };

            //md ### Putting it all together
            //md We can now combine the *outline*, *door mode* and *allowed transformations* together to create our first room template.

            var squareRoom = new RoomTemplateGrid2D(
                squareRoomOutline,
                doorMode,
                name: "Square 8x8",
                allowedTransformations: transformations
            );

            //md We can also create a room template in-place with a single expression.

            var rectangleRoom = new RoomTemplateGrid2D(
                PolygonGrid2D.GetRectangle(6, 10),
                new SimpleDoorModeGrid2D(doorLength: 1, cornerDistance: 1),
                name: "Rectangle 6x10",
                allowedTransformations: new List<TransformationGrid2D>()
                {
                    TransformationGrid2D.Identity,
                    TransformationGrid2D.Rotate90
                }
            );

            //md ## Room description
            //md When we have our room templates ready, we need to create an instance of the `RoomDescriptionGrid2D` which describes the properties of individual rooms in the level. In this tutorial, all the rooms use the same pool of room templates, so we can create only a single room description and reuse it. However, it is also possible to use different room description for different types of rooms. For example, we may want to have a boss room and a spawn room that should use different room templates than other rooms.

            var roomDescription = new RoomDescriptionGrid2D()
            {
                RoomTemplates = new List<RoomTemplateGrid2D>() { squareRoom, rectangleRoom }
            };

            //md ## Level description
            //md The final step is to describe the structure of the level. First, we have to create an instance of the `LevelDescriptionGrid2D<TRoom>` class. For simplicity, We will use `integers` to identify individual rooms. But it is also possible to use a custom room type by using a different generic type parameter.
            
            var levelDescription = new LevelDescriptionGrid2D<int>();

            //md Next, we add individual rooms to the level description.

            levelDescription.AddRoom(0, roomDescription);
            levelDescription.AddRoom(1, roomDescription);
            levelDescription.AddRoom(2, roomDescription);
            levelDescription.AddRoom(3, roomDescription);
            levelDescription.AddRoom(4, roomDescription);

            //md And lastly, we describe how should individual rooms be connected.

            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(0, 3);
            levelDescription.AddConnection(0, 4);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            return levelDescription;
        }
    }
}