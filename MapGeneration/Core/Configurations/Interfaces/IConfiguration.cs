using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;

namespace MapGeneration.Core.Configurations.Interfaces
{
    /// <summary>
	/// Represents a configuration of a layout's node.
	/// </summary>
	/// <typeparam name="TShapeContainer">Type of the shape container.</typeparam>
	public interface IConfiguration<out TShapeContainer, out TNode> : IPositionConfiguration, IShapeConfiguration<TShapeContainer>
	{
		/// <summary>
		/// Shape of the node.
		/// </summary>
		GridPolygon Shape { get; }

        TNode Node { get; }
	}
}