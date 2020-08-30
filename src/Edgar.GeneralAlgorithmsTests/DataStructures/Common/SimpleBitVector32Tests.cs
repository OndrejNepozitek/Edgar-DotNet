using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace GeneralAlgorithms.Tests.DataStructures.Common
{
    using NUnit.Framework;

	[TestFixture]
	public class SimpleBitVector32Tests
	{
		[Test]
		public void Data()
		{
			Assert.AreEqual(0, new SimpleBitVector32(0).Data);
			Assert.AreEqual(1454, new SimpleBitVector32(1454).Data);
		}

		[Test]
		public void GetIndexer()
		{
			var vector = new SimpleBitVector32(101);

			Assert.AreEqual(true, vector[0]);
			Assert.AreEqual(false, vector[1]);
			Assert.AreEqual(true, vector[2]);
			Assert.AreEqual(false, vector[3]);
			Assert.AreEqual(false, vector[4]);
			Assert.AreEqual(true, vector[5]);
			Assert.AreEqual(true, vector[6]);
			Assert.AreEqual(false, vector[7]);

			for (var i = 8; i < 32; i++)
			{
				Assert.AreEqual(false, vector[i]);
			}
		}

		[Test]
		public void SetIndexer()
		{
			var vector = new SimpleBitVector32(0);

			vector[3] = true;
			Assert.AreEqual(true, vector[3]);

			vector[3] = false;
			Assert.AreEqual(false, vector[3]);

			vector[15] = true;
			Assert.AreEqual(true, vector[15]);

			vector[15] = false;
			Assert.AreEqual(false, vector[15]);
		}

		[Test]
		public void StartWithOnes()
		{
			{
				var vector = SimpleBitVector32.StartWithOnes(0);
				Assert.AreEqual(0, vector.Data);
			}

			{
				var vector = SimpleBitVector32.StartWithOnes(13);

				for (var i = 0; i < 13; i++)
				{
					Assert.IsTrue(vector[i]);
				}

				for (var i = 13; i < 32; i++)
				{
					Assert.IsFalse(vector[i]);
				}
			}
		}
	}
}