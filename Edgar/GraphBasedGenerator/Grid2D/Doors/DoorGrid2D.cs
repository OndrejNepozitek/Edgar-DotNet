using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.GraphBasedGenerator.Grid2D.Doors
{
    public class DoorGrid2D
    {
        public Vector2Int From { get; }

        public Vector2Int To { get; }

        public IDoorSocket Socket { get; }

        public DoorGrid2D(Vector2Int from, Vector2Int to, IDoorSocket socket = null)
        {
            From = from;
            To = to;
            Socket = socket;
        }
    }
}