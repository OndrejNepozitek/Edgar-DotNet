using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Utils.Serialization.Models
{
    public class DoorModel<TNode>
	{
		public TNode Node { get; set; }

		public Vector2Int From { get; set; }

		public Vector2Int To { get; set; }
	}
}