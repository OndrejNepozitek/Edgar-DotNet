namespace MapGeneration.Core.Interfaces
{
	public interface ISmartCloneable<out T>
	{
		T SmartClone();
	}
}