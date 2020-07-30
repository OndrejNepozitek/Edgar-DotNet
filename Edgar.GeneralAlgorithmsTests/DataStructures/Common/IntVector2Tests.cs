namespace GeneralAlgorithms.Tests.DataStructures.Common
{
	using GeneralAlgorithms.DataStructures.Common;
	using NUnit.Framework;

	[TestFixture]
	public class IntVector2Tests
	{
		[Test]
		public void Transform_CorrectlyTransforms()
		{
			var point = new IntVector2(1, 2);

			Assert.That(point.Transform(Transformation.Identity), Is.EqualTo(new IntVector2(1, 2)));
			Assert.That(point.Transform(Transformation.Rotate90), Is.EqualTo(new IntVector2(2, -1)));
			Assert.That(point.Transform(Transformation.Rotate180), Is.EqualTo(new IntVector2(-1, -2)));
			Assert.That(point.Transform(Transformation.Rotate270), Is.EqualTo(new IntVector2(-2, 1)));
			Assert.That(point.Transform(Transformation.MirrorX), Is.EqualTo(new IntVector2(1, -2)));
			Assert.That(point.Transform(Transformation.MirrorY), Is.EqualTo(new IntVector2(-1, 2)));
			Assert.That(point.Transform(Transformation.Diagonal13), Is.EqualTo(new IntVector2(2, 1)));
			Assert.That(point.Transform(Transformation.Diagonal24), Is.EqualTo(new IntVector2(-2, -1)));
		}
	}
}