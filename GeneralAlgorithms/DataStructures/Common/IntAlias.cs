namespace GeneralAlgorithms.DataStructures.Common
{
	public class IntAlias<T>
	{
		public int Alias { get; }

		public T Value { get; }

		public IntAlias(int alias, T value)
		{
			Alias = alias;
			Value = value;
		}
	}
}