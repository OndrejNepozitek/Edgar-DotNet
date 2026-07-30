using System;
using System.Collections;
using System.Collections.Generic;

namespace Edgar.Collections
{
    /// <summary>
    /// Factory methods for <see cref="ImmutableArray{T}"/>.
    /// Mirrors the subset of System.Collections.Immutable.ImmutableArray used by Edgar.
    /// </summary>
    public static class ImmutableArray
    {
        /// <summary>
        /// Creates an <see cref="ImmutableArray{T}"/> populated with the contents of the specified sequence.
        /// </summary>
        public static ImmutableArray<T> CreateRange<T>(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            // Reuse underlying array when the source is already an ImmutableArray (boxed).
            if (items is ImmutableArray<T> existing)
            {
                if (existing.IsDefault)
                {
                    throw new InvalidOperationException("Invalid operation on a default ImmutableArray.");
                }

                return existing;
            }

            if (items is ICollection<T> collection)
            {
                var count = collection.Count;
                if (count == 0)
                {
                    return ImmutableArray<T>.Empty;
                }

                var array = new T[count];
                var index = 0;
                foreach (var item in items)
                {
                    array[index++] = item;
                }

                return new ImmutableArray<T>(array);
            }

            // Unknown length: grow via List then take the array (preserves order).
            var list = new List<T>(items);
            if (list.Count == 0)
            {
                return ImmutableArray<T>.Empty;
            }

            return new ImmutableArray<T>(list.ToArray());
        }
    }

    /// <summary>
    /// A readonly array with O(1) indexable lookup time.
    /// Layout matches BCL ImmutableArray: a struct wrapping a single T[] field.
    /// </summary>
    public readonly struct ImmutableArray<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// An empty (initialized) instance. Uses <c>new T[0]</c> (not Array.Empty) to match BCL 5.0.
        /// </summary>
        public static readonly ImmutableArray<T> Empty = new ImmutableArray<T>(new T[0]);

        /// <summary>
        /// The backing array. Null means an uninitialized (default) instance.
        /// </summary>
        private readonly T[] array;

        /// <summary>
        /// Initializes without making a defensive copy. Factories must pass an owned array.
        /// </summary>
        internal ImmutableArray(T[] items)
        {
            array = items;
        }

        /// <summary>
        /// Gets whether this struct was initialized without an actual array instance.
        /// </summary>
        public bool IsDefault => array == null;

        /// <summary>
        /// Gets the element at the specified index.
        /// Throws <see cref="NullReferenceException"/> if this is a default instance (BCL-compatible).
        /// </summary>
        public T this[int index] => array[index];

        /// <summary>
        /// Gets the number of elements in the array.
        /// Throws <see cref="NullReferenceException"/> if this is a default instance (BCL-compatible).
        /// </summary>
        public int Length => array.Length;

        /// <inheritdoc />
        public int Count => Length;

        /// <summary>
        /// Returns a struct enumerator for foreach (does not implement IDisposable — matches BCL).
        /// </summary>
        public Enumerator GetEnumerator()
        {
            // Force NullReferenceException if uninitialized (same technique as BCL).
            _ = array.Length;
            return new Enumerator(array);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (array == null)
            {
                throw new InvalidOperationException("Invalid operation on a default ImmutableArray.");
            }

            return EnumeratorObject.Create(array);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        /// <summary>
        /// Struct enumerator used by foreach. Intentionally does not implement IDisposable.
        /// </summary>
        public struct Enumerator
        {
            private readonly T[] _array;
            private int _index;

            internal Enumerator(T[] array)
            {
                _array = array;
                _index = -1;
            }

            /// <summary>
            /// Gets the currently enumerated value.
            /// </summary>
            public T Current => _array[_index];

            /// <summary>
            /// Advances to the next value to be enumerated.
            /// </summary>
            public bool MoveNext()
            {
                return ++_index < _array.Length;
            }
        }

        private sealed class EnumeratorObject : IEnumerator<T>
        {
            private static readonly IEnumerator<T> EmptyEnumerator =
                new EnumeratorObject(Empty.array);

            private readonly T[] _array;
            private int _index;

            private EnumeratorObject(T[] array)
            {
                _array = array;
                _index = -1;
            }

            public T Current
            {
                get
                {
                    if ((uint)_index < (uint)_array.Length)
                    {
                        return _array[_index];
                    }

                    throw new InvalidOperationException();
                }
            }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                var newIndex = _index + 1;
                var length = _array.Length;

                if ((uint)newIndex <= (uint)length)
                {
                    _index = newIndex;
                    return (uint)newIndex < (uint)length;
                }

                return false;
            }

            public void Reset()
            {
                _index = -1;
            }

            public void Dispose()
            {
            }

            internal static IEnumerator<T> Create(T[] array)
            {
                return array.Length != 0 ? new EnumeratorObject(array) : EmptyEnumerator;
            }
        }
    }
}
