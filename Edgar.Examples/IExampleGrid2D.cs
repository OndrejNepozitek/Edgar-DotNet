using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Grid2D;

namespace Edgar.Examples
{
    public interface IExampleGrid2D<TRoom> : IExample
    {
        IEnumerable<LevelDescriptionGrid2D<TRoom>> GetResults();

        void Run();
    }
}