using System.Collections.Generic;
using System.Linq;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public class HashSetTilemap<TPoint> : ITilemap<TPoint>
    {
        private readonly HashSet<TPoint> points = new HashSet<TPoint>();

        public bool IsEmpty(TPoint point)
        {
            return !points.Contains(point);
        }

        public void AddPoint(TPoint point)
        {
            points.Add(point);
        }

        public IEnumerable<TPoint> GetPoints()
        {
            return points.ToList();
        }
    }
}