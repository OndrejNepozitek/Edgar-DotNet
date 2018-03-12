namespace MapGeneration.Core.Experimental
{
	using Interfaces.Core;
	using Interfaces.Core.GeneratorPlanners;

	// TODO: rename or remove
	/// <inheritdoc />
	public class GeneratorContext : IGeneratorContext
	{
		public int IterationsCount { get; set; }
	}
}