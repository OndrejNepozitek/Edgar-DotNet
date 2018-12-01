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
		TNode Node { get; }

		GridPolygon Shape { get;  }

		IntVector2 Position { get; }

		bool IsCorridor { get; }

		IList<IDoorInfo<TNode>> Doors { get; }

		IRoomDescription RoomDescription { get; }

		int Rotation { get; }
	}
}