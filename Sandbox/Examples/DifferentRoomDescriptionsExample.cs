using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Utils;
using Sandbox.Utils;

namespace Sandbox.Examples
{
    public class DifferentRoomDescriptionsExample : IExample
	{
		public MapDescription<int> GetMapDescription()
		{
			// Create boss room template and room description
            var bossRoom = new RoomTemplate(
                new GridPolygonBuilder()
                    .AddPoint(2, 0).AddPoint(2, 1).AddPoint(1, 1).AddPoint(1, 2)
                    .AddPoint(0, 2).AddPoint(0, 7).AddPoint(1, 7).AddPoint(1, 8)
                    .AddPoint(2, 8).AddPoint(2, 9).AddPoint(7, 9).AddPoint(7, 8)
                    .AddPoint(8, 8).AddPoint(8, 7).AddPoint(9, 7).AddPoint(9, 2)
                    .AddPoint(8, 2).AddPoint(8, 1).AddPoint(7, 1).AddPoint(7, 0)
                    .Build().Scale(new IntVector2(2, 2)),
                new SimpleDoorMode(1, 1)
            );

            var bossRoomDescription = new BasicRoomDescription(new List<RoomTemplate>() { bossRoom });

            // Create basic room templates and room description
            var doorMode = new SimpleDoorMode(1, 1);

            var squareRoom = new RoomTemplate(
                GridPolygon.GetSquare(8),
                doorMode
            );

            var rectangleRoom = new RoomTemplate(
                GridPolygon.GetRectangle(6, 10),
                doorMode,
                new List<Transformation>() { Transformation.Identity, Transformation.Rotate90}
            );

            var basicRoomDescription = new BasicRoomDescription(new List<RoomTemplate>() { squareRoom, rectangleRoom });

			// Create map description
            var mapDescription = new MapDescription<int>();

            // Get graph
            var graph = GraphsDatabase.GetExample2();

            // Add boss room
            mapDescription.AddRoom(8, bossRoomDescription);

            // Add other rooms
            foreach (var vertex in graph.Vertices.Where(x => x != 8))
            {
                mapDescription.AddRoom(vertex, basicRoomDescription);
            }

            // Add connections
            foreach (var connection in graph.Edges)
            {
                mapDescription.AddConnection(connection.From, connection.To);
            }

			return mapDescription;
        }
	}
}