using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Doors;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    /// <summary>
    /// Represents a door mode on the 2D (integer) grid.
    /// A door mode describes which door positions are available for a room template.
    /// </summary>
    public interface IDoorModeGrid2D : IDoorMode<DoorLineGrid2D, PolygonGrid2D>
    {
        
    }
}