---
id: differentShapesGuide
title: Different shapes for some rooms
sidebar_label: Different shapes for some rooms
---

User can easily specify that some rooms should have different shapes than the others. It can be useful for example when you want to set a special rooms shape for your boss room.

## Examples
Following examples both specify that a room with id 10 should have a special room shape and other rooms should use default room shapes.

### Using C# api

```csharp
// Program.cs

internal class Program
{
  private static void Main(string[] args)
  {
    var mapDescription = new MapDescription<int>();

    // Add rooms here
    // Add passages here
    // Add default room shapes here

    var bossRoom = new RoomDescription(
      new GridPolygonBuilder()
        .AddPoint(0, 0)
        .AddPoint(0, 6)
        .AddPoint(6, 6)
        .AddPoint(6, 0)
        .Build(),
      new OverlapMode(1, 1)
    );

    mapDescription.AddRoomShapes(10, bossRoom);
  }
}
```

### Using config files
```yaml
# mapDescription.yml

# define rooms range here
# define passages here
# define default room shapes here

rooms:
  10:
    roomShapes:
      -
        roomDescriptionName: bossRoom

customRoomDescriptionsSet:
  roomDescriptions:
    bossRoom:
      shape: [[0,0], [0,6], [6,6], [6,0]]
      doorsMode: !!OverlapMode
        minimumOverlap: 1
        doorLength: 1
```