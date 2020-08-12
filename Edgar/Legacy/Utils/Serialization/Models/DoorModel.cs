namespace MapGeneration.Utils.Serialization.Models
{
	using GeneralAlgorithms.DataStructures.Common;

	public class DoorModel<TNode>
	{
		public TNode Node { get; set; }

		public Vector2Int From { get; set; }

		public Vector2Int To { get; set; }
	}
}