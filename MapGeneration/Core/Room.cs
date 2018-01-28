namespace MapGeneration.Core
{
	using System.Collections.Generic;
	using Doors.DoorModes;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public class Room<TNode> : IRoom<TNode>
	{
		public TNode Node { get; }

		public IRoomDescription RoomDescription { get; }

		public IntVector2 Position { get; }

		public IList<OrthogonalLine> Doors { get; }

		/*public Room(TNode node, GridPolygon shape, IntVector2 position)
		{
			Node = node;
			RoomDescription = new RoomDescription(shape, new OverlapMode());
		}*/
	}
}