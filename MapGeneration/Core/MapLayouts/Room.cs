using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;

namespace MapGeneration.Core.MapLayouts
{
    /// <summary>
    /// Represents a layout room.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
	public class Room<TNode>
	{
		public TNode Node { get; }

		public GridPolygon Shape { get; }

		public IntVector2 Position { get; }

		public bool IsCorridor { get; }

		public List<DoorInfo<TNode>> Doors { get; set; }

		public RoomTemplate RoomTemplate { get; }

        public IRoomDescription RoomDescription { get; }

        public Transformation Transformation { get; }

		public List<Transformation> Transformations { get; }

		public Room(TNode node, GridPolygon shape, IntVector2 position, bool isCorridor, RoomTemplate roomTemplate, IRoomDescription roomDescription, Transformation transformation, List<Transformation> transformations)
		{
			Node = node;
			Shape = shape;
			Position = position;
			IsCorridor = isCorridor;
			RoomTemplate = roomTemplate;
			Transformation = transformation;
			Transformations = transformations;
            RoomDescription = roomDescription;
            Doors = null;
		}
	}
}