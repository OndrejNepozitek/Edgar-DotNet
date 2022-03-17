using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons
{
    /// <summary>
    /// Computes polygon overlap by fast caching with int aliases.
    /// </summary>
    public class FastPolygonOverlap : PolygonOverlapBase<IntAlias<PolygonGrid2D>>
    {
        private readonly List<List<RectangleGrid2D>> decompositions = new List<List<RectangleGrid2D>>();
        private readonly GridPolygonPartitioning polygonPartitioning = new GridPolygonPartitioning();

        protected override List<RectangleGrid2D> GetDecomposition(IntAlias<PolygonGrid2D> polygon)
        {
            var alias = polygon.Alias;

            if (alias >= decompositions.Count)
            {
                while (alias >= decompositions.Count)
                {
                    decompositions.Add(null);
                }
            }

            var decomposition = decompositions[alias];

            if (decomposition == null)
            {
                decomposition = polygonPartitioning.GetPartitions(polygon.Value);
                decompositions[alias] = decomposition;
            }

            return decomposition;
        }

        protected override RectangleGrid2D GetBoundingRectangle(IntAlias<PolygonGrid2D> polygon)
        {
            return polygon.Value.BoundingRectangle;
        }
    }
}