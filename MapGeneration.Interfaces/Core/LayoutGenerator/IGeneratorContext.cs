namespace MapGeneration.Interfaces.Core.LayoutGenerator
{
	/// <summary>
	/// Represents a context of a layout generator.
	/// </summary>
	public interface IGeneratorContext
	{
		int IterationsCount { get; }
	}
}