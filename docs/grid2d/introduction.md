---
title: Guides
sidebar_label: Introduction
---

This section will walk you through the process of creating so-called **map descriptions** which are used to describe the structure and look of generated layouts.

## Vocabulary

- **Rooms** and **connections** describe the high-level structure of generated layouts.
- **Room template** describes one possible shape of a room. Each room template consists of:
    - **Polygon** which describes the outline of the room template.
    - List of possible **door positions**.
    - List of allowed **transformations**. E.g. that the room template can be rotated by 180 degrees.
- **Room description** is a set of room templates that describes all the possible shapes of a single room.
- **Map description** holds the whole graph of rooms and connections together with room descriptions for each room.

> **Note:** In the older version of the library, it was also possible to use YAML config files to construct map descriptions. However, YAML support was dropped in favor of C# API as it is more flexible and it is currently not possible to support both.

```csharp
/// <summary>
/// Adds a given room to the map description
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="room">The room to be added.</param>
/// <param name="roomDescription">Corresponding room description that describes the look of the room.</param>
public void AddRoom(TRoom room, IRoomDescription roomDescription)
```