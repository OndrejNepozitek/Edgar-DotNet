using System.Collections.Generic;
using System.Linq;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public class HashSetTilemap<TPoint, TRoom> : ITilemap<TPoint, TRoom>
    {
        private readonly Dictionary<TPoint, HashSet<TRoom>> points = new Dictionary<TPoint, HashSet<TRoom>>();

        public bool IsEmpty(TPoint point)
        {
            return !points.ContainsKey(point);
        }

        public void AddPoint(TPoint point, TRoom room)
        {
            if (points.TryGetValue(point, out var rooms))
            {
                rooms.Add(room);
            }
            else
            {
                points.Add(point, new HashSet<TRoom>() { room });
            }
        }

        public List<TRoom> GetRoomsOnTile(TPoint point)
        {
            if (points.TryGetValue(point, out var rooms))
            {
                return rooms.ToList();
            }

            return null;
        }

        public IEnumerable<TPoint> GetPoints()
        {
            return points.Keys.ToList();
        }
    }
}