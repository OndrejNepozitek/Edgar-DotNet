namespace GeneralAlgorithms.DataStructures.Common
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Two way dictionary.
	/// </summary>
	/// <remarks>
	/// Methods from <see cref="IDictionary{TKey,TValue}"/> keep their old semantics.
	/// </remarks>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class TwoWayDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> keyValueDict = new Dictionary<TKey, TValue>();
		private readonly Dictionary<TValue, TKey> valueKeyDict = new Dictionary<TValue, TKey>();

		/// <inheritdoc />
		public ICollection<TKey> Keys => keyValueDict.Keys;

		/// <inheritdoc />
		public ICollection<TValue> Values => keyValueDict.Values;

		/// <inheritdoc />
		public int Count => keyValueDict.Count;

		/// <inheritdoc />
		public bool IsReadOnly => false;

		/// <inheritdoc />
		public TValue this[TKey key]
		{
			get => keyValueDict[key];
			set => keyValueDict[key] = value;
		}

		/// <inheritdoc />
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return keyValueDict.GetEnumerator();
		}

		/// <inheritdoc />
		public void Clear()
		{
			keyValueDict.Clear();
			valueKeyDict.Clear();
		}

		/// <inheritdoc />
		public void Add(TKey key, TValue value)
		{
			if (keyValueDict.ContainsKey(key) || valueKeyDict.ContainsKey(value))
				throw new ArgumentException("Given key already exists");

			keyValueDict.Add(key, value);
			valueKeyDict.Add(value, key);
		}

		/// <inheritdoc />
		public bool Remove(TKey key)
		{
			if (keyValueDict.TryGetValue(key, out var value))
			{
				keyValueDict.Remove(key);
				valueKeyDict.Remove(value);

				return true;
			}

			return false;
		}

		/// <inheritdoc />
		public bool ContainsKey(TKey key)
		{
			return keyValueDict.ContainsKey(key);
		}

		/// <summary>
		/// Determines whether the dictionary contains an element with the specified value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool ContainsValue(TValue value)
		{
			return valueKeyDict.ContainsKey(value);
		}

		/// <inheritdoc />
		public bool TryGetValue(TKey key, out TValue value)
		{
			return keyValueDict.TryGetValue(key, out value);
		}

		/// <summary>
		/// Gets the key associated with the specified value.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool TryGetKey(TValue value, out TKey key)
		{
			return valueKeyDict.TryGetValue(value, out key);
		}

		/// <summary>
		/// Gets the key associated with the specified value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public TKey GetByValue(TValue value)
		{
			return valueKeyDict[value];
		}

		#region Explicit implementations

		/// <inheritdoc />
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}