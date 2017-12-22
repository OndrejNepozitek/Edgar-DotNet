namespace MapGeneration.Core.Interfaces
{
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public interface IRoom<out TNode>
	{
		TNode Node { get; }

		GridPolygon Shape { get; }

		IntVector2 Position { get; }
	}
}