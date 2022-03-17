using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace GeneralAlgorithms.Tests.DataStructures.Common
{
    using NUnit.Framework;

    [TestFixture]
    public class IntVector2Tests
    {
        [Test]
        public void Transform_CorrectlyTransforms()
        {
            var point = new Vector2Int(1, 2);

            Assert.That(point.Transform(TransformationGrid2D.Identity), Is.EqualTo(new Vector2Int(1, 2)));
            Assert.That(point.Transform(TransformationGrid2D.Rotate90), Is.EqualTo(new Vector2Int(2, -1)));
            Assert.That(point.Transform(TransformationGrid2D.Rotate180), Is.EqualTo(new Vector2Int(-1, -2)));
            Assert.That(point.Transform(TransformationGrid2D.Rotate270), Is.EqualTo(new Vector2Int(-2, 1)));
            Assert.That(point.Transform(TransformationGrid2D.MirrorX), Is.EqualTo(new Vector2Int(1, -2)));
            Assert.That(point.Transform(TransformationGrid2D.MirrorY), Is.EqualTo(new Vector2Int(-1, 2)));
            Assert.That(point.Transform(TransformationGrid2D.Diagonal13), Is.EqualTo(new Vector2Int(2, 1)));
            Assert.That(point.Transform(TransformationGrid2D.Diagonal24), Is.EqualTo(new Vector2Int(-2, -1)));
        }
    }
}