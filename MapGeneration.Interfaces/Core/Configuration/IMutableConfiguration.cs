namespace MapGeneration.Interfaces.Core.Configuration
{
	using GeneralAlgorithms.DataStructures.Common;

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
		new IntVector2 Position { get; set; }

        new TNode Node { get; set; }
	}
}