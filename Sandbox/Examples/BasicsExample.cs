using System;
using System.Collections.Generic;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Interfaces.Core.MapDescriptions;

namespace Sandbox.Examples
{
    using GeneralAlgorithms.DataStructures.Polygons;
    using MapGeneration.Core.Doors.DoorModes;
    using MapGeneration.Core.MapDescriptions;

    public class BasicsExample : IExample
    {
        public MapDescription<int> GetMapDescription()
        {
            // Prepare room templates
            var doorMode = new SimpleDoorMode(1, 1);

            var squareRoom = new RoomTemplate(
                new GridPolygonBuilder()
                    .AddPoint(0, 0)
                    .AddPoint(0, 8)
                    .AddPoint(8, 8)
                    .AddPoint(8, 0)
                    .Build(),
                doorMode
            );

            var rectangleRoom = new RoomTemplate(
                GridPolygon.GetRectangle(6, 10),
                doorMode,
                new List<Transformation>() { Transformation.Identity, Transformation.Rotate90}
            );

            // Create room description
            var basicRoomDescription = new BasicRoomDescription(new List<IRoomTemplate>() { squareRoom, rectangleRoom });

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