using Edgar.Geometry;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    public interface IPolygonTransformationHandler
    {
        PolygonGrid2D Transform(PolygonGrid2D polygon, TransformationGrid2D transformation);
    }
}