---
id: basicMapDescription
title: Basics
sidebar_label: Basics
---

In this tutorial, we will create a basic map description with 4 rooms that have rectangular shapes. The process has 2 steps - specifying the graph and assigning shapes to rooms.

## Graph
First, we must create the underlying graph of rooms. Every vertex in the graph represents one room in the final layout. And every edge represents a connection of two rooms by doors. To make things simple, we will use a 4-cycle in this tutorial:

![alt-text](assets/basics/graph.svg)

Let's create the graph:

### Using C# api
```csharp
var mapDescription = new MapDescription<int>();

// Add rooms ( - you would normally use a for cycle)
mapDescription.AddRoom(0);
mapDescription.AddRoom(1);
mapDescription.AddRoom(2);
mapDescription.AddRoom(3);

// Add passages
mapDescription.AddPassage(0, 1);
mapDescription.AddPassage(0, 3);
mapDescription.AddPassage(1, 2);
mapDescription.AddPassage(2, 3);
```

### Using config files
```yaml
# basicDescription.yml

# We want 4 vertices
roomsRange:
  from: 0
  to: 3

# Define passages
passages:
  - [0,1]
  - [0,3]
  - [1,2]
  - [2.3]
```

## Room shapes
The next step is to add room shapes. We must create an instance of a `RoomDescription` class for each room shape. For that, we need a *polygon* and a *door mode*. 

### Polygons
We define polygons by a sequence of their vertices. In this tutorial, we will use an *8x8 square* and a *6x10 rectangle* but it is possible to use any orthogonal polygon.

**Note:** Orthogonal (or rectilinear) polygon is a polygon of whose edge intersections are at right angles. When on an integer grid, all sides of orthogonal polygons are aligned with one of the axes.

### Door modes
`IDoorMode` is an interface that specifies door positions of a given polygon. The most simple *door mode* is an `OverlapMode` - it lets us specify the lenght of doors and how far from corners they must be. In this tutorial, we will use doors with length of 1 unit and at least 1 unit away from corners.

**Note:** There is currently an additional door mode available - `SpecificPositionsMode`. This mode lets you specify exactly which door lines are to be used.

Lets put it together:

### Using C# api
```csharp
// Rooms and passages are added here.

// Add room shapes
var doorMode = new OverlapMode(1, 1);

var squareRoom = new RoomDescription(
  GridPolygon.GetSquare(8),
  doorMode
);
var rectangleRoom = new RoomDescription(
  GridPolygon.GetRectangle(6, 10),
  doorMode
);

mapDescription.AddRoomShapes(squareRoom);
mapDescription.AddRoomShapes(rectangleRoom);
```

### Using config files
We first create a *basic_rooms.yml* file under the *Resources/Rooms* folder. Here we add our room descriptions:

```yaml
# basicRooms.yml

name: basicRooms
roomDescriptions:
  # Create 8x8 square
  8-square:
    shape: [[0,0], [0,8], [8,8], [8,0]]
    doorMode: !OverlapMode
      doorLength: 1
      cornerDistance: 1
  # Create 6x10 rectangle
  6-10-rectangle:
    shape: [[0,0], [0,10], [6,10], [6,0]]
    doorMode: !OverlapMode
      doorLength: 1
      cornerDistance: 1
```

And now we have to register them to be used in our map description:

```yaml
# basicDescription.yml

# Rooms and passages are added here.

defaultRoomShapes:
  - setName: basicRooms
```

## Summary
The map description is now ready to be used in a layout generator. You can find the full C# source code [here TODO](TODO) and the config file [here TODO](TODO).

## Results

<div class="results">
  <a href="/MapGeneration/docs/assets/basics/0.jpg" target="_blank">
    <img src="/MapGeneration/docs/assets/basics/0.jpg" alt="result">
  </a>
  <a href="/MapGeneration/docs/assets/basics/1.jpg" target="_blank">
    <img src="/MapGeneration/docs/assets/basics/1.jpg" alt="result">
  </a>
  <a href="/MapGeneration/docs/assets/basics/2.jpg" target="_blank">
    <img src="/MapGeneration/docs/assets/basics/2.jpg" alt="result">
  </a>
  <a href="/MapGeneration/docs/assets/basics/3.jpg" target="_blank">
    <img src="/MapGeneration/docs/assets/basics/3.jpg" alt="result">
  </a>
</div>

**Note:** Click on images to see them in a full size.