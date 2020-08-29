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

You decide exactly how many rooms you want in a level and how they should be connected, and the generator produces levels that follow exactly that structure. Do you want a boss room at the end of each level? Or a shop room halfway through the level? Everything is possible with a graph-based approach.

### Handmade room templates

The appearance of individual rooms is controlled with so-called room templates. These are pre-authored building blocks from which the algorithm chooses when generating a level. They are created with Unity tilemaps, but they can also contain additional game objects such as lights, enemies or chests with loot. You can also assign different room templates to different types of rooms. For example, a spawn room should probably look different than a boss room.

## Key features

- Procedural dungeon generator
- Describe the structure of levels with a graph of rooms and connections 
- Control the appearance of rooms with handmade room templates 
- Connect rooms either directly with doors or with short corridors
- Easy to customize with custom post-processing logic
- Supports Unity 2018.4 and newer
- Currently works only in 2D but may support 3D in future
- Comprehensive [documentation](https://ondrejnepozitek.github.io/Edgar-Unity/docs/introduction/)
- Multiple example scenes included

## PRO version

There are two versions of this asset - free version and PRO version. The free version contains the core functions of the generator and should be fine for simple procedural dungeons. The PRO version can be bought at [itch.io](https://ondrejnepozitek.itch.io/edgar-pro) and contains some additional features. As of now, the PRO version contains features like platformer generator or isometric levels and also two advanced example scenes. If you like this asset, please consider buying the PRO version to support the development.

- Coroutines - Call the generator as a coroutine so that the game does not freeze when generating a level ([docs](https://ondrejnepozitek.github.io/Edgar-Unity/docs/generators/dungeon-generator#pro-with-coroutines))
- Custom rooms - It is possible to add additional fields to rooms and connections in a level graph ([docs](https://ondrejnepozitek.github.io/Edgar-Unity/docs/basics/level-graphs#pro-custom-rooms-and-connections))      
- Platformers - Generator that is able to produce platformer levels ([docs](https://ondrejnepozitek.github.io/Edgar-Unity/docs/generators/platformer-generator), [example](https://ondrejnepozitek.github.io/Edgar-Unity/docs/examples/platformer-1))
- Isometric - Simple example of isometric levels ([example](https://ondrejnepozitek.github.io/Edgar-Unity/docs/examples/isometric-1))
- Dead Cells - Tutorial on how to generate levels that are similar to Dead Cells ([docs](https://ondrejnepozitek.github.io/Edgar-Unity/docs/examples/dead-cells))
- Enter the Gungeon - Tutorial on how to generate levels that are similar to Enter the Gungeon ([docs](https://ondrejnepozitek.github.io/Edgar-Unity/docs/examples/enter-the-gungeon/))
- Custom input - Modify a level graph before it is used in the generator (e.g. add a random secret room) ([docs](https://ondrejnepozitek.github.io/Edgar-Unity/docs/generators/custom-input))
- (Planned) Fog of War
- (Planned) Additional generators

## Limitations

- Still in alpha version - there may be some breaking changes in the API
- Some level graphs may be too hard for the generator - see the [guidelines](https://ondrejnepozitek.github.io/Edgar-Unity/docs/basics/performance-tips)
- The graph-based approach is not suitable for large levels - we recommend less than 30 rooms
- Not everything can be configured via editor - some programming knowledge is needed for more advanced setups
                      
## Getting started

Install the asset (instructions are below) and head to the [documentation](https://ondrejnepozitek.github.io/Edgar-Unity/docs/introduction). The documentation describes all the basics and also multiple example scenes that should help you get started. 
                      
## Installation

There are several ways of installing the plugin:

### via Package Manager
Add the following line to the `packages/manifest.json` file under the `dependencies` section (you must have git installed):
```
 "com.ondrejnepozitek.procedurallevelgenerator": "https://github.com/OndrejNepozitek/ProceduralLevelGenerator-Unity.git#upm"
```
To try the examples, go to the Package Manager, find this plugin in the list of installed assets and import examples.

> Note: When importing the package, I've got some weird "DirectoryNotFoundException: Could not find a part of the path" errors even though all the files are there. If that happens to you, just ignore that.

#### How to update
After installing the package, Unity adds something like this to your `manifest.json`:

```
  "lock": {
    "com.ondrejnepozitek.procedurallevelgenerator": {
      "hash": "fc2e2ea5a50ec4d1d23806e30b87d13cf74af04e",
      "revision": "upm"
    }
  }
```

Remove it to let Unity download a new version of the plugin.

### via .unitypackage

Go to Releases and download the unitypackage that's included in every release. Then import the package to Unity project (*Assets -> Import package -> Custom package*).

#### How to update
In order to be able to download a new version of the plugin, **we recommend to not change anything inside the Assets/ProceduralLevelGenerator folder**. At this stage of the project, files are often moved, renamed or deleted, and Unity does not handle that very well.

The safest way to update to the new version is to completely remove the old version (*Assets/ProceduralLevelGenerator* directory) and then import the new version. (Make sure to backup your project before deleting anything.)                                                         
                                                             
## Workflow 

### 1. Draw rooms and corridors

![](https://ondrejnepozitek.github.io/Edgar-Unity/img/v2/room_templates_multiple.png)

### 2. Prepare the structure of the level

<img src="https://ondrejnepozitek.github.io/Edgar-Unity/img/v2/examples/example1_level_graph2.png" height="500" />

### 3. Generate levels

![](https://ondrejnepozitek.github.io/Edgar-Unity/img/v2/generated_levels_multiple.png)

## Examples

![](https://ondrejnepozitek.github.io/Edgar-Unity/img/original/example1_result1.png)

![](https://ondrejnepozitek.github.io/Edgar-Unity/img/original/example1_result_reallife1.png)

![](https://ondrejnepozitek.github.io/Edgar-Unity/img/original/example2_result1.png)

![](https://ondrejnepozitek.github.io/Edgar-Unity/img/original/example2_result_reallife1.png)

## Get in touch

If you have any questions, let me know at ondra-at-nepozitek.cz or create an issue here on github.

## Terms of use

The plugin can be used in both commercial and non-commercial projects, but **cannot be redistributed or resold**. If you want to include this plugin in your own asset, please contact me, and we will figure that out.


