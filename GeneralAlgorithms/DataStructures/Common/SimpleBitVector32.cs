namespace GeneralAlgorithms.DataStructures.Common
{
	using System;
	using System.Collections.Specialized;

	public struct SimpleBitVector32
	{
		private BitVector32 vector32;

		public bool this[int position]
		{
			get => GetBit(position);
			set => SetBit(position, value);
		}

		public int Data => vector32.Data;

		public int Length => 32;

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