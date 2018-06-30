# ProceduralLevelGenerator - WIP

This project is a library for procedural generation of 2D layouts based on a graph of rooms connections.

## See the documentation [here](https://ondrejnepozitek.github.io/MapGeneration/docs/introduction.html)

## Features
- Any planar connected graph as an input
- Any orthogonal polygon as a room shape
- Some graph nodes having different shapes
- Specifying door positions of room shapes
- Rooms either directly connected by doors or connected by corridors
- Export to JSON, SVG, JPG
- Majority of features available through a GUI and YAML config files
- Implicit support for keys and locks - just define the graph hovewer you like

## Inspiration
The main idea of the algorithm used in this library comes from a [paper](http://chongyangma.com/publications/gl/index.html) written by **Chongyang Ma** and colleagues so be sure to check their work out.

Some things in this library are done differently and/or improved:
- **Integer coordinates** are used instead of reals - room shapes are therefore only orthogonal polygons.
- With integer coordinates, **optimized polygon operations** (intersections, etc..) were implemented with a complete control over the process.
- User has a complete control over door positions of rooms.
- The algorithm was optimized to generate a layout as fast as possible and nothing is then reused when generating more layouts. (As opposed to the original paper where following layouts are generated using already computed partial layouts from the process of generating the first layout. The reason for that is that I was not satisfied with the variability of such layouts.)
- A specialized version of the generator was implemented to support **adding (usally) short corridors** between rooms to the layout without sacrificing most of the convergence speed. (Average number of iterations usually stays the same but iterations themselves are slower.)

## Examples

### Input

![alt-text](https://ondrejnepozitek.github.io/MapGeneration/docs/assets/introduction/introduction.svg)

### Results

<div class="results">
  <a href="https://ondrejnepozitek.github.io/MapGeneration/docs/assets/introduction/0.jpg" target="_blank"><img width="24%" src="https://ondrejnepozitek.github.io/MapGeneration/docs/assets/introduction/0.jpg" alt="result"></a>
  <a href="https://ondrejnepozitek.github.io/MapGeneration/docs/assets/introduction/1.jpg" target="_blank"><img width="24%" src="https://ondrejnepozitek.github.io/MapGeneration/docs/assets/introduction/1.jpg" alt="result"></a>
  <a href="https://ondrejnepozitek.github.io/MapGeneration/docs/assets/introduction/2.jpg" target="_blank"><img width="24%" src="https://ondrejnepozitek.github.io/MapGeneration/docs/assets/introduction/2.jpg" alt="result"></a>
  <a href="https://ondrejnepozitek.github.io/MapGeneration/docs/assets/introduction/3.jpg" target="_blank"><img width="24%" src="https://ondrejnepozitek.github.io/MapGeneration/docs/assets/introduction/3.jpg" alt="result"></a>
</div>

**Note:** Click on images to see them in a full size.
