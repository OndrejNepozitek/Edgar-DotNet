using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Core.MapLayouts
{
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