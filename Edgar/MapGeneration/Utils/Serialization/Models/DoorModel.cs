namespace MapGeneration.Utils.Serialization.Models
{
	using GeneralAlgorithms.DataStructures.Common;

	public class DoorModel<TNode>
	{
		public TNode Node { get; set; }

		public IntVector2 From { get; set; }

		public IntVector2 To { get; set; }
	}
}