using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Grid2D.Doors;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    public class RoomTemplateGrid2D
    {
        public PolygonGrid2D Outline { get; }

        public IDoorModeGrid2D Doors { get; }

        public string Name { get; }

        public RepeatMode? RepeatMode { get; }

        public List<Transformation> AllowedTransformations { get; }

        public RoomTemplateGrid2D(PolygonGrid2D outline, IDoorModeGrid2D doors, string name = null, RepeatMode? repeatMode = null, List<Transformation> allowedTransformations = null)
        {
            Outline = outline;
            Doors = doors;
            Name = name ?? "Room template";
            RepeatMode = repeatMode;
            AllowedTransformations = allowedTransformations ?? new List<Transformation>() { Transformation.Identity };;
        }
    }
}