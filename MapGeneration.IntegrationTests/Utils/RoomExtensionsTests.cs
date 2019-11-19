using System.Linq;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Utils;
using NUnit.Framework;

namespace MapGeneration.IntegrationTests.Utils
{
    [TestFixture]
    public class RoomExtensionsTests
    {
        [Test]
        public void TransformPointToNewPosition_RectanglePoints()
        {
            // Create a rectangular room shape
            // Move it away from (0, 0) so that we can properly test the functionality
            var rectangleShape = GridPolygon.GetRectangle(10, 4) + new IntVector2(10, 10);

            // Create points to be transformed
            // These could be for example traps, spawn points, etc.
            // We use points of the rectangle here because it is easy to check that the transformation is correct.
            var pointsToTransform = rectangleShape.GetPoints();

            var mapDescription = new MapDescription<int>();

            // Create simple graph with 2 vertices and 1 edge
            mapDescription.AddRoom(0);
            mapDescription.AddRoom(1);
            mapDescription.AddPassage(0, 1);

            // Add the rectangle shape
            mapDescription.AddRoomShapes(new RoomDescription(rectangleShape, new OverlapMode(1, 0)));

            var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
            var layout = layoutGenerator.GetLayouts(mapDescription, 1)[0];

            foreach (var room in layout.Rooms)
            {
                // The points were chosen to be the points of the polygon, so after transforming them, they should
                // be equal to the room.Shape + room.Position points
                var transformedPoints = pointsToTransform.Select(x => room.TransformPointToNewPosition(x));
                var expectedPoints = (room.Shape + room.Position).GetPoints();

                Assert.That(transformedPoints, Is.EquivalentTo(expectedPoints));
            }
        }
    }
}