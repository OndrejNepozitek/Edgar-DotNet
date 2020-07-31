﻿using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;

namespace MapGeneration.Core.Configurations.Interfaces
{
    /// <summary>
	/// Represents a configuration of a layout's node.
	/// </summary>
	/// <typeparam name="TShapeContainer">Type of the shape container.</typeparam>
	public interface IConfiguration<out TShapeContainer, out TNode>
	{
		/// <summary>
		/// Shape of the node.
		/// </summary>
		GridPolygon Shape { get; }

		/// <summary>
		/// Shape container of the node.
		/// </summary>
		TShapeContainer ShapeContainer { get; }

		/// <summary>
		/// Position of the node.
		/// </summary>
		Vector2Int Position { get; }

		/// <summary>
		/// Whether a node is valid or not.
		/// </summary>
		bool IsValid { get; }

		TNode Node { get; }
	}
}