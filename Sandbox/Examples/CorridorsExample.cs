using System;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using MapGeneration.Core.MapDescriptions.Interfaces;

namespace Sandbox.Examples
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core.Doors.DoorModes;
	using MapGeneration.Core.MapDescriptions;
	using MapGeneration.Utils;
	using Utils;

	public class CorridorsExample : IExample
	{
		public MapDescription<int> GetMapDescription()
		{
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

            var basicRoomDescription = new BasicRoomDescription(new List<IRoomTemplate>() { squareRoom, rectangleRoom });
            
			// Basic corridor shape
			var corridorRoom1x2 = new RoomTemplate(
                GridPolygon.GetRectangle(1, 2),
				new ManualDoorMode(new List<OrthogonalLine>()
				{
					new OrthogonalLine(new IntVector2(0, 0), new IntVector2(1, 0)),
					new OrthogonalLine(new IntVector2(0, 2), new IntVector2(1, 2))
				}),
                new List<Transformation>() { Transformation.Identity, Transformation.Rotate90 }
			);

            var corridorRoomDescription = new CorridorRoomDescription(new List<IRoomTemplate>() { corridorRoom1x2 });

            // Add longer corridor
            if (false)
            {
                var corridorRoom1x4 = new RoomTemplate(
                    GridPolygon.GetRectangle(1, 4),
                    new ManualDoorMode(new List<OrthogonalLine>()
                    {
                        new OrthogonalLine(new IntVector2(0, 0), new IntVector2(1, 0)),
                        new OrthogonalLine(new IntVector2(0, 4), new IntVector2(1, 4))
                    }),
                    new List<Transformation>() { Transformation.Identity, Transformation.Rotate90 }
                );

                corridorRoomDescription.RoomTemplates.Add(corridorRoom1x4);
            }

            // Add l-shaped corridor
            if (true)
            {
                var corridorRoomLShaped = new RoomTemplate(
                    new GridPolygonBuilder()
                        .AddPoint(0, 2)
                        .AddPoint(0, 3)
                        .AddPoint(3, 3)
                        .AddPoint(3, 0)
                        .AddPoint(2, 0)
                        .AddPoint(2, 2)
                        .Build(), 
                    new ManualDoorMode(new List<OrthogonalLine>()
                    {
                        new OrthogonalLine(new IntVector2(0, 2), new IntVector2(0, 3)),
                        new OrthogonalLine(new IntVector2(2, 0), new IntVector2(3, 0))
                    }),
                    TransformationHelper.GetAllTransformations().ToList()
                );

                corridorRoomDescription.RoomTemplates.Add(corridorRoomLShaped);
            }

            // Create map description
            var mapDescription = new MapDescription<int>();
            var graph = GraphsDatabase.GetExample1();

            // Add non-corridor rooms
            foreach (var room in graph.Vertices)
            {
                mapDescription.AddRoom(room, basicRoomDescription);
            }

            // We need to somehow identify our corridor rooms
            // Here we simply number them and keep track which was the last used number
            var counter = graph.VerticesCount;

            foreach (var connection in graph.Edges)
            {
                // We manually insert a new node between each neighboring nodes in the graph
                mapDescription.AddRoom(counter, corridorRoomDescription);

                // And instead of connecting the rooms directly, we connect them to the corridor room
                mapDescription.AddConnection(connection.From, counter);
                mapDescription.AddConnection(connection.To, counter);
                counter++;
            }

			return mapDescription;
		}
	}
}