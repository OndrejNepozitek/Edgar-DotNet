# ProceduralLevelGenerator

[![Gitter chat](https://badges.gitter.im/OndrejNepozitek/ProceduralLevelGenerator.png)](https://gitter.im/ProceduralLevelGenerator/community)

This project is a library for procedural generation of 2D layouts based on a graph of rooms connections.

To produce a game level, the algorithm takes a set of polygonal building blocks and a level connectivity graph (the level topology) as an input. Nodes in the graph represent rooms, and edges define connectivities between them. The graph has to be planar. The goal of the algorithm is to assign a room shape and a position to each node in the graph in a way that no two room shapes intersect and every pair of neighbouring room shapes can be connected by doors.

## See the documentation [here](https://ondrejnepozitek.github.io/ProceduralLevelGenerator/docs/introduction)

## Features
- Any planar connected graph can be used as an input
- Any orthogonal polygon can be used as a room shape
- Complete control over shapes of individual rooms
- Complete control over door positions of individual room shapes
- Rooms either directly connected by doors or connected by corridors
- Export to JSON, SVG, JPG
- Majority of features available through a GUI and YAML config files
- Implicit support for keys and locks - just define the connectivity graph hovewer you like

## Current state of the project
The library should be functional, but is far from perfect. There are quite a few places that I know need an improvement and I have a bunch of new features in mind that will hopefully get implemented in not too distant future. I would also love to get any **feedback** - either on the features of the algorithm or on how usable it is from a programmer's point of view. If you have any questions or suggestions, you can either create an issue or contact me on [gitter](https://gitter.im/OndrejNepozitek/ProceduralLevelGenerator).

## Bachelor thesis and paper
This library was created as a part of my bachelor thesis. Text of the thesis can be found [here](https://github.com/OndrejNepozitek/ProceduralLevelGenerator/blob/text/bachelor_thesis.pdf). After completing the thesis, we decided to transform it to a paper and submit it to the Game-On 2018 conference. The extended version of the paper can be found [here](https://github.com/OndrejNepozitek/ProceduralLevelGenerator/blob/text/extended_paper.pdf). This version also contains a practical use-case of the algorithm, which is not present in the regular version due to length limitations.

## Inspiration
The main idea of the algorithm used in this library comes from a [paper](http://chongyangma.com/publications/gl/index.html) written by **Chongyang Ma** and colleagues so be sure to check their work out.

Some things in this library are done differently and/or improved:
- **Integer coordinates** are used instead of reals - room shapes are therefore only orthogonal polygons.
- With integer coordinates, **optimized polygon operations** (intersections, etc..) were implemented with a complete control over the process.
- User has a complete control over door positions of rooms.
- The algorithm was optimized to generate a layout as fast as possible.
- A specialized version of the generator was implemented to support **adding (usally) short corridors** between rooms to the layout without sacrificing most of the convergence speed. (Average number of iterations usually stays the same but iterations themselves are slower.)

## Examples

### Input

![alt-text](https://ondrejnepozitek.github.io/ProceduralLevelGenerator/docs/assets/introduction/introduction.svg)

### Results

<div class="results">
  <a href="https://ondrejnepozitek.github.io/ProceduralLevelGenerator/docs/assets/introduction/0.jpg" target="_blank"><img width="24%" src="https://ondrejnepozitek.github.io/ProceduralLevelGenerator/docs/assets/introduction/0.jpg" alt="result"></a>
  <a href="https://ondrejnepozitek.github.io/ProceduralLevelGenerator/docs/assets/introduction/1.jpg" target="_blank"><img width="24%" src="https://ondrejnepozitek.github.io/ProceduralLevelGenerator/docs/assets/introduction/1.jpg" alt="result"></a>
  <a href="https://ondrejnepozitek.github.io/ProceduralLevelGenerator/docs/assets/introduction/2.jpg" target="_blank"><img width="24%" src="https://ondrejnepozitek.github.io/ProceduralLevelGenerator/docs/assets/introduction/2.jpg" alt="result"></a>
  <a href="https://ondrejnepozitek.github.io/ProceduralLevelGenerator/docs/assets/introduction/3.jpg" target="_blank"><img width="24%" src="https://ondrejnepozitek.github.io/ProceduralLevelGenerator/docs/assets/introduction/3.jpg" alt="result"></a>
</div>

**Note:** Click on images to see them in a full size.
