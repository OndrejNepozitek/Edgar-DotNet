namespace MapGeneration.Core.SimulatedAnnealing
{
	using System.Threading;

	/// <inheritdoc />
	public class SAContext : ISAContext
	{
		public int IterationsCount { get; set; }

		public CancellationToken? CancellationToken { get; set; }
	}
}