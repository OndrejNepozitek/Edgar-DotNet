namespace Sandbox.Examples
{
	using MapGeneration.Core.MapDescriptions;

	public interface IExample
	{
		MapDescriptionOld<int> GetMapDescription();
	}
}