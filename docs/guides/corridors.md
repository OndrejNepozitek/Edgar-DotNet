---
title: Corridors
sidebar_label: Corridors
---

import useBaseUrl from "@docusaurus/useBaseUrl";
import { Gallery, GalleryImage } from "@theme/Gallery";

Rooms in generated layouts can be either directly connected by doors or connected by corridors. In this tutorial, we will look into how to connect rooms by corridors.

## Setup
In this tutorial, we will use a graph with 17 vertices codenamed *Example1*:

<img style={{ marginBottom: "20px" }} alt="Graph" src={useBaseUrl('img/graphs/example1.svg')} />;

We will use basic default shapes for rooms - a square and a rectangle - as they are not important for this tutorial.

## Introduction

You may think that we already know everything to add corridors - we know how to use different room template for different nodes so we can insert a new node between each pair of neighboring rooms and use special corridor room templates. However, by doing so, we may end up with twice as many vertices as were in the original graph, which is not good for the performance of the algorithm.

Instead, we will use the `CorridorRoomDescription` to mark corridor rooms and let the algorithm know that it can optimize the process of laying corridors out.

> **Note:** Even though the library can handle corridors of arbitrary shapes, you should not get too wild if you care about the performance of the algorithm. Please refer to the [Performance tips](performance-tips#corridors) guide to see how to properly design corridors. The rule of thumb is to use straight not too long corridors.

## Adding corridors

In this tutorial, we will use rectangular corridors with width 1 and height 2. We first create the room template:

```csharp
var corridorRoom1x2 = new RoomTemplate(
  GridPolygon.GetRectangle(1, 2),
  new ManualDoorMode(new List<OrthogonalLine>()
  {
    new OrthogonalLine(new IntVector2(0, 0), new IntVector2(1, 0)),
    new OrthogonalLine(new IntVector2(0, 2), new IntVector2(1, 2))
  }),
  new List<Transformation>() { Transformation.Identity, Transformation.Rotate90 }
);
```

Then we create the room description. We must make sure to use the `CorridorRoomDescription` class:

```csharp
var corridorRoomDescription = new CorridorRoomDescription(new List<IRoomTemplate>() { corridorRoom1x2 });
```

Then we add non-corridor rooms:

```csharp
// Create map description
var mapDescription = new MapDescription<int>();
var graph = GraphsDatabase.GetExample1();

// Add non-corridor rooms
foreach (var room in graph.Vertices)
{
  mapDescription.AddRoom(room, basicRoomDescription);
}
```

> **Note:** To make this tutorial simpler, the graph is constructed somewhere else and here we just query its vertices and edges. It is absolutely valid to add all rooms and connections manually.

And finally we add corridor rooms:

```csharp
// We need to somehow identify our corridor rooms
// Here we simply number them and keep track which was the last used number
var counter = graph.VerticesCount;

foreach (var connection in graph.Edges)
{
  // We manually insert a new node between each neighboring nodes in the graph
  mapDescription.AddRoom(counter, corridorRoomDescription);

  // And instead of connecting the rooms directly, we connect them to the corridor room
  mapDescription.AddConnection(connection.From, counter);
  mapDescription.AddConnection(connection.To, counter);
  counter++;
}
```

> **Note:** **It is possible to have corridors between only some of the rooms and have the rest connected directly by doors.** In the older versions of the library, it was possible to either have corridors between all rooms or do not have corridors at all.

## Results

<Gallery cols={4}>
  <GalleryImage src="img/corridors/0.jpg" />
  <GalleryImage src="img/corridors/1.jpg" />
  <GalleryImage src="img/corridors/2.jpg" />
  <GalleryImage src="img/corridors/3.jpg" />
</Gallery>

### Longer corridors

We can also add some longer corridors:

```csharp
var corridorRoom1x3 = new RoomTemplate(
  GridPolygon.GetRectangle(1, 3),
  new ManualDoorMode(new List<OrthogonalLine>()
  {
      new OrthogonalLine(new IntVector2(0, 0), new IntVector2(1, 0)),
      new OrthogonalLine(new IntVector2(0, 3), new IntVector2(1, 3))
  }),
  new List<Transformation>() { Transformation.Identity, Transformation.Rotate90 }
);
```

<Gallery cols={4}>
  <GalleryImage src="img/corridors_longer/0.jpg" />
  <GalleryImage src="img/corridors_longer/1.jpg" />
  <GalleryImage src="img/corridors_longer/2.jpg" />
  <GalleryImage src="img/corridors_longer/3.jpg" />
</Gallery>

### L-shaped corridors

We can also add some non-straight corridors:

```csharp
var corridorRoomLShaped = new RoomTemplate(
  new GridPolygonBuilder()
    .AddPoint(0, 2)
    .AddPoint(0, 3)
    .AddPoint(3, 3)
    .AddPoint(3, 0)
    .AddPoint(2, 0)
    .AddPoint(2, 2)
    .Build(), 
  new ManualDoorMode(new List<OrthogonalLine>()
  {
    new OrthogonalLine(new IntVector2(0, 2), new IntVector2(0, 3)),
    new OrthogonalLine(new IntVector2(2, 0), new IntVector2(3, 0))
  }),
  TransformationHelper.GetAllTransformations().ToList()
);
```

<Gallery cols={4}>
  <GalleryImage src="img/corridors_l/0.jpg" />
  <GalleryImage src="img/corridors_l/1.jpg" />
  <GalleryImage src="img/corridors_l/2.jpg" />
  <GalleryImage src="img/corridors_l/3.jpg" />
</Gallery>