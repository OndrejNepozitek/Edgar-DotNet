using System.Collections.Generic;

namespace Edgar.Legacy.Benchmarks.Interfaces
{
    public interface IBenchmark<TInput>
    {
        IList<IBenchmarkScenario<TInput>> GetScenarios();
    }
}