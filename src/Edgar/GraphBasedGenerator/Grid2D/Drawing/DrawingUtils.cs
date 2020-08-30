using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;

namespace Edgar.GraphBasedGenerator.Grid2D.Drawing
{
    /// <summary>
    /// Utility class for drawing layouts.
    /// </summary>
    public static class DrawingUtils
    {
        /// <summary>
        /// Computes the offset that must be applied to the bounding box in order to center it to a given target with and height.
        /// </summary>
        public static Vector2 GetOffset(RectangleGrid2D boundingBox, int targetWidth, int targetHeight, float scale)
        {
            var centerX = scale * (boundingBox.A.X + boundingBox.B.X) / 2;
            var centerY = scale * (boundingBox.A.Y + boundingBox.B.Y) / 2;

            return new Vector2((targetWidth / 2f - centerX), (targetHeight / 2f - centerY));
        }

        /// <summary>
        /// Computes the scale that must be applied to the bounding box in order to fit a given target width, target height and padding.
        /// </summary>
        public static float GetScale(RectangleGrid2D boundingBox, int targetWidth, int targetHeight, float padding)
        {
            var scale = (targetWidth - 2 * padding) / boundingBox.Width;

            if (scale * boundingBox.Height > (targetHeight - 2 * padding))
            {
                scale = (targetHeight - 2 * padding) / boundingBox.Height;
            }

            return scale;
        }

        /// <summary>
        /// Computes the width/height ratio of the bounding box.
        /// </summary>
        public static float GetWidthHeightRatio(RectangleGrid2D boundingBox)
        {
            return boundingBox.Width / (float) boundingBox.Height;
        }

        /// <summary>
        /// Computes the bounding box of a given list of polygons.
        /// </summary>
        public static RectangleGrid2D GetBoundingBox(List<PolygonGrid2D> polygons)
        {
            var points = polygons.SelectMany(x => x.GetPoints()).ToList();

            var minx = points.Min(x => x.X);
            var miny = points.Min(x => x.Y);
            var maxx = points.Max(x => x.X);
            var maxy = points.Max(x => x.Y);

            return new RectangleGrid2D(new Vector2Int(minx, miny), new Vector2Int(maxx, maxy));
        }

        /// <summary>
        /// Computes the final width, height and scale of a given bounding box.
        /// </summary>
        public static (int width, int height, float scale) GetSize(RectangleGrid2D boundingBox, int? targetWidth, int? targetHeight, float? targetScale, int? paddingAbsolute, float paddingPercentage)
        {
            var ratio = GetWidthHeightRatio(boundingBox);

            if (targetWidth == null && targetHeight != null)
            {
                targetWidth = (int) (targetHeight * ratio);
            }
            else if (targetWidth != null && targetHeight == null)
            {
                targetHeight = (int) (targetWidth / ratio);
            } 
            else if (targetWidth == null && targetHeight == null)
            {
                var scale = targetScale ?? 1;

                if (paddingAbsolute.HasValue)
                {
                    targetWidth = (int) (scale * boundingBox.Width + paddingAbsolute);
                    targetHeight = (int) (boundingBox.Height + paddingAbsolute);
                }
                else
                {
                    targetWidth = (int) (1 / (1 - 2 * paddingPercentage) * scale * boundingBox.Width);
                    targetHeight = (int) (1 / (1 - 2 * paddingPercentage) * scale * boundingBox.Height);
                }
            }

            var padding = paddingAbsolute ?? paddingPercentage * Math.Min(targetWidth.Value, targetHeight.Value);

            if (targetScale == null)
            {
                targetScale = GetScale(boundingBox, targetWidth.Value, targetHeight.Value, padding);
            }

            return (targetWidth.Value, targetHeight.Value, targetScale.Value);
        }
    }
}