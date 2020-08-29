<h1 align="center">
  <br>
  Edgar for .NET
  <br>
</h1>

<h4 align="center">Graph-based procedural 2D layout generator</h4>

<p align="center">
  <a href="https://ondrejnepozitek.github.io/Edgar-DotNet/docs/introduction/"><img src="https://img.shields.io/badge/online-docs-important" /></a>
  <a href="https://github.com/OndrejNepozitek/Edgar-Unity/workflows/Build/badge.svg"><img src="https://github.com/OndrejNepozitek/Edgar-Unity/workflows/Build/badge.svg" /></a>
  <a href="https://www.nuget.org/packages/Edgar-DotNet"><img src="https://img.shields.io/nuget/vpre/Edgar-DotNet" /></a>
  <a href="https://github.com/OndrejNepozitek/Edgar-Unity"><img src="https://img.shields.io/badge/see also-Unity%20plugin-important" /></a>
</p>

<p align="center">
  <a href="#introduction">Introduction</a> |
  <a href="#key-features">Key features</a> |
  <a href="#pro-version">PRO version</a> |
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
                                                   
## Introduction

This project is a .NET library for procedural generation of 2D dungeons (and platformers) and aims to give game designers a **complete control** over generated levels. It combines **graph-based approach** to procedural generation with **handmade room templates** to generate levels with a **feeling of consistency**. If you are using Unity, make sure to check out [Edgar for Unity](https://github.com/OndrejNepozitek/Edgar-Unity) - Unity plugin based on this library.

### Graph-based approach

You decide exactly how many rooms you want in a level and how they should be connected, and the generator produces layouts that follow exactly that structure. Do you want a boss room at the end of each level? Or a shop room halfway through the level? Everything is possible with a graph-based approach.

### Handmade room templates

The appearance of individual rooms is controlled with so-called room templates. These are pre-authored building blocks from which the algorithm chooses when generating a layout. Each room template consists of an outline polygon and a set of available door positions. You can also assign different room templates to different types of rooms. For example, a spawn room should probably look different than a boss room.

## Key features

- Procedural dungeon generator
- Describe the structure of levels with a graph of rooms and connections 
- Control the appearance of rooms with handmade room templates 
- Connect rooms either directly with doors or with short corridors
- Supports .NET Standard 2.0
- Currently works only in 2D but may support 3D in future
- Comprehensive [documentation](https://ondrejnepozitek.github.io/Edgar-DotNet/docs/introduction/) with multiple examples

## Limitations

- Still in alpha version - there may be some breaking changes in the API
- Some level graphs may be too hard for the generator - see the [guidelines](https://ondrejnepozitek.github.io/Edgar-Unity/docs/basics/performance-tips)
- The graph-based approach is not suitable for large levels - we recommend less than 30 rooms
- Not everything can be configured via editor - some programming knowledge is needed for more advanced setups
                      
## Getting started

Install the asset (instructions are below) and head to the [documentation](https://ondrejnepozitek.github.io/Edgar-Unity/docs/introduction). The documentation describes all the basics and also multiple example scenes that should help you get started. 
                      
## Installation

Download the latest version from [Nuget](https://www.nuget.org/packages/Edgar-DotNet).                                                
                                                             
## Example

Below is a very simple setup of the generator. We create two rectangular room templates, add 4 rooms to the level description and connect the rooms so that they form a cycle.

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

Here are two layouts that were produced by the generator:

<img src="https://ondrejnepozitek.github.io/Edgar-DotNet/readme/simple_layout.png" width="49%" /> <img src="https://ondrejnepozitek.github.io/Edgar-DotNet/readme/simple_layout_2.png" width="49%" />

## Examples

![](https://ondrejnepozitek.github.io/Edgar-Unity/img/original/example1_result1.png)

![](https://ondrejnepozitek.github.io/Edgar-Unity/img/original/example1_result_reallife1.png)

![](https://ondrejnepozitek.github.io/Edgar-Unity/img/original/example2_result1.png)

![](https://ondrejnepozitek.github.io/Edgar-Unity/img/original/example2_result_reallife1.png)

## Get in touch

If you have any questions, let me know at ondra-at-nepozitek.cz or create an issue here on github.

## Terms of use

The plugin can be used in both commercial and non-commercial projects, but **cannot be redistributed or resold**. If you want to include this plugin in your own asset, please contact me, and we will figure that out.


