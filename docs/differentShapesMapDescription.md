---
id: differentShapesMapDescription
title: Different shapes
sidebar_label: Different shapes
---

import useBaseUrl from "@docusaurus/useBaseUrl";
import { Gallery, GalleryImage } from "@theme/Gallery";

User can easily specify that some rooms should have different shapes from the others. It can be useful for example when you want to set a special rooms shape for your boss room or any other rooms with a special meaning.

## Setup
In this tutorial, we will use a graph with 9 vertices codenamed *Example2*:

<img alt="Graph" src={useBaseUrl('img/graphs/example2.svg')} />;

We will use basic default shapes for rooms - a square and a rectangle - as they are not important for this tutorial.

## Adding custom shapes

In this tutorial, we imagine that the room with number 8 is a boss In  room and that we want to make it look special.

### Using C# api

In the code snippet below you can see that we used a special overload of `AddRoomShapes` to add a shape for a specific node.

```csharp
var mapDescription = new MapDescription<int>();

// Graph and default shapes are set here.

// Add boss room shape
var bossRoom = new RoomDescription(
  new GridPolygonBuilder()
    .AddPoint(2, 0).AddPoint(2, 1).AddPoint(1, 1).AddPoint(1, 2)
    .AddPoint(0, 2).AddPoint(0, 7).AddPoint(1, 7).AddPoint(1, 8)
    .AddPoint(2, 8).AddPoint(2, 9).AddPoint(7, 9).AddPoint(7, 8)
    .AddPoint(8, 8).AddPoint(8, 7).AddPoint(9, 7).AddPoint(9, 2)
    .AddPoint(8, 2).AddPoint(8, 1).AddPoint(7, 1).AddPoint(7, 0)
  .Build().Scale(new IntVector2(2, 2)),
  new OverlapMode(1, 1)
);

mapDescription.AddRoomShapes(8, bossRoom);
```

### Using config files

In the config file below you can see 2 new things:
- We do not have to always create a file for our room descriptions. If we have a room description that is specific for the current map description, we can put it in the same file under the `customRoomDescriptionsSet` section. This set can be referred to with the name `custom`. This approach is also good if we want to contain the whole map description in one file.
- We created a new section named `rooms`. This section is used to define behaviour that is specific for a given node. In our case, we specified that the node _8_ should use our _bossRoom_ shape instead of default room shapes.

```yaml
# Resources/Maps/tutorial_differentShapes.yml

# Rooms and passages are added here.

# Default room shapes are added here.

rooms:
  [8]:
    roomShapes:
      -
        setName: custom
        roomDescriptionName: bossRoom
        scale: [2,2]

customRoomDescriptionsSet:
  roomDescriptions:
    bossRoom:
      shape: [
          [2,0], [2,1], [1,1], [1,2],
          [0,2], [0,7], [1,7], [1,8],
          [2,8], [2,9], [7,9], [7,8],
          [8,8], [8,7], [9,7], [9,2],
          [8,2], [8,1], [7,1], [7,0]
      ]
      doorMode: !OverlapMode
        cornerDistance: 1
        doorLength: 1
```

## Summary
The map description is now ready to be used in a layout generator. You can find the full C# source code [here](https://github.com/OndrejNepozitek/ProceduralLevelGenerator/blob/master/Sandbox/Examples/DifferentShapesExample.cs) and the config files [here](https://github.com/OndrejNepozitek/ProceduralLevelGenerator/tree/master/Resources).

## Results

<Gallery cols={2}>
  <GalleryImage src="img/differentShapes/0.jpg" />
  <GalleryImage src="img/differentShapes/1.jpg" />
  <GalleryImage src="img/differentShapes/2.jpg" />
  <GalleryImage src="img/differentShapes/3.jpg" />
</Gallery>

**Note:** Click on images to see them in a full size.