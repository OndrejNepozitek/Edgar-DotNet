namespace MapGeneration.Core.MapLayouts
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core.MapDescriptions;
	using Interfaces.Core.MapLayouts;

	/// <inheritdoc />
	public class Room<TNode> : IRoom<TNode>
	{
		public TNode Node { get; }

		public GridPolygon Shape { get; }

		public IntVector2 Position { get; }

		public bool IsCorridor { get; }

		public IList<IDoorInfo<TNode>> Doors { get; set; }

		public IRoomDescription RoomDescription { get; }

		public int Rotation { get; }

		public Room(TNode node, GridPolygon shape, IntVector2 position, bool isCorridor, IRoomDescription roomDescription, int rotation)
		{
			Node = node;
			Shape = shape;
			Position = position;
			IsCorridor = isCorridor;
			RoomDescription = roomDescription;
			Rotation = rotation;
			Doors = null;
		}
	}
}