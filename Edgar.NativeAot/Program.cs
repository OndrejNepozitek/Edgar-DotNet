using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Legacy.Utils;
using Edgar.NativeAotLib;
using Edgar.Utils;
using System.Diagnostics;
using File = System.IO.File;

namespace Edgar.NativeAot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GenerateFromUnity();
            return;

            Console.WriteLine("Running...");
            Console.WriteLine(DateTime.Now);

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

            var levelDescriptionJson = SystemTextJsonUtils.SerializeToJson(levelDescription);
            File.WriteAllText("levelDescription.json", levelDescriptionJson);
            var deserializedLevelDescription = SystemTextJsonUtils.DeserializeFromJson(levelDescriptionJson);
            var levelDescriptionJson2 = SystemTextJsonUtils.SerializeToJson(deserializedLevelDescription);
            File.WriteAllText("levelDescriptionRe.json", levelDescriptionJson2);

            if (levelDescriptionJson != levelDescriptionJson2)
            {
                throw new InvalidOperationException();
            }

            var random = new Random(0);
            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
            generator.InjectRandomGenerator(random);

            var layout = generator.GenerateLayout();

            var layoutJson = SystemTextJsonUtils.SerializeToJson(layout);
            File.WriteAllText("layout.json", layoutJson);

            Console.WriteLine(DateTime.Now);
        }

        private static void GenerateFromUnity()
        {
            var json = File.ReadAllText("levelDescriptionUnity.json");
            var levelDescription = SystemTextJsonUtils.DeserializeFromJson(json);

            var sw = new Stopwatch();
            sw.Start();

            var random = new Random(1198642031);
            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
            generator.InjectRandomGenerator(random);

            var layout = generator.GenerateLayout();
            var layoutJson = SystemTextJsonUtils.SerializeToJson(layout);

            Console.WriteLine($"Layout generated in {sw.ElapsedMilliseconds / 1000f:F} seconds");
            Console.WriteLine($"Iterations: {generator.IterationsCount}, time: {generator.TimeTotal}");

            File.WriteAllText("layoutUnity.json", layoutJson);
        }
    }
}