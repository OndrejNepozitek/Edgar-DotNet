namespace MapGeneration.Core.LayoutGenerators
{
	using Interfaces.Core.LayoutGenerator;

	/// <inheritdoc />
	public class GeneratorContext : IGeneratorContext
	{
		public int IterationsCount { get; set; }
	}
}