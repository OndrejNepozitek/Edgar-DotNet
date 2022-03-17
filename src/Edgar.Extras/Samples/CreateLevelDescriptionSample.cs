using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;

namespace Edgar.Extras.Samples
{
    public static class CreateLevelDescriptionSample
    {
        public static LevelDescriptionGrid2D<int> GetLevelDescription()
        {
            // Prepare room templates
            var doorMode = new SimpleDoorModeGrid2D(1, 1);

            var squareRoom = new RoomTemplateGrid2D(
                new PolygonGrid2DBuilder()
                    .AddPoint(0, 0)
                    .AddPoint(0, 8)
                    .AddPoint(8, 8)
                    .AddPoint(8, 0)
                    .Build(),
                doorMode
            );

            var rectangleRoom = new RoomTemplateGrid2D(
                PolygonGrid2D.GetRectangle(6, 10),
                doorMode,
                allowedTransformations: new List<TransformationGrid2D>()
                    {TransformationGrid2D.Identity, TransformationGrid2D.Rotate90}
            );

            // Create room description
            var basicRoomDescription =
                new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() {squareRoom, rectangleRoom});

            // Create map description
            var levelDescription = new LevelDescriptionGrid2D<int>();

            // Add rooms
            levelDescription.AddRoom(0, basicRoomDescription);
            levelDescription.AddRoom(1, basicRoomDescription);
            levelDescription.AddRoom(2, basicRoomDescription);
            levelDescription.AddRoom(3, basicRoomDescription);

            // Add connections
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(0, 3);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            // Add room shapes
            return levelDescription;
        }
    }
}