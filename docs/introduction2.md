---
id: introduction2
title: Introduction
sidebar_label: Introduction
---

This section will walk you through the process of creating *map descriptions*. *Map description* is a data structure that holds all the information about how the final layout should look like. They are used as an input to the layout generator.

## Creating map descriptions
Creating map descriptions is a 2 step process:
1) Graph of rooms connections is created. The graph describes how many rooms are present in the layout and which rooms are connected by doors or corridors.
2) Room shapes are assigned to graph nodes. This process is very customizable and will be explained in detail later in this section.

## Using map descriptions
When we have a map description, it is easy to let the layout generator create our layouts.

### Using C# api
The hardest thing is to create an instance of the layout generator. After including the *MapGeneration* (TODO: change name) dll, we get access to the `ChainBasedGenerator` class. The problem is that this class has quite a lot of generic types and even after getting the parameters right, it is not ready to be used and has to be configured.

To make things easier, there is a `LayoutGeneratorFactory` class with 2 static methods. Using this class, you can easily get a ready-to-be-used instance of the layout generator.

```csharp
var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator();
// var layoutGenerator = LayoutGeneratorFactory.GetChainBasedGeneratorWithCorridors(/* parameters */);
```

And layouts are then generated like this:

```csharp
var mapDescription = /* create map description */;
var layouts = layoutGenerator.Generate(mapDescription, 10); // Get 10 layouts
```

**Note:** You can look at the source code of `LayoutGeneratorFactory` to see what needs to be done to create an instance of `ChainBasedGenerator`.

### Using config files
After starting the GUI project, you can easily choose the file that contains your map description and start the generator by clicking the *Generate* button.