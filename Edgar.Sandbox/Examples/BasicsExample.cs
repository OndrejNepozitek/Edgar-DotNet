using System;
using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Sandbox.Examples
{
    public class BasicsExample : IExample
    {
        public MapDescription<int> GetMapDescription()
        {
            // Prepare room templates
            var doorMode = new SimpleDoorMode(1, 1);

            var squareRoom = new RoomTemplate(
                new PolygonGrid2DBuilder()
                    .AddPoint(0, 0)
                    .AddPoint(0, 8)
                    .AddPoint(8, 8)
                    .AddPoint(8, 0)
                    .Build(),
                doorMode
            );

            var rectangleRoom = new RoomTemplate(
                PolygonGrid2D.GetRectangle(6, 10),
                doorMode,
                new List<TransformationGrid2D>() { TransformationGrid2D.Identity, TransformationGrid2D.Rotate90}
            );

            // Create room description
            var basicRoomDescription = new BasicRoomDescription(new List<RoomTemplate>() { squareRoom, rectangleRoom });

            // Create map description
            var mapDescription = new MapDescription<int>();

            // Add rooms
            mapDescription.AddRoom(0, basicRoomDescription);
            mapDescription.AddRoom(1, basicRoomDescription);
            mapDescription.AddRoom(2, basicRoomDescription);
            mapDescription.AddRoom(3, basicRoomDescription);

            // Add connections
            mapDescription.AddConnection(0, 1);
            mapDescription.AddConnection(0, 3);
            mapDescription.AddConnection(1, 2);
            mapDescription.AddConnection(2, 3);

            // Add room shapes
            return mapDescription;
        }
    }
}