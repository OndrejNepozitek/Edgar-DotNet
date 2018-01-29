namespace MapGeneration.Core
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public class Room<TNode> : IRoom<TNode>
	{
		public TNode Node { get; }
		public GridPolygon Shape { get; }

		public IntVector2 Position { get; }

		public IList<OrthogonalLine> Doors { get; }

		public Room(TNode node, GridPolygon shape, IntVector2 position)
		{
			Node = node;
			Shape = shape;
			Position = position;
			Doors = null;
		}
	}
}