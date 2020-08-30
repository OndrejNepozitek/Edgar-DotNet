using System;

namespace Edgar.GraphBasedGenerator.Grid2D.Drawing
{
    /// <summary>
    /// Represents a range of type T.
    /// </summary>
    public class Range<T> where T : IComparable<T>
    {
        /// <summary>
        /// Minimum of the range. Inclusive.
        /// </summary>
        public T Minimum { get; }

        /// <summary>
        /// Maximum of the range. Inclusive.
        /// </summary>
        public T Maximum { get; }

        public Range(T minimum, T maximum)
        {
            Minimum = minimum;
            Maximum = maximum;

            if (!IsValid())
            {
                throw new ArgumentException($"The range is not valid. {nameof(Minimum)} must be less then or equal to {nameof(Maximum)}.");
            }
        }

        private bool IsValid()
        {
            return Minimum.CompareTo(Maximum) <= 0;
        }
    }
}