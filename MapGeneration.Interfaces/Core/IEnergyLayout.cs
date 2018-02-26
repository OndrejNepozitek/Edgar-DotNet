namespace MapGeneration.Interfaces.Core
{
	public interface IEnergyLayout<TNode, TConfiguration> : ILayout<TNode, TConfiguration> 
	{
		float Energy { get; set; }

		bool IsValid { get; set; }
	}
}