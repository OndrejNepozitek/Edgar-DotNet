namespace MapGeneration.Interfaces.Core.LayoutGenerator
{
	using Utils;

	public interface IChainBasedLayoutGenerator<in TMapDescription, TLayout> : 
		IObservableGenerator<TMapDescription, TLayout>,
		IBenchmarkableLayoutGenerator<TMapDescription, TLayout>,
		IRandomInjectable,
		ICancellable
	{
		
	}
}