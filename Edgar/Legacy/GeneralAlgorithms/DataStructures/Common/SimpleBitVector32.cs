using System;
using System.Collections.Specialized;

namespace Edgar.Legacy.GeneralAlgorithms.DataStructures.Common
{
    /// <summary>
	/// Datastructure to hold upto 32 boolean values.
	/// </summary>
	public struct SimpleBitVector32
	{
		private BitVector32 vector32;

		/// <summary>
		/// Gets or sets a value of a bit at a given position.
		/// </summary>
		/// <remarks>
		/// Position must be an integer between 0 and 31 (inclusive).
		/// </remarks>
		/// <param name="position"></param>
		/// <returns></returns>
		public bool this[int position]
		{
			get => GetBit(position);
			set => SetBit(position, value);
		}

		/// <summary>
		/// Gets the vector as an integer.
		/// </summary>
		/// <remarks>
		/// 0-th position of the vectors equals to the least significant bit of the returned integer.
		/// </remarks>
		public int Data => vector32.Data;

		/// <summary>
		/// Gets a length of the vector.
		/// </summary>
		public int Length => 32;

		/// <summary>
		/// Creates a vector from given integer.
		/// </summary>
		/// <remarks>
		/// 0-th position of the vectors equals to the least significant bit of the integer.
		/// </remarks>
		/// <param name="data"></param>
		public SimpleBitVector32(int data)
		{
			vector32 = new BitVector32(data);
		}

		private void SetBit(int position, bool value)
		{
			if (position < 0 || position > 31)
				throw new InvalidOperationException("Position must be between 0 and 31 (inclusive).");

			vector32[1 << position] = value;
		}

		private bool GetBit(int position)
		{
			if (position < 0 || position > 31)
				throw new InvalidOperationException("Position must be between 0 and 31 (inclusive).");

			return vector32[1 << position];
		}

		/// <summary>
		/// Returns a vector where first x positions are ones.
		/// </summary>
		/// <param name="count">How many positions should be set to one.</param>
		/// <returns></returns>
		public static SimpleBitVector32 StartWithOnes(int count)
		{
			if (count < 0 || count > 31)
				throw new InvalidOperationException("Count must be between 0 and 31 (inclusive).");

			if (count == 0)
			{
				return new SimpleBitVector32(0);
			}

			return new SimpleBitVector32(int.MaxValue >> (32 - count - 1));
		}
	}
}