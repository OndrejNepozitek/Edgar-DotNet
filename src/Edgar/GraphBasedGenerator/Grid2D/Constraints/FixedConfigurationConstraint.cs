using Edgar.Geometry;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    public class FixedConfigurationConstraint<TRoom> : IGeneratorConstraintGrid2D<TRoom>
    {
        // TODO: how to make it nullable?
        public TRoom Room { get; set; }

        public Vector2Int? Position { get; set; }

        public RoomTemplateGrid2D RoomTemplate { get; set; }
    }
}