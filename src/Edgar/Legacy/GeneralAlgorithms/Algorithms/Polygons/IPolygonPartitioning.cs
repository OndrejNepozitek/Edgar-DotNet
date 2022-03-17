using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons
{
    /// <summary>
    /// Represents algorithms that can decompose a given polygon into rectangular partitions that 
    /// do not overlap and cover the whole polygon.
    /// </summary>
    public interface IPolygonPartitioning
    {
        /// <summary>
        /// Gets partitions of a given polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        List<RectangleGrid2D> GetPartitions(PolygonGrid2D polygon);
    }
}