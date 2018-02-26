namespace MapGeneration.Core.Interfaces.Configuration
{
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public interface IConfiguration<out TShapeContainer>
	{
		GridPolygon Shape { get; }

		TShapeContainer ShapeContainer { get; }

		IntVector2 Position { get; }

		bool IsValid { get; }
	}
}