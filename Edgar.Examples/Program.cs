using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Doors;
using Edgar.Legacy.Core.Doors.ManualMode;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Newtonsoft.Json;

namespace Edgar.Examples
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            Parser
                .Default
                .ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        private static void Run(Options options)
        {
            LoadRoomTemplates();

            var examplesGenerator = new ExamplesGenerator(options.SourceFolder, options.OutputFolder);
            examplesGenerator.Run();
        }

        private static void LoadRoomTemplates()
        {
            var settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
            };

            MapDescription<int> mapDescription = null;

            if (File.Exists($"Resources/MapDescriptions/gungeon_1_1.json"))
            {
                mapDescription = JsonConvert.DeserializeObject<MapDescription<int>>(
                    File.ReadAllText($"Resources/MapDescriptions/gungeon_1_1.json"), settings);
            }

            var roomTemplates = new HashSet<RoomTemplate>();
            foreach (var room in mapDescription.GetGraph().Vertices)
            {
                var roomDescription = mapDescription.GetRoomDescription(room);

                //if (roomDescription is BasicRoomDescription basicRoomDescription)
                //{
                //    foreach (var roomTemplate in basicRoomDescription.RoomTemplates)
                //    {
                //        roomTemplates.Add(roomTemplate);
                //    }
                //}
                if (roomDescription is CorridorRoomDescription corridorRoomDescription)
                {
                    foreach (var roomTemplate in corridorRoomDescription.RoomTemplates)
                    {
                        roomTemplates.Add(roomTemplate);
                    }
                }
            }

            var roomTemplateNames = new List<string>()
            {
                "Normal 1",
                "Normal 1 - duplicate",
                "Normal 2",
                "Normal 3",
                "Normal 4",
                "Normal 5",
                "Normal 6",
                "Spawn",
                "Hub 1",
                "Hub 3 - duplicate 1",
                "Hub 3 - duplicate 2",
                "Reward",
                "Normal 7 small",
                "Normal 8",
                "Normal 7 small - duplicate",
                "Reward - duplicate",
                "Secret",
            };

            // Console.WriteLine("var roomTemplates = new Dictionary<string, RoomTemplateGrid2D>()");
            Console.WriteLine("var roomTemplates = new List<RoomTemplateGrid2D>()");
            Console.WriteLine("{");

            var counter = 0;
            foreach (var roomTemplate in roomTemplates)
            {
                // Console.WriteLine($"  {{\"{roomTemplateNames[counter++]}\", new RoomTemplateGrid2D(");
                Console.WriteLine($"  new RoomTemplateGrid2D(");
                Console.WriteLine("    new PolygonGrid2DBuilder()");

                var points = roomTemplate.Shape.GetPoints();
                for (var i = 0; i < points.Count; i++)
                {
                    var point = points[i];
                    if (i % 4 == 0)
                    {
                        if (i != 0)
                        {
                            Console.WriteLine();
                        }
                        Console.Write("      ");
                    }

                    Console.Write($".AddPoint({point.X}, {point.Y})");
                }

                Console.WriteLine();
                Console.WriteLine("      .Build(),");

                if (roomTemplate.DoorsMode is SimpleDoorMode simpleDoorMode)
                {
                    Console.WriteLine($"    new SimpleDoorModeGrid2D({simpleDoorMode.DoorLength}, {simpleDoorMode.CornerDistance})");
                } 
                else if (roomTemplate.DoorsMode is ManualDoorMode manualDoorMode)
                {
                    Console.WriteLine("    new ManualDoorModeGrid2D(new List<DoorGrid2D>()");
                    Console.WriteLine("      {");

                    foreach (var doorPosition in manualDoorMode.DoorPositions)
                    {
                        Console.WriteLine($"      new DoorGrid2D(new Vector2Int({doorPosition.From.X}, {doorPosition.From.Y}), new Vector2Int({doorPosition.To.X}, {doorPosition.To.Y})),");
                    }

                    Console.WriteLine("      }");
                    Console.WriteLine("    )");
                }

                // Console.WriteLine("  )},");
                Console.WriteLine("  ),");

                
            }

            Console.WriteLine("};");
        }
    }
}