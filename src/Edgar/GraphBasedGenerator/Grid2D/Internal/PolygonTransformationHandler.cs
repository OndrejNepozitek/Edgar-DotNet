using Edgar.Geometry;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal
{
    public sealed class PolygonTransformationHandler : IPolygonTransformationHandler
    {
        public PolygonGrid2D Transform(PolygonGrid2D polygon, TransformationGrid2D transformation)
        {
            return polygon.Transform(transformation);
        }
    }
}