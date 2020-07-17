---
title: Different room descriptions
sidebar_label: Different room descriptions
---

import useBaseUrl from "@docusaurus/useBaseUrl";
import { Gallery, GalleryImage } from "@theme/Gallery";

User can easily specify that some rooms should have different shapes from the others. It can be useful for example when you want to have a special room template for your boss room or any other room with a special meaning.

## Setup
In this tutorial, we will use a graph with 9 vertices codenamed *Example2*:

<img style={{marginBottom: 20}} alt="Graph" src={useBaseUrl('img/graphs/example2.svg')} />

The room number 8 will be our boss room and all the other rooms will have basic square and rectangular shapes as they are not important for this tutorial.

## Prepare room descriptions

In this tutorial, we imagine that the room with number 8 is a boss room and that we want to make it look special. We first create a room template for our boss room:

```csharp
var bossRoom = new RoomTemplate(
  new GridPolygonBuilder()
    .AddPoint(2, 0).AddPoint(2, 1).AddPoint(1, 1).AddPoint(1, 2)
    .AddPoint(0, 2).AddPoint(0, 7).AddPoint(1, 7).AddPoint(1, 8)
    .AddPoint(2, 8).AddPoint(2, 9).AddPoint(7, 9).AddPoint(7, 8)
    .AddPoint(8, 8).AddPoint(8, 7).AddPoint(9, 7).AddPoint(9, 2)
    .AddPoint(8, 2).AddPoint(8, 1).AddPoint(7, 1).AddPoint(7, 0)
    .Build().Scale(new IntVector2(2, 2)),
  new SimpleDoorMode(1, 1)
);
```

Then we create a room description from that template:

```csharp
var bossRoomDescription = new BasicRoomDescription(new List<IRoomTemplate>() { bossRoom });
```

And finally we create a room description for our basic rooms:

```csharp
var squareRoom = ;// Setup square room
var rectangleRoom = ;// Setup rectanble room
var basicRoomDescription = new BasicRoomDescription(new List<IRoomTemplate>() { squareRoom, rectangleRoom });
```

## Prepare map description

Now we just make sure to use correct room descriptions:

```csharp
// Create map description
var mapDescription = new MapDescription<int>();

// Add boss room
mapDescription.AddRoom(8, bossRoomDescription);

// Add other rooms
for (int i = 0; i < 8; i++)
{
    mapDescription.AddRoom(i, basicRoomDescription);
}

// Add connections
```

And the map description is ready to be used.

## Results

<Gallery cols={4}>
  <GalleryImage src="img/differentShapes/0.jpg" />
  <GalleryImage src="img/differentShapes/1.jpg" />
  <GalleryImage src="img/differentShapes/2.jpg" />
  <GalleryImage src="img/differentShapes/3.jpg" />
</Gallery>

> **Note:** You can find the full C# source code [here](https://github.com/OndrejNepozitek/ProceduralLevelGenerator/blob/master/Sandbox/Examples/DifferentRoomDescriptionsExample.cs).