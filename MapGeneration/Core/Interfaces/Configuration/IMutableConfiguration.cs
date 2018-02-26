namespace MapGeneration.Core.Interfaces.Configuration
{
	using GeneralAlgorithms.DataStructures.Common;

	public interface IMutableConfiguration<TShapeContainer> : IConfiguration<TShapeContainer>
	{
		new TShapeContainer ShapeContainer { get; set; }

		new IntVector2 Position { get; set; }
	}
}