namespace MapGeneration.Core.Interfaces
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;

	public interface IRoom<out TNode>
	{
		TNode Node { get; }

		IRoomDescription RoomDescription { get; }

		IntVector2 Position { get; }

		IList<OrthogonalLine> Doors { get; }
	}
}