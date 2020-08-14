using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Grid2D;

namespace Edgar.Examples
{
    public interface IExampleGrid2D : IExample
    {
        IEnumerable<LevelDescriptionGrid2D<int>> GetResults();
    }
}