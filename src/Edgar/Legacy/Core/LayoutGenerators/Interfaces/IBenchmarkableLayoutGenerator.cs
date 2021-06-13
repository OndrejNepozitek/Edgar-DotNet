using Edgar.Benchmarks.Interfaces;

namespace Edgar.Legacy.Core.LayoutGenerators.Interfaces
{
    public interface IBenchmarkableLayoutGenerator<out TLayout> : ILayoutGenerator<TLayout>
    {
        TLayout GenerateLayout(out IGeneratorRun runData);
    }
}