using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Common.Corridors;
using Edgar.Graphs;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public class CorridorsPathfindingHandlerGrid2D<TLayout, TRoom, TConfiguration> : ICorridorsHandler<TLayout, TRoom>
        where TLayout : ILayout<TRoom, TConfiguration>, ISmartCloneable<TLayout> // TODO: is this necessary?
        where TConfiguration : IConfiguration<RoomTemplateInstanceGrid2D, Vector2Int, TRoom>, new()
    {
        private readonly ILevelDescription<TRoom> levelDescription;
        private readonly IGraph<TRoom> graph;
        private int aliasCounter;
        private readonly CorridorsPathfindingConfiguration configuration;
        private readonly int minimumRoomDistance;
        private readonly RectangleGrid2D corridorShape;
        private readonly CorridorsPathfinding<TLayout, TRoom, TConfiguration> corridorsPathfinding;

        public CorridorsPathfindingHandlerGrid2D(ILevelDescription<TRoom> levelDescription, int aliasCounter, CorridorsPathfindingConfiguration configuration, int minimumRoomDistance)
        {
            this.levelDescription = levelDescription;
            this.aliasCounter = aliasCounter;
            this.configuration = configuration;
            this.minimumRoomDistance = configuration.MinimumRoomDistance ?? minimumRoomDistance;
            graph = levelDescription.GetGraph();
            this.corridorShape = new RectangleGrid2D(new Vector2Int(0, 0), new Vector2Int(configuration.CorridorWidth, configuration.CorridorHeight));
            corridorsPathfinding = new CorridorsPathfinding<TLayout, TRoom, TConfiguration>(configuration.CorridorWidth, configuration.CorridorHeight, minimumRoomDistance, configuration.HorizontalDoor, configuration.VerticalDoor);
        }

        public bool AddCorridors(TLayout layout, IEnumerable<TRoom> chain)
        {
            var clone = layout.SmartClone();
            var corridors = chain.Where(x => levelDescription.GetRoomDescription(x).IsCorridor).ToList();

            if (AddCorridors(clone, corridors))
            {
                foreach (var corridor in corridors)
                {
                    clone.GetConfiguration(corridor, out var configuration);
                    layout.SetConfiguration(corridor, configuration);
                }

                // Console.WriteLine("Success");
                return true;
            }

            // Console.WriteLine("Fail");
            return false;
        }

        private bool AddCorridors(TLayout layout, List<TRoom> corridorRooms)
        {
            var tilemap = new CorridorsPathfindingTilemap<TRoom,TConfiguration>(corridorShape, minimumRoomDistance);
            var tilemapWithoutRoomDistance = new CorridorsPathfindingTilemap<TRoom,TConfiguration>(corridorShape, 0);

            foreach (var configuration in layout.GetAllConfigurations())
            {
                tilemap.AddRoom(configuration);
                tilemapWithoutRoomDistance.AddRoom(configuration);
            }

            foreach (var corridorRoom in corridorRooms)
            {
                var neighbors = graph.GetNeighbours(corridorRoom).ToList();
                var from = neighbors[0];
                var to = neighbors[1];

                var pathfindingResult = corridorsPathfinding.FindPath(layout, from, to, tilemap, tilemapWithoutRoomDistance);

                if (pathfindingResult.IsSuccessful == false)
                {
                    return false;
                }

                var roomTemplate = new RoomTemplateGrid2D(pathfindingResult.Outline, new ManualDoorModeGrid2D(new List<DoorGrid2D>()
                {
                    pathfindingResult.DoorFrom,
                    pathfindingResult.DoorTo
                }));

                var configuration = new TConfiguration()
                {
                    Position = new Vector2Int(),
                    Room =  corridorRoom, 
                    RoomShape = new RoomTemplateInstanceGrid2D(roomTemplate, roomTemplate.Outline, roomTemplate.Doors.GetDoors(roomTemplate.Outline), new List<TransformationGrid2D>() { TransformationGrid2D.Identity })
                    {
                        RoomShapeAlias = new IntAlias<PolygonGrid2D>(aliasCounter++, roomTemplate.Outline)
                    }
                };

                layout.SetConfiguration(corridorRoom, configuration);

                tilemap.AddRoom(configuration);
                tilemapWithoutRoomDistance.AddRoom(configuration);
            }

            return true;
        }
    }
}