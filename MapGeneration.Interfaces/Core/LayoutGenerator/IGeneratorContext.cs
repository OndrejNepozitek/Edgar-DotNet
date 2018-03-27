namespace MapGeneration.Interfaces.Core.LayoutGenerator
{
	/// <summary>
	/// Represents a context of a generator layout.
	/// </summary>
	public interface IGeneratorContext
	{
		int IterationsCount { get; }
	}
}