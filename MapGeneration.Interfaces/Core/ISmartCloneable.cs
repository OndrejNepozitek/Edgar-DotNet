namespace MapGeneration.Interfaces.Core
{
	public interface ISmartCloneable<out T>
	{
		T SmartClone();
	}
}