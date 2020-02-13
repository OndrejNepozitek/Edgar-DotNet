using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Configurations;
using MapGeneration.Core.Configurations.EnergyData;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.Doors;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.MapDescriptions;
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
            var roomTemplate1 = new RoomTemplate(GridPolygon.GetSquare(10), new SimpleDoorMode(1, 0), TransformationHelper.GetAllTransformations().ToList());
            var roomTemplate2 = new RoomTemplate(GridPolygon.GetRectangle(5, 10), new SimpleDoorMode(1, 0), TransformationHelper.GetAllTransformations().ToList());

            var roomDescription1 = new BasicRoomDescription(new List<IRoomTemplate>() { roomTemplate1 });
            var roomDescription2 = new BasicRoomDescription(new List<IRoomTemplate>() { roomTemplate1, roomTemplate2 });

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