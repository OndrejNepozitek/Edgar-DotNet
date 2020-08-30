---
title: Introduction
sidebar_label: Introduction
---

import useBaseUrl from "@docusaurus/useBaseUrl";
import { Gallery, GalleryImage } from "@theme/Gallery";

<img src="https://ondrejnepozitek.github.io/Edgar-DotNet/readme/example_1.png" width="32%" /> <img src="https://ondrejnepozitek.github.io/Edgar-DotNet/readme/example_2.png" width="32%" /> <img src="https://ondrejnepozitek.github.io/Edgar-DotNet/readme/example_3.png" width="32%" />

<p style={{ textAlign: 'center'}}><i>(Design of exported levels inspired by <a href="https://watabou.itch.io/one-page-dungeon">Watabou's One Page Dungeon</a>)</i></p>

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

## Examples

<Gallery cols={2}>
  <GalleryImage src={require('./grid2d/basics/0_0.png').default} />
  <GalleryImage src={require('./grid2d/basics/0_1.png').default} />
  <GalleryImage src={require('./grid2d/minimum-room-distance/2_0.png').default} />
  <GalleryImage src={require('./grid2d/minimum-room-distance/2_1.png').default} />
  <GalleryImage src={require('./grid2d/complex-dungeon/0_0.png').default} />
  <GalleryImage src={require('./grid2d/complex-dungeon/0_1.png').default} />
</Gallery>