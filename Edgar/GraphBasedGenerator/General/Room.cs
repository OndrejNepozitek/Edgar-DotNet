﻿using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.GraphBasedGenerator.General
{
    /// <summary>
    /// Represents a layout room.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
	public class Room<TNode>
	{
        /// <summary>
        /// Corresponding input graph node.
        /// </summary>
        public TNode Node { get; }

        /// <summary>
        /// Shape of the room.
        /// </summary>
        public PolygonGrid2D Shape { get; }

        /// <summary>
        /// Position of the room.
        /// </summary>
        public Vector2Int Position { get; }
        
        /// <summary>
        /// Room template used for this room.
        /// </summary>
        public RoomTemplate RoomTemplate { get; }

        /// <summary>
        /// Room template instance used for this room.
        /// </summary>
        public RoomTemplateInstance RoomTemplateInstance { get; }

        /// <summary>
        /// Whether it is a corridor room or not.
        /// </summary>
        public bool IsCorridor { get; }

        /// <summary>
        /// Information about connections to neighbours.
        /// </summary>
        public List<DoorInfo<TNode>> Doors { get; set; }

        /// <summary>
        /// Room description.
        /// </summary>
        public RoomDescriptionGrid2D RoomDescription { get; }

        /// <summary>
        /// Chosen transformation of the room shape.
        /// </summary>
        public Transformation Transformation { get; }

        /// <summary>
        /// All possible transformations of the room description.
        /// </summary>
        public IList<Transformation> Transformations { get; }

		public Room(TNode node, PolygonGrid2D shape, Vector2Int position, bool isCorridor, RoomTemplate roomTemplate, RoomDescriptionGrid2D roomDescription, Transformation transformation, List<Transformation> transformations, RoomTemplateInstance roomTemplateInstance)
		{
			Node = node;
			Shape = shape;
			Position = position;
			IsCorridor = isCorridor;
			RoomTemplate = roomTemplate;
			Transformation = transformation;
			Transformations = transformations;
            RoomTemplateInstance = roomTemplateInstance;
            RoomDescription = roomDescription;
        }
	}
}