---
title: Corridors
sidebar_label: Corridors
---

import useBaseUrl from "@docusaurus/useBaseUrl";
import { Gallery, GalleryImage } from "@theme/Gallery";

There are currently 2 layout generators implemented. The first one generates layouts where neighbouring rooms are connected by doors. The second one, on the other hand, connects neighbouring rooms by corridors.

In this tutorial, we will look into how to use the latter one.

## Setup
In this tutorial, we will use a graph with 17 vertices codenamed *Example1*:

<img alt="Graph" src={useBaseUrl('img/graphs/example1.svg')} />;

We will use basic default shapes for rooms - a square and a rectangle - as they are not important for this tutorial.

## Adding corridors

Corridors could be handled as normal rooms just with specific shapes. Hovewer, this approach is very slow as the total number of nodes in the graph becomes almost twice the original count (because map description needs connected graphs).

Instead, a completely different approach was implemented. The algoritm ignores corridor rooms and tries to get valid layouts consisting only of the ogirinal rooms without corridors. Hovewer, the algorithms does not place neighbouring rooms directly next to each other. It places such rooms in a way that they are specified number of units away from each other (this is reffered to be the *offset of configuration spaces* througnout this documentation). When a valid layout is found, the algorithm tries to greedily add corridor rooms.

**Note:** Currently, `MapDescription` class supports either not having corridors or adding a corridor between every pair of neighbouring rooms.

In this tutorial, we will set the offset of non corridor neighbours to be equal to 1 and then set room shapes of corridor rooms to be 1x1 squares.

Let's add corridors:

### Using C# api

First we have to obtain the right instance of `ChainBasedGenerator` class:

```csharp
var layoutGenerator = LayoutGeneratorFactory.GetChainBasedGeneratorWithCorridors<int>(new List<int>() { 1 });
```

And now we setup the map description:

```csharp
var mapDescription = new MapDescription<int>();

// Graph and default shapes are set here.

// Setup corridor shapes
var corridorRoom = new RoomDescription(
  GridPolygon.GetSquare(1),
  new SpecificPositionsMode(new List<OrthogonalLine>()
  {
    new OrthogonalLine(new IntVector2(0, 0), new IntVector2(1, 0)),
    new OrthogonalLine(new IntVector2(0, 1), new IntVector2(1, 1))
  })
);

mapDescription.AddCorridorShapes(corridorRoom);

// Enable corridors
mapDescription.SetWithCorridors(true, new List<int>() { 1 });
```

### Using config files
We first create a tutorial_corridors.yml file under the Resources/Rooms folder. Here we add room shapes for our corridors. We will create one shape - 1x1 square with doors on two opposite sides. 

```yaml
# Resources/Rooms/tutorial_corridors.yml

name: tutorial_corridors
roomDescriptions:
  1-square:
    shape: [[0,0], [0,1], [1,1], [1,0]]
    doorMode: !SpecificPositionsMode
      doorPositions: 
        - [[0,0], [1,0]]
        - [[0,1], [1,1]]
```
Now we create the map description:

```yaml
# Resources/Maps/tutorial_basic.yml

# Rooms and passages are added here.

# Default room shapes are added here.

corridors:
  enable: true
  offsets: [1]
  corridorShapes:
    -
      setName: tutorial_corridors
```

## Summary
The map description is now ready to be used in a layout generator. You can find the full C# source code [here](https://github.com/OndrejNepozitek/ProceduralLevelGenerator/blob/master/Sandbox/Examples/CorridorsExample.cs) and the config files [here](https://github.com/OndrejNepozitek/ProceduralLevelGenerator/tree/master/Resources).

Don't forget to use the appropriate layout generator if using the C# api.

## Results

<Gallery cols={4}>
  <GalleryImage src="img/corridors/0.jpg" />
  <GalleryImage src="img/corridors/1.jpg" />
  <GalleryImage src="img/corridors/2.jpg" />
  <GalleryImage src="img/corridors/3.jpg" />
</Gallery>


**Note:** Click on images to see them in a full size.