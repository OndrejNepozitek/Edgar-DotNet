---
id: generatorPlanners
title: Generator planners
sidebar_label: Generator planners
---

Complete layouts are created by adding chains one by one to valid partial layouts. One valid partial layout is often used as a base for generating multiple partial layouts with one more chain added. This process creates a tree-like structure with nodes being valid partial layouts and children nodes being partial layouts generated from the parent layout (by adding the next chain).

**Generator planners** make it possible to control how is the tree built.

## Basic generator planner
The `BasicGeneratorPlanner` class is (as its name suggests) a basic implementation of a generator planner. It always picks a node on the deepest level of the tree (the one with the maximum number of chains) and tries to use it to generate layouts.

Some nodes in the tree get marked as _finished_. This is because either a given layout evolver fails to generate more layouts or because the generator planner decides that the node is no longer good enough to be further explored. Such nodes are then not considered when deciding which node to expand.

`BasicGeneratorPlanner` tries to generate 5 layouts from each node before making the node _finished_.

## Possible improvements
Currently, the number of layouts generated from each node is fixed. It would be interesting to implement a generator planner that somehow adapts its parameters as it tries to generate layouts.

And what about making one with a multi-threading support?

## Implementing a custom generator planner
All generator planners must implement `IGeneratorPlanner<TLayout>` interface. The source code with a detailed documentation can be found [on github](https://github.com/OndrejNepozitek/MapGeneration/blob/master/MapGeneration.Interfaces/Core/GeneratorPlanners/IGeneratorPlanner.cs). 

There is also a `GeneratorPlannerBase<TLayout>` class with an abstract method that takes the current tree as an input and returns a node that should be further explored. You can look how it is being used by `BasicGeneratorPlanner<TLayout>` [here](https://github.com/OndrejNepozitek/MapGeneration/blob/master/MapGeneration/Core/GeneratorPlanners/BasicGeneratorPlanner.cs).

### Hooking into `ChainBasedGenerator`
`ChainBasedGenerator` provides a method with the following signature to inject your own `IGeneratorPlanner<TLayout>` implementation. The creator is then called everytime the generation is started. 

```C#
void SetGeneratorPlannerCreator(Func<TMapDescription, IGeneratorPlanner> creator)
```

**Example usage**: (generic parameters were omitted for simplicity)
```C#
chainBasedGenerator.SetGeneratorPlannerCreator((mapDescription) => {
    return new BasicGeneratorPlanner();
});
```