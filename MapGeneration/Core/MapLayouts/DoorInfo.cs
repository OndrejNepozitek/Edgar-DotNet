namespace MapGeneration.Core.MapLayouts
{
	using GeneralAlgorithms.DataStructures.Common;
    using MapGeneration.Core.MapLayouts.Interfaces;

    /// <summary>
    /// Represents door information.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
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