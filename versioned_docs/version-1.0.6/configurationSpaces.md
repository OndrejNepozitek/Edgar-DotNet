---
id: configurationSpaces
title: Configuration spaces
sidebar_label: Configuration spaces
---

*Following paragraphs describe configuration spaces in the context of this library. See the Motivation section for a more general description.*

Configuration spaces let us quickly check whether two configurations are valid with respect to each other. In general, they have 3 main purposes:
- To check whether two configurations are valid with respect to each other
- To suggest a new shape when perturbing a node
- To suggest a new position when perturbing a node

The usual implementation checks if two neighbouring nodes are connected by doors, gives us a random shape from a predefined set of shapes and gets, for a given node, a position that is the best choice for optimizing door connectivity with neighbouring nodes.

The speed of configuration spaces is crucial to the overall convergence speed. That means that majority of information **should be precomputed** and the absolute minimum should be computed on the fly.

## Motivation
In motion planning for robotics, a *configuration space* of a robot is a set of all configurations it can attain (while avoiding obstacles). 

In our context, for each pair of polygons (one fixed and one able to move) a configuration space is a set of such positions of the moving polygon that the two polygons do not overlap and can be connected by doors. When working with polygons, each configuration space can be represented by a (possibly empty) set of lines.

With configuration spaces of all the pairs of room shapes, we can easily check if two configurations satisfy constraints defined above. We just take the shape of the first configuration as the fixed one and then look if the position of the other configuration is contained in the configuration space of the pair.

When perturbing a position of a node, we can get configuration spaces for configurations of all its neighbours and then find a point that intersects most of them. By doing so, we always pick kind of good position which really helps the generator to converge faster.

## Computing configuration spaces
TODO

## Implementation
*This section is about the logic of working with already precomputed configuration spaces. For a description of how to compute configuration spaces, see the section above.*

The implementation is pretty straightforward. `AbstractConfigurationSpaces` class takes care of computing intersections of configuration spaces - either to check if two configurations are valid or to get a maximum intersection of a set of configuration spaces.

`ConfigurationSpaces` class inherits from the abstract class and implements all the getters needed for `AbstractConfigurationSpaces` to work. 

`ConfigurationSpaces` also supports adding weights to shapes. That means that some shapes have a bigger chance to be selected when asking for a random shape. This can be useful if we want a specific distribution of shapes (e.g. more squares than rectangles).

## Implementing custom configuration spaces
All configurations spaces implementations must implement `IConfigurationSpaces` interface. The source code with a detailed documentation can be found [on github](https://github.com/OndrejNepozitek/MapGeneration/blob/master/MapGeneration.Interfaces/Core/ConfigurationSpaces/IConfigurationSpaces.cs). 

The easiest way to add a custom behaviour is to implement your own generator that will construct an instance of `ConfigurationSpaces` class. If thas is not enough, you can make your own class that inherits from `AbstractConfigurationSpaces` or even implement the `IConfigurationSpaces` interface all by yourself.

### Hooking into `ChainBasedGenerator`
`ChainBasedGenerator` provides a method with the following signature to inject your own `IConfigurationSpaces` implementation. The creator is then called everytime the generation is started. 

```C#
void SetConfigurationSpacesCreator(Func<TMapDescription, IConfigurationSpaces> creator);
```

**Example usage**: (generic and constructor parameters were omitted for simplicity)
```C#
var chainBasedGenerator = new ChainBasedGenerator();
var configurationSpacesGenerator = new ConfigurationSpacesGenerator();

chainBasedGenerator.SetConfigurationSpacesCreator((mapDescription) => {
    return configurationSpacesGenerator.Generate(mapDescription)
});
```