using System.Collections.Generic;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Polygons;

namespace MapGeneration.Simplified
{
    public class CachedPolygonOverlap : PolygonOverlapBase<GridPolygon>
    {
        private readonly Dictionary<GridPolygon, List<GridRectangle>> decompositions = new Dictionary<GridPolygon, List<GridRectangle>>();
        private readonly GridPolygonPartitioning polygonPartitioning = new GridPolygonPartitioning();

        protected override List<GridRectangle> GetDecomposition(GridPolygon polygon)
        {
            if (!decompositions.TryGetValue(polygon, out var decomposition))
            {
                decomposition = polygonPartitioning.GetPartitions(polygon);
                decompositions[polygon] = decomposition;
            }
            
            return decomposition;
        }

        protected override GridRectangle GetBoundingRectangle(GridPolygon polygon)
        {
            return polygon.BoundingRectangle;
        }
    }
}