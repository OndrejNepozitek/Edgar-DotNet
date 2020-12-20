using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.GridPseudo3D.Doors;

namespace Edgar.GraphBasedGenerator.GridPseudo3D
{
    /// <summary>
    /// Represents a room template - an outline of a room together with all possible door positions.
    /// </summary>
    public class RoomTemplateGridPseudo3D : RoomTemplateGrid2D
    {
        public new IDoorModeGridPseudo3D Doors { get; }

        public RoomTemplateGridPseudo3D(PolygonGrid2D outline, IDoorModeGridPseudo3D doors, string name = null, RoomTemplateRepeatMode? repeatMode = null, List<TransformationGrid2D> allowedTransformations = null) 
        : base(outline, null, name, repeatMode, allowedTransformations)
        {
            Doors = doors;
        }
    }
}