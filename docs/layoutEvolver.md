---
id: layoutEvolver
title: Layout evolvers
sidebar_label: Layout evolvers
---

The goal of layout evolvers is to, given an initial layout, evolve valid layouts. Layout evolvers are aware that the generation process is incremental and that only nodes in one chain are evolved at a time. Evolvers should terminate as soon as possible if they find themselves in a situation where it is unlikely to quickly produce valid layouts.

## Simulated annealing evolver
As its name suggests, this evolver is based on the _simulated annealing_ algorithm. 

### Implementation
Current implementation looks similar to the following pseudocode:

```
T = initial temperature
s = initial layout

for (specified number of cycles) {
  T = decreased temperature

  for (specified number of trials) {
    s_new = perturb a node (from the current chain) in s

    if (s_new is valid) {
        output s_new
    }

    if (s_new is good enough) {
        s = s_new
    }
  }
}
```

### Random restarts
Evolvers are sometimes given layouts that cannot be evolved when perturbing only the nodes in the current chain. In such situations, an evolver should terminate as soon as possible. Therefore, s system of random restarts is implemented. The chance of terminating the algorithm increases as the algorithm fails to produce good enough partial layouts.

### Parameters
TODO

## Implementing a custom evolver
All evolvers must implement `ILayoutEvolver<TLayout, TNode>` interface. The source code with a detailed documentation can be found [on github](https://github.com/OndrejNepozitek/MapGeneration/blob/master/MapGeneration.Interfaces/Core/ILayoutEvolver.cs). 

_Implementing your own evolver can be really easy as you can use existing `LayoutOperations` to handle all the layout operations and just focus on the evolution itself._

### Lazy evaluation
The `Evolve` method of `ILayoutEvolver<TLayout, TNode>` returns an `IEnumerable<TLayout>`. This makes it possible to easily implement layout evolvers with lazy evaluation (using `yield return` syntax). Lazy evaluation can have a _huge_ impact on the overall convergence speed because [generator planners](generatorPlanners.md) are able to make optimizations without ever generating more partial layouts than they really need.

### Hooking into `ChainBasedGenerator`
`ChainBasedGenerator` provides a method with the following signature to inject your own `ILayoutEvolver<TLayout, TNode>` implementation. The creator is then called everytime the generation is started. 

```C#
void SetLayoutEvolverCreator(Func<TMapDescription, IChainBasedLayoutOperations, ILayoutEvolver> creator);
```

__Note__: If your evolver does some heavy lifting on instantiation and does not depend on the actual `MapDescription` and/or `LayoutOperations`, you may create an instance beforehand and then return always the same one from the creator function.

**Example usage**: (generic parameters were omitted for simplicity)
```C#
var chainBasedGenerator = new ChainBasedGenerator();

chainBasedGenerator.SetLayoutEvolverCreator((mapDescription, layoutOperations) => {
    return new SimulatedAnnealingEvolver(layoutOperations);
});
```

