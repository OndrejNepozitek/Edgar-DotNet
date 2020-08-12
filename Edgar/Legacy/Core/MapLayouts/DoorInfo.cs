namespace MapGeneration.Core.MapLayouts
{
	using GeneralAlgorithms.DataStructures.Common;

    /// <summary>
    /// Represents door information.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
	public class DoorInfo<TNode>
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