---
id: benchmarks
title: Benchmarks
sidebar_label: Benchmarks
---

*You can find the most recent implementation with a detailed api documentation on [github](https://github.com/OndrejNepozitek/MapGeneration/tree/master/MapGeneration/Benchmarks).*

When working with probabilistic algorithms, it is often hard to say if recent changes made it converge faster or not. One should run the algorithm at least tens of times to get a meaningful result and this process can be really time consuming if not automated.

## `Benchmark` class
`Benchmark` class was implemented to make the process of benchmarking the algorithm as easy as possible. It provides various overloads of the `Execute()` method that run the benchmarks.

### Benchmarking a single map description
The first version is the simplest one. It lets you specify a generator and a single map description and run the algorithm for a given number of times. The method returns `BenchmarkResult` class and you must process the information yourself.

The following code demonstrates how to start a benchmark that will run 20 times and in each run call the generator to generate 5 layouts:

(generic and constructor parameters were omitted for simplicity)
```C#
var benchmark = new Benchmark();
var layoutGenerator = new LayoutGenerator();
var mapDescription = new MapDescription();

// Run the benchmark 20 times, generate 5 layouts in each run
var result = benchmark.Execute(layoutGenerator, "Basic benchmark", mapDescription, repeats: 20, numberOfLayouts: 5);
```

### Benchmarking multiple map descriptions
When optimizing a layout generator, one should try to run the algorithm on multiple different map descriptions. It often happens that optimizing for only one map description results in making others run significantly worse. And here comes into play the second version of the `Execute()` method. It lets you run the benchmark on multiple map descriptions and also outputs the result to the console (and a given TextWriter if specified).

The following code demonstrates how to start a benchmark that will make 20 runs on both given map descriptions:

(generic and constructor parameters were omitted for simplicity)
```C#
var benchmark = new Benchmark();
var layoutGenerator = new LayoutGenerator();

var mapDescription1 = new MapDescription();
var mapDescription2 = new MapDescription();

var mapDescriptions = new List<Tuple<string, MapDescription>>() {
    Tuple.Create("First map description", mapDescription1),
    Tuple.Create("Second map description", mapDescription2)
};

var result = benchmark.Execute(layoutGenerator, "Benchmark of 2 map descriptions", mapDescriptions, repeats: 20, numberOfLayouts: 5);
```

## `BenchmarkScenario` class
TODO

In an ideal world, parameters of a generator would be indepenent. It would suffice to fix all but one parameter, optimize the free one and then move to another one. In the real world, parameters influence one another. One could use an algorithm like CMA-ES to optimize the vector of all available parameters but it would probably be very slow as the layout generation takes some time.

## Tips and tricks
All overloads of the `Execute()` accept an optional `debugWriter` parameter that, if present, makes the benchmark output (sometimes) useful debug information to a given `TextWriter`.