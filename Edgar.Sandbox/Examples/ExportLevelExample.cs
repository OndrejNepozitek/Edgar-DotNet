using System;
using System.IO;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Utils.MapDrawing;
using Newtonsoft.Json;

namespace Sandbox.Examples
{
    public class ExportLevelExample
    {
        public void Run()
        {
            // Create map description
            var mapDescription = new CorridorsExample().GetMapDescription();

            // Instantiate dungeon generator
            var generator = new DungeonGenerator<int>(mapDescription);
            generator.InjectRandomGenerator(new Random(0));

            // Generate level
            var layout = generator.GenerateLayout();

            // Export to svg and save to file
            var svgDrawer = new SVGLayoutDrawer<int>();
            var svg = svgDrawer.DrawLayout(layout, 800, forceSquare: true);
            File.WriteAllText("generated_level.svg", svg);

            // Export to json and save to file
            var json = JsonConvert.SerializeObject(layout, Formatting.Indented);
            File.WriteAllText("generated_level.json", json);
        }
    }
}