using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Edgar.Geometry;
using Edgar.Graphs;

namespace Edgar.GraphBasedGenerator.Grid2D.Drawing
{
    public class GraphDrawer<TRoom> : DungeonDrawerBase
    {
        public Bitmap DrawGraph(LevelDescriptionGrid2D<TRoom> levelDescription, Dictionary<TRoom, Vector2Int> positions, DungeonDrawerOptions options)
        {
            var configurations = GetConfigurations(positions);

            var outlines = configurations.Select(x => x.Outline + x.Position).ToList();
            var boundingBox = DrawingUtils.GetBoundingBox(outlines);
            var (width, height, scale) = DrawingUtils.GetSize(boundingBox, options.Width, options.Height, options.Scale, options.PaddingAbsolute, options.PaddingPercentage);
            var offset = DrawingUtils.GetOffset(boundingBox, width, height, scale);

            bitmap = new Bitmap(width, height);
            graphics = Graphics.FromImage(bitmap);

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(248, 248, 244)))
            {
                graphics.FillRectangle(brush, 0, 0, width, height);
            }
            
            var outlinePen = new Pen(Color.FromArgb(50, 50, 50), 0.25f)
            {
                EndCap = LineCap.Round,
                StartCap = LineCap.Round
            };

            var shadePen = new Pen(Color.FromArgb(204, 206, 206), 1.3f)
            {
                EndCap = LineCap.Round,
                StartCap = LineCap.Round
            };

            graphics.TranslateTransform(offset.X, offset.Y);
            graphics.ScaleTransform(scale, scale);


            foreach (var configuration in configurations)
            {
                DrawShading(GetOutline(configuration.Outline, null, configuration.Position), shadePen);
            }

            //var hatchingUsedPoints = new List<Vector2>();
            //foreach (var configuration in configurations)
            //{
            //    DrawHatching(configuration.Outline + configuration.Position, hatchingUsedPoints, options.HatchingClusterOffset, options.HatchingLength);
            //}

            DrawEdges(levelDescription.GetGraphWithoutCorridors(), configurations, outlinePen);

            foreach (var configuration in configurations)
            {
                DrawRoomBackground(configuration.Outline + configuration.Position);
                DrawGrid(configuration.Outline + configuration.Position);
                DrawOutline(configuration.Outline + configuration.Position, GetOutline(configuration.Outline, null, configuration.Position), outlinePen);
            }

            foreach (var configuration in configurations)
            {
                if (options.ShowRoomNames)
                {
                    DrawTextOntoPolygon(configuration.Outline + configuration.Position, configuration.Room.ToString(), options.FontSize);
                }
            }

            shadePen.Dispose();
            outlinePen.Dispose();

            return bitmap;
        }

        private void DrawEdges(IGraph<TRoom> graph, List<RoomConfiguration> configurations, Pen pen)
        {
            foreach (var edge in graph.Edges)
            {
                var configuration1 = configurations.Single(x => x.Room.Equals(edge.From));
                var configuration2 = configurations.Single(x => x.Room.Equals(edge.To));

                var from = 0.5f * (Vector2)(configuration1.Outline.BoundingRectangle.A + configuration1.Outline.BoundingRectangle.B) + configuration1.Position;
                var to = 0.5f * (Vector2)(configuration2.Outline.BoundingRectangle.A + configuration2.Outline.BoundingRectangle.B) + configuration2.Position;

                graphics.DrawLine(pen, from.X, from.Y, to.X, to.Y);
            }
        }

        private List<RoomConfiguration> GetConfigurations(Dictionary<TRoom, Vector2Int> positions)
        {
            var configurations = new List<RoomConfiguration>();
            var outline = PolygonGrid2D.GetRectangle(7,7);
            var scale = 9;

            foreach (var pair in positions)
            {
                var room = pair.Key;
                var position = pair.Value;

                configurations.Add(new RoomConfiguration(room, scale * position, outline));
            }

            return configurations;
        }

        private class RoomConfiguration
        {
            public TRoom Room { get; }

            public Vector2Int Position { get; }

            public PolygonGrid2D Outline { get; }

            public RoomConfiguration(TRoom room, Vector2Int position, PolygonGrid2D outline)
            {
                Room = room;
                Position = position;
                Outline = outline;
            }
        }
    }
}