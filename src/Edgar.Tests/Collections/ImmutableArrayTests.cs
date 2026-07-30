using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Collections;
using NUnit.Framework;

namespace Edgar.Tests.Collections
{
    [TestFixture]
    public class ImmutableArrayTests
    {
        [Test]
        public void CreateRange_FromList_PreservesOrderAndContents()
        {
            var source = new List<int> { 3, 1, 4, 1, 5 };

            var array = ImmutableArray.CreateRange(source);

            Assert.That(array.Length, Is.EqualTo(5));
            Assert.That(array.Count, Is.EqualTo(5));
            Assert.That(array[0], Is.EqualTo(3));
            Assert.That(array[1], Is.EqualTo(1));
            Assert.That(array[2], Is.EqualTo(4));
            Assert.That(array[3], Is.EqualTo(1));
            Assert.That(array[4], Is.EqualTo(5));
            Assert.That(array.ToList(), Is.EqualTo(source));
        }

        [Test]
        public void CreateRange_FromEmptyList_ReturnsEmpty()
        {
            var array = ImmutableArray.CreateRange(new List<int>());

            Assert.That(array.IsDefault, Is.False);
            Assert.That(array.Length, Is.EqualTo(0));
            Assert.That(array.Length, Is.EqualTo(ImmutableArray<int>.Empty.Length));
        }

        [Test]
        public void CreateRange_FromEmptyEnumerable_ReturnsEmpty()
        {
            IEnumerable<int> source = EmptyEnumerable();

            var array = ImmutableArray.CreateRange(source);

            Assert.That(array.Length, Is.EqualTo(0));
            Assert.That(array.ToList(), Is.Empty);
        }

        [Test]
        public void CreateRange_FromEnumerable_PreservesOrder()
        {
            IEnumerable<int> source = YieldInOrder(10, 20, 30);

            var array = ImmutableArray.CreateRange(source);

            Assert.That(array.ToList(), Is.EqualTo(new[] { 10, 20, 30 }));
        }

        [Test]
        public void CreateRange_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArray.CreateRange<int>(null));
        }

        [Test]
        public void CreateRange_CopiesSource_MutationDoesNotAffectArray()
        {
            var source = new List<int> { 1, 2, 3 };

            var array = ImmutableArray.CreateRange(source);
            source[0] = 99;
            source.Add(4);

            Assert.That(array.Length, Is.EqualTo(3));
            Assert.That(array[0], Is.EqualTo(1));
            Assert.That(array.ToList(), Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void CreateRange_FromExistingImmutableArray_ReusesContents()
        {
            var original = ImmutableArray.CreateRange(new[] { 7, 8, 9 });

            // Box as IEnumerable so CreateRange takes the reuse path
            IEnumerable<int> boxed = original;
            var reused = ImmutableArray.CreateRange(boxed);

            Assert.That(reused.Length, Is.EqualTo(3));
            Assert.That(reused.ToList(), Is.EqualTo(new[] { 7, 8, 9 }));
        }

        [Test]
        public void CreateRange_FromDefaultImmutableArray_ThrowsInvalidOperationException()
        {
            ImmutableArray<int> uninitialized = default;
            IEnumerable<int> boxed = uninitialized;

            Assert.Throws<InvalidOperationException>(() => ImmutableArray.CreateRange(boxed));
        }

        [Test]
        public void Empty_IsInitializedAndEmpty()
        {
            var empty = ImmutableArray<string>.Empty;

            Assert.That(empty.IsDefault, Is.False);
            Assert.That(empty.Length, Is.EqualTo(0));
            Assert.That(empty.Count, Is.EqualTo(0));
            Assert.That(empty.ToList(), Is.Empty);
        }

        [Test]
        public void Default_IsDefault_AndLengthThrows()
        {
            ImmutableArray<int> uninitialized = default;

            Assert.That(uninitialized.IsDefault, Is.True);
            Assert.Throws<NullReferenceException>(() =>
            {
                var _ = uninitialized.Length;
            });
            Assert.Throws<NullReferenceException>(() =>
            {
                var _ = uninitialized[0];
            });
            Assert.Throws<NullReferenceException>(() =>
            {
                foreach (var _ in uninitialized)
                {
                }
            });
        }

        [Test]
        public void Foreach_EnumeratesInOrder()
        {
            var array = ImmutableArray.CreateRange(new List<int> { 2, 4, 6, 8 });
            var result = new List<int>();

            foreach (var item in array)
            {
                result.Add(item);
            }

            Assert.That(result, Is.EqualTo(new[] { 2, 4, 6, 8 }));
        }

        [Test]
        public void StructEnumerator_StartsBeforeFirstElement()
        {
            var array = ImmutableArray.CreateRange(new List<int> { 42 });
            var enumerator = array.GetEnumerator();

            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(42));
            Assert.That(enumerator.MoveNext(), Is.False);
        }

        [Test]
        public void AsIEnumerable_SupportsLinq()
        {
            var array = ImmutableArray.CreateRange(new List<int> { 1, 2, 3, 4 });

            Assert.That(array.Select(x => x * 2).ToList(), Is.EqualTo(new[] { 2, 4, 6, 8 }));
            Assert.That(array.Where(x => x % 2 == 0).ToList(), Is.EqualTo(new[] { 2, 4 }));
            Assert.That(array.Sum(), Is.EqualTo(10));
        }

        [Test]
        public void Indexer_OutOfRange_Throws()
        {
            var array = ImmutableArray.CreateRange(new List<int> { 1 });

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var _ = array[1];
            });
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var _ = array[-1];
            });
        }

        [Test]
        public void IEnumerableGetEnumerator_OnDefault_ThrowsInvalidOperationException()
        {
            ImmutableArray<int> uninitialized = default;
            IEnumerable<int> enumerable = uninitialized;

            Assert.Throws<InvalidOperationException>(() => enumerable.GetEnumerator());
        }

        private static IEnumerable<int> EmptyEnumerable()
        {
            yield break;
        }

        private static IEnumerable<int> YieldInOrder(params int[] values)
        {
            foreach (var value in values)
            {
                yield return value;
            }
        }
    }
}
