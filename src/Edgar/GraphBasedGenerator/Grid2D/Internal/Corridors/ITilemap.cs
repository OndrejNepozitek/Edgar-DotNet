using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public interface ITilemap<TPoint, TRoom> : ITilemap<TPoint>
    {
        void AddPoint(TPoint point, TRoom room);

        List<TRoom> GetRoomsOnTile(TPoint point);
    }

    public interface ITilemap<TPoint>
    {
        bool IsEmpty(TPoint point);

        IEnumerable<TPoint> GetPoints();
    }
}