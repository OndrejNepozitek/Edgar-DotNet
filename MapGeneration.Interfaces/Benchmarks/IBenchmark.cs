using System.Collections.Generic;

namespace MapGeneration.Interfaces.Benchmarks
{
    public interface IBenchmark<TInput>
    {
        IList<IBenchmarkScenario<TInput>> GetScenarios();
    }
}