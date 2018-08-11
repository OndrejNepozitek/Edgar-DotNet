namespace GeneralAlgorithms.DataStructures.Common
{
	/// <summary>
	/// Integer alias of an object of a given generic type.
	/// </summary>
	/// <remarks>
	/// Useful when array indexing is much faster than using a dictionary.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public class IntAlias<T>
	{
		/// <summary>
		/// Integer alias of the Value.
		/// </summary>
		public int Alias { get; }

		/// <summary>
		/// Real value.
		/// </summary>
		public T Value { get; }

		public IntAlias(int alias, T value)
		{
			Alias = alias;
			Value = value;
		}
	}
}