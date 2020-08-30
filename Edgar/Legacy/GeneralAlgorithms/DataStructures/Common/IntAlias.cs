namespace Edgar.Legacy.GeneralAlgorithms.DataStructures.Common
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

        #region Equals

        protected bool Equals(IntAlias<T> other)
        {
            return Alias == other.Alias;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntAlias<T>) obj);
        }

        public override int GetHashCode()
        {
            return Alias;
        }

        public static bool operator ==(IntAlias<T> left, IntAlias<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IntAlias<T> left, IntAlias<T> right)
        {
            return !Equals(left, right);
        }

        #endregion
	}
}