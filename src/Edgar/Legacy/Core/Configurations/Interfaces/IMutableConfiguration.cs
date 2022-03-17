using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Core.Configurations.Interfaces
{
    /// <summary>
    /// Represents a mutable configuration of a layout's node.
    /// </summary>
    /// <typeparam name="TShapeContainer">Type of the shape container.</typeparam>
    public interface IMutableConfiguration<TShapeContainer, TNode> : IConfiguration<TShapeContainer, TNode>
    {
        /// <summary>
        /// Shape container of the node.
        /// </summary>
        new TShapeContainer ShapeContainer { get; set; }

        /// <summary>
        /// Position of the node.
        /// </summary>
        new Vector2Int Position { get; set; }

        new TNode Node { get; set; }
    }
}