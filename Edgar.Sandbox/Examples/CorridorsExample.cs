using System.Drawing.Text;
using System.Linq;
using Edgar.Geometry;
using Edgar.Legacy.Core.Doors.ManualMode;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using Edgar.Legacy.Utils;

namespace Sandbox.Examples
{
	using System.Collections.Generic;

    public class CorridorsExample : IExample
	{
        public MapDescription<int> GetMapDescription()
        {
            const bool useLongCorridors = false;
            const bool useLShapedCorridors = false;
            const bool useWideCorridors = false;

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
            
			// Basic corridor shape
			var corridorRoom1x2 = new RoomTemplate(
                PolygonGrid2D.GetRectangle(1, 2),
				new ManualDoorMode(new List<OrthogonalLine>()
				{
					new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(1, 0)),
					new OrthogonalLine(new Vector2Int(0, 2), new Vector2Int(1, 2))
				}),
                new List<TransformationGrid2D>() { TransformationGrid2D.Identity, TransformationGrid2D.Rotate90 }
			);

            var corridorRoomDescription = new CorridorRoomDescription(new List<RoomTemplate>() { corridorRoom1x2 });

            // Add longer corridor
            if (useLongCorridors)
            {
                var corridorRoom1x4 = new RoomTemplate(
                    PolygonGrid2D.GetRectangle(1, 4),
                    new ManualDoorMode(new List<OrthogonalLine>()
                    {
                        new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(1, 0)),
                        new OrthogonalLine(new Vector2Int(0, 4), new Vector2Int(1, 4))
                    }),
                    new List<TransformationGrid2D>() { TransformationGrid2D.Identity, TransformationGrid2D.Rotate90 }
                );

                corridorRoomDescription.RoomTemplates.Add(corridorRoom1x4);
            }

            // Add l-shaped corridor
            if (useLShapedCorridors)
            {
                var corridorRoomLShaped = new RoomTemplate(
                    new PolygonGrid2DBuilder()
                        .AddPoint(0, 2)
                        .AddPoint(0, 3)
                        .AddPoint(3, 3)
                        .AddPoint(3, 0)
                        .AddPoint(2, 0)
                        .AddPoint(2, 2)
                        .Build(), 
                    new ManualDoorMode(new List<OrthogonalLine>()
                    {
                        new OrthogonalLine(new Vector2Int(0, 2), new Vector2Int(0, 3)),
                        new OrthogonalLine(new Vector2Int(2, 0), new Vector2Int(3, 0))
                    }),
                    TransformationGrid2DHelper.GetAllTransformationsOld().ToList()
                );

                corridorRoomDescription.RoomTemplates.Add(corridorRoomLShaped);
            }

            // Add wide corridor
            if (useWideCorridors)
            {
                var corridorWide = new RoomTemplate(
                    new PolygonGrid2DBuilder()
                        .AddPoint(1, 0)
                        .AddPoint(1, 1)
                        .AddPoint(0, 1)
                        .AddPoint(0, 4)
                        .AddPoint(1, 4)
                        .AddPoint(1, 5)
                        .AddPoint(2, 5)
                        .AddPoint(2, 4)
                        .AddPoint(3, 4)
                        .AddPoint(3, 1)
                        .AddPoint(2, 1)
                        .AddPoint(2, 0)
                        .Build(), 
                    new ManualDoorMode(new List<OrthogonalLine>()
                    {
                        new OrthogonalLine(new Vector2Int(1, 0), new Vector2Int(2, 0)),
                        new OrthogonalLine(new Vector2Int(1, 5), new Vector2Int(2, 5))
                    }),
                    TransformationGrid2DHelper.GetAllTransformationsOld().ToList()
                );

                corridorRoomDescription.RoomTemplates.Add(corridorWide);
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
                if (true)
                {
                    if (true)
                    {
                        if (connection.From % 2 == 0 && connection.To % 2 == 0)
                        {
                            // We manually insert a new node between each neighboring nodes in the graph
                            mapDescription.AddRoom(counter, corridorRoomDescription);

                            // And instead of connecting the rooms directly, we connect them to the corridor room
                            mapDescription.AddConnection(connection.From, counter);
                            mapDescription.AddConnection(connection.To, counter);
                            counter++;
                        }
                        else
                        {
                            mapDescription.AddConnection(connection.From, connection.To);
                        }
                    }
                    else
                    {
                        var desc1 = new CorridorRoomDescription(new List<RoomTemplate>() { corridorRoomDescription.RoomTemplates[0]} );
                        var desc2 = new CorridorRoomDescription(new List<RoomTemplate>() { corridorRoomDescription.RoomTemplates[1]} );

                        // We manually insert a new node between each neighboring nodes in the graph
                        mapDescription.AddRoom(counter, connection.From % 2 == 0 && connection.To % 2 == 0 ? desc2 : desc1);

                        // And instead of connecting the rooms directly, we connect them to the corridor room
                        mapDescription.AddConnection(connection.From, counter);
                        mapDescription.AddConnection(connection.To, counter);
                        counter++;
                    }
                }
                else
                {
                    // We manually insert a new node between each neighboring nodes in the graph
                    mapDescription.AddRoom(counter, corridorRoomDescription);

                    // And instead of connecting the rooms directly, we connect them to the corridor room
                    mapDescription.AddConnection(connection.From, counter);
                    mapDescription.AddConnection(connection.To, counter);
                    counter++;
                }
            }

			return mapDescription;
		}
	}
}