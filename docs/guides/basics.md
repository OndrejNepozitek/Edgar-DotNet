---
title: Basics
sidebar_label: Basics
---

import useBaseUrl from "@docusaurus/useBaseUrl";
import { Gallery, GalleryImage } from "@theme/Gallery";

In this tutorial, we will create a basic map description with 4 rooms that have rectangular shapes. The process has 2 steps - specifying the graph and assigning shapes to rooms.

## Room templates

First, we will create our room templates. We must create an instance of `RoomTemplate` class for each room room template. To do that, we need to create a *polygon* that defines the outline of the room template and also provide a list of possible door positions.

### Polygons
We define polygons with a sequence of their vertices. In this tutorial, we will use an *8x8 square* and a *6x10 rectangle* but it is possible to use any orthogonal polygon.

> **Note:** Orthogonal (or rectilinear) polygon is a polygon of whose edge intersections are at right angles. When on an integer grid, each side of an orthogonal polygon is aligned with one of the axes.

### Doors
`IDoorMode` is an interface that specifies door positions of a given polygon. The most simple *door mode* is the `SimpleDoorMode` - it lets us specify the lenght of doors and how far from corners they must be. In this tutorial, we will use doors with length of 1 unit and at least 1 unit away from corners.

> **Note:** There is also an additional door mode available - `ManualDoorMode`. This mode lets you specify exactly which door lines are to be used.

### (Optional) Allowed transformations

Optionally, it is also possible to let the algorithm apply some transformations to the room, e.g. rotate it by 90 degrees or mirror it by the X axis. The algorithm then also handles all the door positions automatically.

### Putting it all together

```csharp
var doorMode = new SimpleDoorMode(1, 1);

var squareRoom = new RoomTemplate(
  new GridPolygonBuilder()
    .AddPoint(0, 0)
    .AddPoint(0, 8)
    .AddPoint(8, 8)
    .AddPoint(8, 0)
    .Build(),
  doorMode
);

var rectangleRoom = new RoomTemplate(
  GridPolygon.GetRectangle(6, 10),
  doorMode,
  new List<Transformation>() { Transformation.Identity, Transformation.Rotate90 }
);
```

> **Note:** There are several ways of constructing polygons:
  - `GridPolygon.GetSquare(width)` for squares
  - `GridPolygon.GetRectangle(width, height)` for rectangles
  - `GridPolygonBuilder` with the `.AddPoint(x, y)`
  - or the `GridPolygon(IEnumerable<IntVector2> points)` constructor

## Room descriptions

There is one more step needed before we create the graph of rooms and connections. The algorithm works best if it can choose from multiple room templates when deciding how should a room look like. So we will create a so called *room description* which is simply a list of possible room templates which we later assign to a room.

In this tutorial, we do not have any special rooms that should look different than other rooms so we can create only a single room description and reuse it for all rooms.

```csharp
var basicRoomDescription = new BasicRoomDescription(new List<IRoomTemplate>() { squareRoom, rectangleRoom });
```

## Graph
Now we are ready to create the graph of rooms. Every vertex in the graph represents one room in the final layout. And every edge represents a connection of two rooms by doors. To make things simple, we will use a 4-cycle in this tutorial:

<img style={{marginBottom: 20}} alt="Graph" src={useBaseUrl('img/basics/graph.svg')} />

First, we will create an instance of the `MapDescription` class:

```csharp
var mapDescription = new MapDescription<int>();
```

Then we will add all the rooms together with our room description:

```csharp
mapDescription.AddRoom(0, basicRoomDescription);
mapDescription.AddRoom(1, basicRoomDescription);
mapDescription.AddRoom(2, basicRoomDescription);
mapDescription.AddRoom(3, basicRoomDescription);
```

And finally we add all the connections:

```csharp
mapDescription.AddConnection(0, 1);
mapDescription.AddConnection(0, 3);
mapDescription.AddConnection(1, 2);
mapDescription.AddConnection(2, 3);
```

The map description is now ready to be used in the generator.

> **Note:** For simplicity, we use the `int` version of the map description where each room is represented by an integer. But if we want to store additional data for each room, we can use the map description with a custom room type.

## Generating layouts

Now we are ready to create an instance of the dungeon generator and generate a layout:

```csharp
var generator = new DungeonGenerator<int>(mapDescription);
var layout = generator.GenerateLayout();
```

## Results

<Gallery cols={4}>
  <GalleryImage src="img/basics/0.jpg" />
  <GalleryImage src="img/basics/1.jpg" />
  <GalleryImage src="img/basics/2.jpg" />
  <GalleryImage src="img/basics/3.jpg" />
</Gallery>

> **Note:** You can find the full C# source code [here](https://github.com/OndrejNepozitek/ProceduralLevelGenerator/blob/master/Sandbox/Examples/BasicsExample.cs).