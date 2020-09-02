using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public interface ITilemap<TPoint>
    {
        bool IsEmpty(TPoint point);

        void AddPoint(TPoint point);

        IEnumerable<TPoint> GetPoints();
    }
}