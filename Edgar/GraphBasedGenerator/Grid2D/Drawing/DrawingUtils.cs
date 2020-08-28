using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;

namespace Edgar.GraphBasedGenerator.Grid2D.Drawing
{
    public static class DrawingUtils
    {
        public static Vector2 GetOffset(RectangleGrid2D boundingBox, int targetWidth, int targetHeight, float scale)
        {
            var centerX = scale * (boundingBox.A.X + boundingBox.B.X) / 2;
            var centerY = scale * (boundingBox.A.Y + boundingBox.B.Y) / 2;

            return new Vector2((targetWidth / 2f - centerX), (targetHeight / 2f - centerY));
        }

        public static float GetScale(RectangleGrid2D boundingBox, int targetWidth, int targetHeight, float borderSize)
        {
            var scale = (targetWidth - 2 * borderSize) / boundingBox.Width;

            if (scale * boundingBox.Height > (targetHeight - borderSize))
            {
                scale = (targetHeight - 2 * borderSize) / boundingBox.Height;
            }

            return scale;
        }

        public static float GetWidthHeightRatio(RectangleGrid2D boundingBox)
        {
            return boundingBox.Width / (float) boundingBox.Height;
        }

        public static RectangleGrid2D GetBoundingBox(List<PolygonGrid2D> polygons)
        {
            var points = polygons.SelectMany(x => x.GetPoints()).ToList();

            var minx = points.Min(x => x.X);
            var miny = points.Min(x => x.Y);
            var maxx = points.Max(x => x.X);
            var maxy = points.Max(x => x.Y);

            return new RectangleGrid2D(new Vector2Int(minx, miny), new Vector2Int(maxx, maxy));
        }

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

            var padding = paddingAbsolute ?? paddingPercentage * targetWidth.Value;

            if (targetScale == null)
            {
                targetScale = GetScale(boundingBox, targetWidth.Value, targetHeight.Value, padding);
            }

            return (targetWidth.Value, targetHeight.Value, targetScale.Value);
        }

        public class SizeInfo
        {
            public int? Width { get; set; }

            public int? Height { get; set; }

            public float? Scale { get; set; }
        }
    }
}