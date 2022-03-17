using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Utils.Serialization.Models
{
    public class RoomModel<TNode>
    {
        public TNode Node { get; set; }

        public IList<Vector2Int> Shape { get; set; }

        public Vector2Int Position { get; set; }

        public bool IsCorridor { get; set; }

        public IList<DoorModel<TNode>> Doors { get; set; }
    }
}