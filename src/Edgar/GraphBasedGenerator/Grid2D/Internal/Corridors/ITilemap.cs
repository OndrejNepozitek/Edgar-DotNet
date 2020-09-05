using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public interface ITilemap<TPoint, TRoom>
    {
        bool IsEmpty(TPoint point);

        void AddPoint(TPoint point, TRoom room);

        List<TRoom> GetRoomsOnTile(TPoint point);

        IEnumerable<TPoint> GetPoints();
    }

    public interface ITilemap<TPoint>
    {
        bool IsEmpty(TPoint point);

        void AddPoint(TPoint point);

        IEnumerable<TPoint> GetPoints();
    }
}