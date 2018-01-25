namespace MapGeneration.Core.Interfaces
{
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public interface IConfiguration<out TConfiguration, TShapeContainer>
	{
		GridPolygon Shape { get; }

		TShapeContainer ShapeContainer { get; }

		IntVector2 Position { get; }

		SimpleBitVector32 ValidityVector { get; }

		bool IsValid { get; }

		TConfiguration SetShape(TShapeContainer shapeContainer);

		TConfiguration SetPosition(IntVector2 position);

		TConfiguration SetValidityVector(SimpleBitVector32 validityVector);
	}
}