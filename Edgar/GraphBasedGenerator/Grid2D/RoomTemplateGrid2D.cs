using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    public class RoomTemplateGrid2D
    {
        public PolygonGrid2D Outline { get; }

        public IDoorModeGrid2D Doors { get; }

        public string Name { get; }

        public RoomTemplateRepeatMode? RepeatMode { get; }

        public List<TransformationGrid2D> AllowedTransformations { get; }

        public RoomTemplateGrid2D(PolygonGrid2D outline, IDoorModeGrid2D doors, string name = null, RoomTemplateRepeatMode? repeatMode = null, List<TransformationGrid2D> allowedTransformations = null)
        {
            Outline = outline;
            Doors = doors;
            Name = name ?? "Room template";
            RepeatMode = repeatMode;
            AllowedTransformations = allowedTransformations ?? new List<TransformationGrid2D>() { TransformationGrid2D.Identity };;
        }
    }
}