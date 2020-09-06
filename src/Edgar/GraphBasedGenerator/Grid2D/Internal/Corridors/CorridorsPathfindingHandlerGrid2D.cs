using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Common.Corridors;
using Edgar.GraphBasedGenerator.Grid2D.Drawing;
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
            corridorsPathfinding = new CorridorsPathfinding<TLayout, TRoom, TConfiguration>(configuration.CorridorWidth, configuration.CorridorHeight, minimumRoomDistance, configuration.HorizontalDoor, configuration.VerticalDoor, configuration.MaximumPathCost);
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

    public class CustomDrawer<TRoom> : RoomTemplateDrawer
        {
            public Bitmap DrawRoomTemplates(List<PolygonGrid2D> roomTemplates, DungeonDrawerOptions options,
                List<Vector2Int> positions, Dictionary<Vector2Int, int> costs, ITilemap<Vector2Int, TRoom> tilemap)
            {
            var configurations = new List<RoomTemplateConfiguration>();

            for (var i = 0; i < roomTemplates.Count; i++)
            {
                var roomTemplate = roomTemplates[i];
                configurations.Add(new RoomTemplateConfiguration(new RoomTemplateGrid2D(roomTemplate, null), positions[i]));
            }

            var outlines = configurations.Select(x => x.RoomTemplate.Outline + x.Position).ToList();
            var boundingBox = DrawingUtils.GetBoundingBox(outlines);
            var (width, height, scale) = DrawingUtils.GetSize(boundingBox, options.Width, options.Height, options.Scale, options.PaddingAbsolute, options.PaddingPercentage);
            var offset = DrawingUtils.GetOffset(boundingBox, width, height, scale);

            bitmap = new Bitmap(width, height);
            graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(248, 248, 244)))
            {
                graphics.FillRectangle(brush, 0, 0, width, height);
            }
            
            var outlinePen = new Pen(Color.FromArgb(50, 50, 50), 0.2f)
            {
                EndCap = LineCap.Round,
                StartCap = LineCap.Round
            };

            var shadePen = new Pen(Color.FromArgb(204, 206, 206), 1.3f)
            {
                EndCap = LineCap.Round,
                StartCap = LineCap.Round
            };

            graphics.TranslateTransform(offset.X, height - offset.Y);
            graphics.ScaleTransform(scale, scale);
            graphics.ScaleTransform(1, -1);

            if (options.EnableShading)
            {
                foreach (var roomTemplate in configurations)
                {
                    DrawShading(GetOutline(roomTemplate.RoomTemplate.Outline, null, roomTemplate.Position), shadePen);
                }
            }

            if (options.EnableHatching)
            {
                var hatchingUsedPoints = new List<Tuple<RectangleGrid2D, List<Vector2>>>();
                foreach (var roomTemplate in configurations)
                {
                    DrawHatching(roomTemplate.RoomTemplate.Outline + roomTemplate.Position, hatchingUsedPoints, options.HatchingClusterOffset, options.HatchingLength);
                }
            }

            foreach (var roomTemplate in configurations)
            {
                DrawRoomBackground(roomTemplate.RoomTemplate.Outline + roomTemplate.Position);
                DrawGrid(roomTemplate.RoomTemplate.Outline + roomTemplate.Position);
                DrawOutline(roomTemplate.RoomTemplate.Outline + roomTemplate.Position, GetOutline(roomTemplate.RoomTemplate.Outline, null, roomTemplate.Position), outlinePen);
                // DrawDoors(roomTemplate);
            }

            foreach (var roomTemplate in configurations)
            {
                if (options.ShowRoomNames)
                {
                    DrawTextOntoPolygon(roomTemplate.RoomTemplate.Outline + roomTemplate.Position, roomTemplate.RoomTemplate.Name, options.FontSize);
                }
            }

            if (true)
            {
                var radius = 0.33f;
                graphics.DrawEllipse(outlinePen, -radius, -radius, 2 * radius, 2 * radius);
            }

            if (tilemap != null)
            {
                var radius = 0.05f;

                var tilemapPen = new Pen(Color.Red, 0.2f)
                {
                    EndCap = LineCap.Round,
                    StartCap = LineCap.Round
                };

                foreach (var point in tilemap.GetPoints())
                {
                    graphics.DrawEllipse(tilemapPen, point.X - radius, point.Y - radius, 2 * radius, 2 * radius);
                }
            }

            if (costs != null)
            {
                var oldTransform = graphics.Transform;
                
                using (var font = new Font("Baskerville Old Face", 0.4f, FontStyle.Regular, GraphicsUnit.Pixel))
                {
                    foreach (var pair in costs)
                    {
                        var position = pair.Key;
                        var cost = pair.Value;

                        var rect = new RectangleF(
                            -0.25f,
                            -0.25f,
                            0.75f,
                            0.75f
                        );

                        var sf = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };

                        graphics.TranslateTransform(position.X, position.Y);
                        graphics.ScaleTransform(1, -1);
                        
                        graphics.DrawString(cost.ToString(), font, Brushes.Black, rect, sf);

                        graphics.Transform = oldTransform;
                    }
                }

                
            }


            shadePen.Dispose();
            outlinePen.Dispose();

            return bitmap;
            }
        }
}