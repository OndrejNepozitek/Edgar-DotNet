using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Edgar.Geometry;

namespace Edgar.GraphBasedGenerator.Grid2D.Drawing
{
    /// <summary>
	/// Class that can export a given layout to a PNG image.
	/// </summary>
    public class DungeonDrawer<TRoom> : DungeonDrawerBase
	{
        private Pen outlinePen;
        private Pen shadePen;

        /// <summary>
        /// Draws a given layout and saves it to a PNG file.
        /// </summary>
        /// <param name="layout">Layout to be drawn.</param>
        /// <param name="path">Path to the file.</param>
        /// <param name="options">Options. See <see cref="DungeonDrawerOptions"/>.</param>
        public void DrawLayoutAndSave(LayoutGrid2D<TRoom> layout, string path, DungeonDrawerOptions options)
        {
            var bitmap = DrawLayout(layout, options);
            bitmap.Save(path);
        }

        /// <summary>
        /// Draws a given layout and returns a bitmap with the drawing.
        /// </summary>
        /// <param name="layout">Layout to be drawn.</param>
        /// <param name="options">Options. See <see cref="DungeonDrawerOptions"/>.</param>
        public Bitmap DrawLayout(LayoutGrid2D<TRoom> layout, DungeonDrawerOptions options)
        {
            var roomOutlines = layout.Rooms.Select(x => x.Outline + x.Position).ToList();
            var boundingBox = DrawingUtils.GetBoundingBox(roomOutlines);
            var (width, height, scale) = DrawingUtils.GetSize(boundingBox, options.Width, options.Height, options.Scale, options.PaddingAbsolute, options.PaddingPercentage);
            var offset = DrawingUtils.GetOffset(boundingBox, width, height, scale);

            bitmap = new Bitmap(width, height);
            graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            using (SolidBrush brush = new SolidBrush(options.BackgroundColor))
            {
                graphics.FillRectangle(brush, 0, 0, width, height);
            }

            outlinePen = new Pen(Color.FromArgb(50, 50, 50), 0.2f)
            {
                EndCap = LineCap.Round,
                StartCap = LineCap.Round
            };

            shadePen = new Pen(Color.FromArgb(204, 206, 206), 1.3f)
            {
                EndCap = LineCap.Round,
                StartCap = LineCap.Round
            };

            var rooms = layout.Rooms.ToList();

            graphics.TranslateTransform(offset.X, offset.Y);
            graphics.ScaleTransform(scale, scale);

            if (options.EnableShading)
            {
                foreach (var room in rooms)
                {
                    DrawShading(GetOutline(room.Outline, room.Doors.Select(x => x.DoorLine).ToList(), room.Position), shadePen);
                }
            }

            if (options.EnableHatching)
            {
                var hatchingUsedPoints = new List<Tuple<RectangleGrid2D, List<Vector2>>>();
                foreach (var room in rooms)
                {
                    DrawHatching(room.Outline + room.Position, hatchingUsedPoints, options.HatchingClusterOffset, options.HatchingLength);
                }
            }

            foreach (var room in rooms)
            {
                DrawRoomBackground(room.Outline + room.Position, options.RoomBackgroundColor);

                if (options.EnableGridLines)
                {
                    DrawGrid(room.Outline + room.Position);
                }

                DrawOutline(room.Outline + room.Position, GetOutline(room.Outline, room.Doors.Select(x => x.DoorLine).ToList(), room.Position), outlinePen);
            }

            foreach (var room in rooms)
            {
                if (options.ShowRoomNames && !room.IsCorridor)
                {
                    DrawTextOntoPolygon(room.Outline + room.Position, room.Room.ToString(), options.FontSize);
                }
            }

            outlinePen.Dispose();
            shadePen.Dispose();

            return bitmap;
        }
    }
}