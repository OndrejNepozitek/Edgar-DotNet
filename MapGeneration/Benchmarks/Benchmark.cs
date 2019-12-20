using System;
using System.Collections.Generic;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Benchmarks;
using MapGeneration.Interfaces.Core.MapLayouts;

namespace MapGeneration.Benchmarks
{
    public class Benchmark<TMapDescription, TLayout> : IBenchmark<GeneratorInput<TMapDescription>>
    {
        public IList<GeneratorInput<TMapDescription>> GetInputs()
        {
            throw new NotImplementedException();
        }

        public IList<IBenchmarkScenario<GeneratorInput<TMapDescription>>> GetScenarios()
        {
            throw new NotImplementedException();
        }
    }

    public static class Benchmark
    {
        public static Benchmark<MapDescriptionOld<TNode>, IMapLayout<TNode>> CreateForNodeType<TNode>()
        {
            return new Benchmark<MapDescriptionOld<TNode>, IMapLayout<TNode>>();
        }
    }
}