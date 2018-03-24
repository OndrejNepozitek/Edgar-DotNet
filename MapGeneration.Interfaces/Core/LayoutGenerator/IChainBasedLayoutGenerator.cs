namespace MapGeneration.Interfaces.Core.LayoutGenerator
{
	using Benchmarks;
	using Utils;

	public interface IChainBasedLayoutGenerator<in TMapDescription, TNode> : IObservableGenerator<TMapDescription, TNode>, IRandomInjectable, ICancellable, IBenchmarkable
	{
		
	}
}