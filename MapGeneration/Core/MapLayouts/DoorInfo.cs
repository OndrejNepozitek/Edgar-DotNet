namespace MapGeneration.Core.MapLayouts
{
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces.Core.MapLayouts;

	/// <inheritdoc />
	public class DoorInfo<TNode> : IDoorInfo<TNode>
	{
		public TNode Node { get; }

		public OrthogonalLine DoorLine { get; }

		public DoorInfo(TNode node, OrthogonalLine doorLine)
		{
			Node = node;
			DoorLine = doorLine;
		}
	}
}