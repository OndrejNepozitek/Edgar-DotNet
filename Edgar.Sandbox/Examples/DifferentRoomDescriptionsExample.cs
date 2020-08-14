using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using Edgar.Legacy.Utils;
using Sandbox.Utils;

namespace Sandbox.Examples
{
    public class DifferentRoomDescriptionsExample : IExample
	{
		public MapDescription<int> GetMapDescription()
		{
			// Create boss room template and room description
            var bossRoom = new RoomTemplate(
                new PolygonGrid2DBuilder()
                    .AddPoint(2, 0).AddPoint(2, 1).AddPoint(1, 1).AddPoint(1, 2)
                    .AddPoint(0, 2).AddPoint(0, 7).AddPoint(1, 7).AddPoint(1, 8)
                    .AddPoint(2, 8).AddPoint(2, 9).AddPoint(7, 9).AddPoint(7, 8)
                    .AddPoint(8, 8).AddPoint(8, 7).AddPoint(9, 7).AddPoint(9, 2)
                    .AddPoint(8, 2).AddPoint(8, 1).AddPoint(7, 1).AddPoint(7, 0)
                    .Build().Scale(new Vector2Int(2, 2)),
                new SimpleDoorMode(1, 1)
            );

            var bossRoomDescription = new BasicRoomDescription(new List<RoomTemplate>() { bossRoom });

            // Create basic room templates and room description
            var doorMode = new SimpleDoorMode(1, 1);

            var squareRoom = new RoomTemplate(
                PolygonGrid2D.GetSquare(8),
                doorMode
            );

            var rectangleRoom = new RoomTemplate(
                PolygonGrid2D.GetRectangle(6, 10),
                doorMode,
                new List<TransformationGrid2D>() { TransformationGrid2D.Identity, TransformationGrid2D.Rotate90}
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