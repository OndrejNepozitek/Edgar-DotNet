namespace MapGeneration.Core.Interfaces
{
	using GeneralAlgorithms.DataStructures.Common;

	public interface IConfiguration<out TConfiguration, TShape>
	{
		TShape Shape { get; }

		IntVector2 Position { get; }

		TConfiguration SetShape(TShape shape);

		TConfiguration SetPosition(IntVector2 position);
	}
}