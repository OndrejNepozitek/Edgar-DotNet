namespace MapGeneration.Core.Interfaces
{
	using GeneralAlgorithms.DataStructures.Common;

	public interface IMutableConfiguration<out TConfiguration, TShapeContainer> : IConfiguration<TShapeContainer>
	{
		TConfiguration SetShape(TShapeContainer shapeContainer);

		TConfiguration SetPosition(IntVector2 position);
	}
}