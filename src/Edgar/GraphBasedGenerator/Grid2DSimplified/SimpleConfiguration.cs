using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Internal;
using Edgar.Legacy.Core.Configurations.Interfaces;

namespace Edgar.GraphBasedGenerator.Grid2DSimplified
{
    public class SimpleConfiguration<TRoom> : IConfiguration<RoomTemplateInstanceGrid2D, Vector2Int, TRoom>
    {
        public RoomTemplateInstanceGrid2D RoomShape { get; set; }

        public Vector2Int Position { get; set; }

        public TRoom Room { get; set; }
    }
}