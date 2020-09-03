using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Edgar.Geometry;

namespace Edgar.GraphBasedGenerator.Grid2D.Drawing
{
    public class RoomTemplateDrawer : DungeonDrawerBase
    {
        public Bitmap DrawRoomTemplates(List<RoomTemplateGrid2D> roomTemplates, DungeonDrawerOptions options, List<Vector2Int> positions = null, bool showOrigin = false)
        {
            var configurations = GetRoomTemplateConfigurations(roomTemplates, options.Width.Value / (double) options.Height.Value, options);

            if (positions != null)
            {
                configurations = new List<RoomTemplateConfiguration>();

                for (var i = 0; i < roomTemplates.Count; i++)
                {
                    var roomTemplate = roomTemplates[i];
                    configurations.Add(new RoomTemplateConfiguration(roomTemplate, positions[i]));
                }
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
                DrawDoors(roomTemplate);
            }

            foreach (var roomTemplate in configurations)
            {
                if (options.ShowRoomNames)
                {
                    DrawTextOntoPolygon(roomTemplate.RoomTemplate.Outline + roomTemplate.Position, roomTemplate.RoomTemplate.Name, options.FontSize);
                }
            }

            if (showOrigin)
            {
                var radius = 0.33f;
                graphics.DrawEllipse(outlinePen, -radius, -radius, 2 * radius, 2 * radius);
            }

            shadePen.Dispose();
            outlinePen.Dispose();

            return bitmap;
        }
        
        public Bitmap DrawRoomTemplates(List<PolygonGrid2D> polygons, DungeonDrawerOptions options)
        {
            var outlines = polygons;
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

            graphics.TranslateTransform(offset.X, offset.Y);
            graphics.ScaleTransform(scale, scale);

            foreach (var polygon in polygons)
            {
                DrawRoomBackground(polygon);
                DrawGrid(polygon);
                DrawOutline(polygon, GetOutline(polygon, null), outlinePen);
                // DrawDoors(roomTemplate);
            }

            //foreach (var roomTemplate in configurations)
            //{
            //    if (options.ShowRoomNames)
            //    {
            //        DrawTextOntoPolygon(roomTemplate.RoomTemplate.Outline + roomTemplate.Position, roomTemplate.RoomTemplate.Name, options.FontSize);
            //    }
            //}

            shadePen.Dispose();
            outlinePen.Dispose();

            return bitmap;
        }

        protected void DrawDoors(RoomTemplateConfiguration configuration)
        {
            var roomTemplate = configuration.RoomTemplate;
            var position = configuration.Position;

            var pen2Width = 0.15f;
            var pen2Offset = 0.2f;
            var shortOffset = 0.05f;
            var color = Color.FromArgb(150, 50, 50);

            var pen1 = new Pen(color, 0.2f);
            var pen2 = new Pen(color, pen2Width);

            List<DoorLineGrid2D> doors = null;

            if (roomTemplate.Doors is SimpleDoorModeGrid2D simpleDoorMode)
            {
                doors = simpleDoorMode.GetDoors(roomTemplate.Outline + position);

                //foreach (var doorLine in doors)
                //{
                //    var from = doorLine.Line.From;
                //    var to = doorLine.Line.To + doorLine.Length * doorLine.Line.GetDirectionVector();

                //    graphics.DrawLine(pen, from.X, from.Y, to.X, to.Y);
                //}
            } else if (roomTemplate.Doors is ManualDoorModeGrid2D manualDoorMode)
            {
                doors = manualDoorMode.GetDoors(roomTemplate.Outline).Select(x => new DoorLineGrid2D(x.Line + position, x.Length, x.DoorSocket)).ToList();
            }

            if (doors != null)
            {
                foreach (var doorLine in doors)
                {
                    foreach (var fromOriginal in doorLine.Line.GetPoints())
                    {
                        var from = (Vector2) fromOriginal;
                        var to = @from + doorLine.Length * doorLine.Line.GetDirectionVector();
                        
                        var directionVector = (Vector2) doorLine.Line.GetDirectionVector();
                        var perpendicular = new Vector2(directionVector.Y, directionVector.X);

                        @from += shortOffset * directionVector;
                        to -= shortOffset * directionVector;

                        graphics.DrawLine(pen1, @from.X, @from.Y, to.X, to.Y);

                        var from1 = @from + pen2Width / 2 * directionVector + pen2Offset * perpendicular;
                        var to1 = @from + pen2Width / 2 * directionVector - pen2Offset * perpendicular;
                        graphics.DrawLine(pen2, from1.X, from1.Y, to1.X, to1.Y);

                        var from2 = to - pen2Width / 2 * directionVector + pen2Offset * perpendicular;
                        var to2 = to - pen2Width / 2 * directionVector - pen2Offset * perpendicular;
                        graphics.DrawLine(pen2, from2.X, from2.Y, to2.X, to2.Y);
                    }
                }
            }
        }

        private List<RoomTemplateConfiguration> GetRoomTemplateConfigurations(List<RoomTemplateGrid2D> roomTemplates, double expectedRatio, DungeonDrawerOptions options)
        {
            var orderedRoomTemplates = roomTemplates.OrderByDescending(x => x.Outline.BoundingRectangle.Width).ToList();
            var minDistance = 3;

            var bestRatio = double.MaxValue;
            var bestScale = double.MinValue;
            List<RoomTemplateConfiguration> bestConfigurations = null;
            List<List<RoomTemplateGrid2D>> lastRows = null;

            while (true)
            {
                int rowWidth;

                if (lastRows == null)
                {
                    rowWidth = orderedRoomTemplates[0].Outline.BoundingRectangle.Width;
                }
                else
                {
                    rowWidth = GetNextBreakPoint(lastRows);
                }

                var rows = GetRows(roomTemplates, rowWidth);
                var configurations = GetConfigurations(rows, rowWidth, minDistance);
                var ratio = GetWidthHeightRatio(configurations);
                var emptySpaceRatio = GetEmptySpaceRatio(rows, rowWidth);

                var outlines = configurations.Select(x => x.RoomTemplate.Outline + x.Position).ToList();
                var boundingBox = DrawingUtils.GetBoundingBox(outlines);
                var (width, height, scale) = DrawingUtils.GetSize(boundingBox, options.Width, options.Height, options.Scale, options.PaddingAbsolute, options.PaddingPercentage);

                if (scale > bestScale)
                {
                    bestConfigurations = configurations;
                    bestScale = scale;
                }

                //if (Math.Abs(expectedRatio - ratio) < Math.Abs(expectedRatio - bestRatio))
                //{
                //    bestConfigurations = configurations;
                //    bestRatio = ratio;
                //}

                lastRows = rows;

                if (rows.Count == 1)
                {
                    if (bestConfigurations == null)
                    {
                        bestConfigurations = configurations;
                    }

                    break;
                }
            }

            return bestConfigurations;
        }

        private double GetWidthHeightRatio(List<RoomTemplateConfiguration> configurations)
        {
            var minX = configurations.Min(x => x.RoomTemplate.Outline.BoundingRectangle.A.X + x.Position.X);
            var minY = configurations.Min(x => x.RoomTemplate.Outline.BoundingRectangle.A.Y + x.Position.Y);
            var maxX = configurations.Max(x => x.RoomTemplate.Outline.BoundingRectangle.B.X + x.Position.X);
            var maxY = configurations.Max(x => x.RoomTemplate.Outline.BoundingRectangle.B.Y + x.Position.Y);

            var width = maxX - minX;
            var height = maxY - minY;

            return width / (double) height;
        }

        private double GetEmptySpaceRatio(List<List<RoomTemplateGrid2D>> rows, int rowWidth)
        {
            var worstRatio = double.MinValue;

            foreach (var row in rows)
            {
                var width = row.Sum(x => x.Outline.BoundingRectangle.Width);
                var emptySpace = rowWidth - width;

                worstRatio = Math.Max(worstRatio, emptySpace / (double) rowWidth);
            }

            return worstRatio;
        }

        private int GetNextBreakPoint(List<List<RoomTemplateGrid2D>> rows)
        {
            var minWidth = int.MaxValue;

            for (var i = 0; i < rows.Count - 1; i++)
            {
                var row = rows[i];
                var nextRow = rows[i + 1];

                var currentWidth = row.Sum(x => x.Outline.BoundingRectangle.Width);
                var nextItemWidth = nextRow[0].Outline.BoundingRectangle.Width;
                var nextWidth = currentWidth + nextItemWidth;

                minWidth = Math.Min(minWidth, nextWidth);
            }

            return minWidth;
        }

        private List<RoomTemplateConfiguration> GetConfigurations(List<List<RoomTemplateGrid2D>> rows, int rowWidth, int minDistance)
        {
            var configurations = new List<RoomTemplateConfiguration>();
            var maxHeight = int.MinValue;

            rowWidth = rows.Max(x => x.Sum(y => y.Outline.BoundingRectangle.Width) + (x.Count - 1) * minDistance);

            var offset = new Vector2Int();
            foreach (var row in rows)
            {
                var totalWidth = row.Sum(x => x.Outline.BoundingRectangle.Width);
                var space = rowWidth - totalWidth;
                var distance = row.Count != 1 ? space / (row.Count - 1) : 0;
                var remainder = space - distance * (row.Count - 1);

                if (row == rows.Last())
                {
                    distance = minDistance;
                    remainder = 0;
                }

                if (row.Count == 1)
                {
                    offset += new Vector2Int(space / 2, 0);
                }

                for (var i = 0; i < row.Count; i++)
                {
                    var roomTemplate = row[i];
                    var outline = roomTemplate.Outline;
                    configurations.Add(new RoomTemplateConfiguration(roomTemplate,
                        -1 * roomTemplate.Outline.BoundingRectangle.A + offset));
                    offset += new Vector2Int(roomTemplate.Outline.BoundingRectangle.Width, 0);
                    maxHeight = Math.Max(maxHeight, outline.BoundingRectangle.Height);

                    offset += new Vector2Int(distance, 0);

                    if (i < remainder)
                    {
                        offset += new Vector2Int(1, 0);
                    }
                }

                offset = new Vector2Int(0, offset.Y + maxHeight + minDistance);
                maxHeight = int.MinValue;
            }

            return configurations;
        }

        private List<List<RoomTemplateGrid2D>> GetRows(List<RoomTemplateGrid2D> roomTemplates, int rowWidth)
        {
            roomTemplates = roomTemplates.OrderByDescending(x => x.Outline.BoundingRectangle.Height).ToList();
            

            var maxHeight = int.MinValue;
            var rows = new List<List<RoomTemplateGrid2D>>();
            rows.Add(new List<RoomTemplateGrid2D>());

            foreach (var roomTemplate in roomTemplates)
            {
                var outline = roomTemplate.Outline;

                if (rows.Last().Sum(x => x.Outline.BoundingRectangle.Width) + outline.BoundingRectangle.Width > rowWidth)
                {
                    maxHeight = int.MinValue;
                    rows.Add(new List<RoomTemplateGrid2D>());
                }

                // configurations.Add(new RoomTemplateConfiguration(roomTemplate, -1 * roomTemplate.Outline.BoundingRectangle.A + offset));
                rows.Last().Add(roomTemplate);
                
                maxHeight = Math.Max(maxHeight, outline.BoundingRectangle.Height);
            }

            return rows;
        }

        protected class RoomTemplateConfiguration
        {
            public RoomTemplateGrid2D RoomTemplate { get; }

            public Vector2Int Position { get; }

            public RoomTemplateConfiguration(RoomTemplateGrid2D roomTemplate, Vector2Int position)
            {
                RoomTemplate = roomTemplate;
                Position = position;
            }
        }
    }
}