using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Core.Configurations;
using Edgar.Legacy.Core.Configurations.EnergyData;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using NUnit.Framework;

namespace MapGeneration.IntegrationTests.Core.ConfigurationSpaces
{
    [TestFixture]
    public class ConfigurationSpacesGeneratorTests
    {
        private ConfigurationSpacesGenerator generator;

        [SetUp]
        public void SetUp()
        {
            generator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());
        }

        [Test]
        public void Generate_BasicTest()
        {
            var roomTemplate1 = new RoomTemplate(PolygonGrid2D.GetSquare(10), new SimpleDoorMode(1, 0), TransformationGrid2DHelper.GetAllTransformationsOld().ToList());
            var roomTemplate2 = new RoomTemplate(PolygonGrid2D.GetRectangle(5, 10), new SimpleDoorMode(1, 0), TransformationGrid2DHelper.GetAllTransformationsOld().ToList());

            var roomDescription1 = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate1 });
            var roomDescription2 = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate1, roomTemplate2 });

            var mapDescription = new MapDescription<int>();
            mapDescription.AddRoom(0, roomDescription1);
            mapDescription.AddRoom(1, roomDescription2);
            mapDescription.AddConnection(0, 1);

            var configurationSpaces = generator.GetConfigurationSpaces<Configuration<CorridorsData>>(mapDescription);

            Assert.That(configurationSpaces.GetShapesForNode(0).Count, Is.EqualTo(1));
            Assert.That(configurationSpaces.GetShapesForNode(1).Count, Is.EqualTo(3));
            Assert.That(configurationSpaces.GetAllShapes().Count, Is.EqualTo(3));
        }
    }
}