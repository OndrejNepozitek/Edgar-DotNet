using System.Collections.Generic;

namespace MapGeneration.Interfaces.Benchmarks
{
    public interface IBenchmark<TInput, TMapDescription, TLayout>
    {
        IList<IBenchmarkScenario<TInput, TMapDescription, TLayout>> GetScenarios();
    }
}