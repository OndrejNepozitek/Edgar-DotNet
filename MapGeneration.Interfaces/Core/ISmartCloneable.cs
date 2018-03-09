namespace MapGeneration.Interfaces.Core
{
	/// <summary>
	/// Interface for types that can be cloned.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ISmartCloneable<out T>
	{
		/// <summary>
		/// Performs a deep clone.
		/// </summary>
		/// <remarks>
		/// May not deep clone everything if it does not make sense in a given context.
		/// </remarks>
		/// <returns></returns>
		T SmartClone();
	}
}