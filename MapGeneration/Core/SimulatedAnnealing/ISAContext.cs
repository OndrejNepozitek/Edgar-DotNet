namespace MapGeneration.Core.SimulatedAnnealing
{
	using System.Threading;

	/// <summary>
	/// Interface describing a context of SAGenerator
	/// </summary>
	public interface ISAContext
	{
		int IterationsCount { get; }

		CancellationToken? CancellationToken { get; }
	}
}