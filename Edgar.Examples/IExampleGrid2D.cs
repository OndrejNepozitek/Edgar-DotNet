using Edgar.GraphBasedGenerator.Grid2D;

namespace Edgar.Examples
{
    public interface IExampleGrid2D : IExample
    {
        LevelDescriptionGrid2D<int> GetLevelDescription();
    }
}