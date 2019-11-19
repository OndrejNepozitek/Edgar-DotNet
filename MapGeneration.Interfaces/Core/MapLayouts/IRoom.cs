namespace MapGeneration.Interfaces.Core.MapLayouts
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapDescriptions;

	/// <summary>
	/// Represents a layout room.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public interface IRoom<TNode>
	{
		/// <summary>
		/// Corresponding input graph node.
		/// </summary>
		TNode Node { get; }

		/// <summary>
		/// Shape of the room.
		/// </summary>
		GridPolygon Shape { get;  }

		/// <summary>
		/// Position of the room.
		/// </summary>
		IntVector2 Position { get; }

		/// <summary>
		/// Whether it is a corridor room or not.
		/// </summary>
		bool IsCorridor { get; }

		/// <summary>
		/// Information about connections to neighbours.
		/// </summary>
		IList<IDoorInfo<TNode>> Doors { get; }

		/// <summary>
		/// Room description.
		/// </summary>
		IRoomDescription RoomDescription { get; }

		/// <summary>
		/// Chosen transformation of the room shape.
		/// </summary>
		Transformation Transformation { get; }

		/// <summary>
		/// All possible transformations of the room description.
		/// </summary>
		IList<Transformation> Transformations { get; }
	}
}