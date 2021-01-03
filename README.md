<h1 align="center">
  <br>
  Edgar for .NET
  <br>
</h1>

<h4 align="center">Graph-based procedural 2D layout generator</h4>

<p align="center">
  <a href="https://ondrejnepozitek.github.io/Edgar-DotNet/docs/introduction/"><img src="https://img.shields.io/badge/online-docs-important" /></a>
  <a href="https://github.com/OndrejNepozitek/Edgar-DotNet/workflows/Build/badge.svg"><img src="https://github.com/OndrejNepozitek/Edgar-DotNet/workflows/Build/badge.svg" /></a>
  <a href="https://www.nuget.org/packages/Edgar-DotNet"><img src="https://img.shields.io/nuget/vpre/Edgar-DotNet" /></a>
  <a href="https://github.com/OndrejNepozitek/Edgar-Unity"><img src="https://img.shields.io/badge/see also-Unity%20plugin-important" /></a>
  <a href="https://discord.gg/syktZ6VWq9"><img src="https://img.shields.io/discord/795301453131415554?label=discord" /></a>
</p>

<p align="center">
  <a href="#introduction">Introduction</a> |
  <a href="#key-features">Key features</a> |
  <a href="#limitations">Limitations</a> |
  <a href="#getting-started">Getting started</a> |
  <a href="#installation">Installation</a> |
  <a href="#workflow">Example</a> |
  <a href="#get-in-touch">Get in touch</a>
</p>

<!--
<p align="center">
  <a href="https://ondrejnepozitek.github.io/Edgar-Unity/">Website</a> |
  <a href="https://ondrejnepozitek.github.io/Edgar-Unity/docs/introduction/">Documentation</a> |
  <a href="https://github.com/OndrejNepozitek/Edgar-Unity/releases">Releases</a> |
  <a href="https://ondrejnepozitek.itch.io/edgar-pro">PRO version on itch.io</a> |
</p>
-->

<!--
<p align="center">
  Need info? Check the <a href="https://ondrejnepozitek.github.io/Edgar-Unity/docs/introduction/">docs</a> or <a href="https://ondrejnepozitek.github.io/Edgar-Unity/">website</a> |
  Or <a href="https://github.com/OndrejNepozitek/Edgar-Unity/issues/new">create an issue</a>
</p>
-->

<img src="https://ondrejnepozitek.github.io/Edgar-DotNet/readme/example_1.png" width="32%" /> <img src="https://ondrejnepozitek.github.io/Edgar-DotNet/readme/example_2.png" width="32%" /> <img src="https://ondrejnepozitek.github.io/Edgar-DotNet/readme/example_3.png" width="32%" />

<p align="center"><i>(Design of exported levels inspired by <a href="https://watabou.itch.io/one-page-dungeon">Watabou's One Page Dungeon</a>)</i></p>
                                                   
## Introduction

This project is a .NET library for procedural generation of 2D dungeons (and platformers) and aims to give game designers a **complete control** over generated levels. It combines **graph-based approach** to procedural generation with **handmade room templates** to generate levels with a **feeling of consistency**. If you are using Unity, make sure to check out [Edgar for Unity](https://github.com/OndrejNepozitek/Edgar-Unity) - Unity plugin based on this library. And I have also written a post about the graph-based approach on [my blog](https://ondra.nepozitek.cz/blog/42/dungeon-generator-part-1-node-based-approach/).

### Graph-based approach

You decide exactly how many rooms you want in a level and how they should be connected, and the generator produces layouts that follow exactly that structure. Do you want a boss room at the end of each level? Or a shop room halfway through the level? Everything is possible with a graph-based approach.

### Handmade room templates

The appearance of individual rooms is controlled with so-called room templates. These are pre-authored building blocks from which the algorithm chooses when generating a layout. Each room template consists of an outline polygon and a set of available door positions. You can also assign different room templates to different types of rooms. For example, a spawn room should probably look different than a boss room.

## CAUTION!

The library is currently being refactored to make the API nicer and make it easier to add new features in the future. As a result, only the most important logic is documented and I would not recommend looking into the insides of the algorithm. If you want to know how the algorithm works, check out my [blog post](https://ondra.nepozitek.cz/blog/42/dungeon-generator-part-1-node-based-approach/).

## Key features

- Procedural dungeon generator
- Describe the structure of levels with a graph of rooms and connections 
- Control the appearance of rooms with handmade room templates 
- Connect rooms either directly with doors or with short corridors
- Export to JSON or PNG
- Supports .NET Standard 2.0
- Currently works only on the 2D grid but may support 3D in future
- Comprehensive [documentation](https://ondrejnepozitek.github.io/Edgar-DotNet/docs/introduction/) with multiple examples
- Implicit support for keys and locks - just define the connectivity graph hovewer you like

## Limitations

- The library is currently being refactored - see the caution above.
- Some level graphs may be too hard for the generator - see the [guidelines](https://ondrejnepozitek.github.io/Edgar-DotNet/docs/basics/performance-tips)
- The graph-based approach is not suitable for large levels - we recommend less than 30 rooms
                      
## Getting started

Install the asset (instructions are below) and head to the [documentation](https://ondrejnepozitek.github.io/Edgar-DotNet/docs/introduction). The documentation describes all the basics and provides multiple examples.
                      
## Installation

Download the latest version from [Nuget](https://www.nuget.org/packages/Edgar-DotNet).                                                
                                                             
## Example

Below is a very simple setup of the generator. We create two rectangular room templates, add 4 rooms to the level description and connect the rooms so that they form a cycle. Be sure to check the [documentation](https://ondrejnepozitek.github.io/Edgar-DotNet/docs/introduction) to see multiple commented examples.

```csharp
// Create square room template
var squareRoomTemplate = new RoomTemplateGrid2D(
    PolygonGrid2D.GetSquare(8),
    new SimpleDoorModeGrid2D(doorLength: 1, cornerDistance: 1)
);

// Create rectangle room template
var rectangleRoomTemplate = new RoomTemplateGrid2D(
    PolygonGrid2D.GetRectangle(6, 10),
    new SimpleDoorModeGrid2D(doorLength: 1, cornerDistance: 1)
);

// Create a room description which says that the room is not a corridor and that it can use the two room templates
var roomDescription = new RoomDescriptionGrid2D(
    isCorridor: false,
    roomTemplates: new List<RoomTemplateGrid2D>() { squareRoomTemplate, rectangleRoomTemplate }
);

// Create an instance of the level description
var levelDescription = new LevelDescriptionGrid2D<int>();

// Add 4 rooms to the level, use the room description that we created beforehand
levelDescription.AddRoom(0, roomDescription);
levelDescription.AddRoom(1, roomDescription);
levelDescription.AddRoom(2, roomDescription);
levelDescription.AddRoom(3, roomDescription);

// Add connections between the rooms - the level graph will be a cycle with 4 vertices
levelDescription.AddConnection(0, 1);
levelDescription.AddConnection(0, 3);
levelDescription.AddConnection(1, 2);
levelDescription.AddConnection(2, 3);

// Create an instance of the generate and generate a layout
var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
var layout = generator.GenerateLayout();

// Export the resulting layout as PNG
var drawer = new DungeonDrawer<int>();
drawer.DrawLayoutAndSave(layout, "layout.png", new DungeonDrawerOptions()
{
    Width = 2000,
    Height = 2000,
});
```

Here are two layouts that were produced from this example:

<img src="https://ondrejnepozitek.github.io/Edgar-DotNet/readme/simple_layout.png" width="49%" /> <img src="https://ondrejnepozitek.github.io/Edgar-DotNet/readme/simple_layout_2.png" width="49%" />

## Get in touch

If you have any questions or want to show off what you created with Edgar, come chat with us in our [discord server](https://discord.gg/syktZ6VWq9). Or if you want to contact me personally, use my email ondra-at-nepozitek.cz or send me a message on Twitter at @OndrejNepozitek.


