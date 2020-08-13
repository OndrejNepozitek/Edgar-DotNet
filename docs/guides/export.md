---
title: Exporting generated layouts
---

It is very simple to export a generated level to SVG:

    // Create map description
    var mapDescription = /* ... */

    // Instantiate dungeon generator
    var generator = new DungeonGenerator<int>(mapDescription);
    generator.InjectRandomGenerator(new Random(0));

    // Generate level
    var layout = generator.GenerateLayout();

    // Export to svg and save to file
    var svgDrawer = new SVGLayoutDrawer<int>();
    var svg = svgDrawer.DrawLayout(layout, 800, forceSquare: true);
    File.WriteAllText("generated_level.svg", svg);