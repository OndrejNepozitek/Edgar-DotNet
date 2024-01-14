using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Legacy.Utils;
using Edgar.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Edgar.Tests
{
    [TestFixture]
    public class AotTests
    {
        [Test]
        public void SerializeToJson()
        {
            var graph = GraphsDatabase.GetExample4();
            var roomTemplates = new List<RoomTemplateGrid2D>()
            {
                new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(10), new SimpleDoorModeGrid2D(1, 1)),
            };

            var levelDescription = new LevelDescriptionGrid2D<int>();
            foreach (var graphVertex in graph.Vertices)
            {
                levelDescription.AddRoom(graphVertex, new RoomDescriptionGrid2D(false, roomTemplates));
            }

            foreach (var graphEdge in graph.Edges)
            {
                levelDescription.AddConnection(graphEdge.From, graphEdge.To);
            }

            var levelDescriptionJson2 = JsonUtils2.SerializeToJson(levelDescription);
            File.WriteAllText("levelDescription2.json", levelDescriptionJson2);
        }

        [Test]
        public void DeserializeLayoutJson()
        {
            var json = File.ReadAllText(
                "C:\\Personal projects\\DotNet\\Edgar-DotNet\\Edgar.NativeAot\\bin\\Debug\\net8.0\\layout.json");
            var layout = JsonUtils2.DeserializeFromJson(json);
        }
    }
}