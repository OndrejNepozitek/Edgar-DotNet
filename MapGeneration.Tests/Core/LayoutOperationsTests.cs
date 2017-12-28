namespace MapGeneration.Tests.Core
{
	using MapGeneration.Core;
	using NUnit.Framework;

	[TestFixture]
	public class LayoutOperationsTests
	{
		private LayoutOperations<int, Layout> layoutOperations;

		[OneTimeSetUp]
		public void SetUp()
		{
			layoutOperations = new LayoutOperations<int, Layout>(null);
		}

		[Test]
		public void RecomputeValidity_AllValid()
		{
			Assert.IsTrue(false);
		}

		[Test]
		public void RecomputeValidity_AllInvalid()
		{
			Assert.IsTrue(false);
		}
	}
}