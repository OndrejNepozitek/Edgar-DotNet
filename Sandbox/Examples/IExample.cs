namespace Sandbox.Examples
{
	using MapGeneration.Core.MapDescriptions;

	public interface IExample
	{
		MapDescription<int> GetMapDescription();
    }
}