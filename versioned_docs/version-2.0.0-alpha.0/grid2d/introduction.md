---
title: Guides
sidebar_label: Introduction
---

This section will walk you through the basics of the generator.

- [Basics](./basics) - how to create a very simple level with rectangular room templates
- [Corridors](./corridors) - how to add corridors to a level
- [Minimum room distance](./minimum-room-distance) - how to control the minimum distance of rooms
- [Complex dungeon setup](./complex-dungeon-setup) - how to setup a complex dungeon with different types of rooms
- [Save and load](./save-load) - how to save and load level descriptions and generated layouts

## Vocabulary

- **Room template** describes one possible shape of a room. Each room template consists of:
    - **polygon** which describes the outline of the room template.
    - list of available **door positions**
    - list of allowed **transformations**, e.g. that the room template can be rotated by 180 degrees
- **Room description** describes the properties of a single room in a level, and it controls the following:
    - which room templates are available for the room
    - whether the room is a corridor or not
- **Level description** is a graph-like data structure that controls the following:
    - how many rooms are there in a level and what are their room descriptions
    - how are rooms connected
    - additonal properties like the minimum distance of rooms

> **Note:** In the older version of the library, it was also possible to use YAML config files to construct map descriptions. However, YAML support was dropped in favor of C# API as it is more flexible and it is currently not possible to support both.
