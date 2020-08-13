using System.Collections.Generic;
using Edgar.Legacy.Core.Doors.Interfaces;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    public class RoomTemplateGrid2D
    {
        public PolygonGrid2D Outline { get; }

        public IDoorMode DoorMode { get; }

        public string Name { get; }

        public RepeatMode? RepeatMode { get; }

        public List<Transformation> AllowedTransformations { get; }

        public RoomTemplateGrid2D(PolygonGrid2D outline, IDoorMode doorMode, string name = null, RepeatMode? repeatMode = null, List<Transformation> allowedTransformations = null)
        {
            Outline = outline;
            DoorMode = doorMode;
            Name = name ?? "Room template";
            RepeatMode = repeatMode;
            AllowedTransformations = allowedTransformations ?? new List<Transformation>() { Transformation.Identity };;
        }
    }
}