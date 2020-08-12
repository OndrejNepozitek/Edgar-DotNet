namespace MapGeneration.Utils.Serialization.Models
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;

	public class RoomModel<TNode>
	{
		public TNode Node { get; set; }

		public IList<Vector2Int> Shape { get; set; }

		public Vector2Int Position { get; set; }

		public bool IsCorridor { get; set; }

		public IList<DoorModel<TNode>> Doors { get; set; }
	}
}