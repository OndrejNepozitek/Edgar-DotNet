using System;
using System.Collections.Generic;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Benchmarks;
using MapGeneration.Interfaces.Core.LayoutGenerator;
using MapGeneration.Interfaces.Core.MapLayouts;

namespace MapGeneration.Benchmarks
{
    public class Benchmark<TMapDescription, TLayout> : IBenchmark<GeneratorInput<TMapDescription>, TMapDescription, TLayout>
    {
        public IList<GeneratorInput<TMapDescription>> GetInputs()
        {
            throw new NotImplementedException();
        }

        public IList<IBenchmarkScenario<GeneratorInput<TMapDescription>, TMapDescription, TLayout>> GetScenarios()
        {
            throw new NotImplementedException();
        }
    }

    public static class Benchmark
    {
        public static Benchmark<MapDescription<TNode>, IMapLayout<TNode>> CreateForNodeType<TNode>()
        {
            return new Benchmark<MapDescription<TNode>, IMapLayout<TNode>>();
        }
    }
}