using System.Collections.Generic;

namespace MapGeneration.Benchmarks.Interfaces
{
    public interface IBenchmark<TInput>
    {
        IList<IBenchmarkScenario<TInput>> GetScenarios();
    }
}