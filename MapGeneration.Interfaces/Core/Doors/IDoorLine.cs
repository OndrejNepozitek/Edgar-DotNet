namespace MapGeneration.Interfaces.Core.Doors
{
	using GeneralAlgorithms.DataStructures.Common;

	public interface IDoorLine
	{
		OrthogonalLine Line { get; }

		int Length { get; }
	}
}