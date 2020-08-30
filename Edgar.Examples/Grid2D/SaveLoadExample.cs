using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Grid2D;

namespace Edgar.Examples.Grid2D
{
    public class SaveLoadExample : IExampleGrid2D<int>
    {
        public string Name => "Save and load";

        public string DocsFileName => "save-load";

        public string EntryPointMethod => nameof(Run);

        public void Run()
        {
            
        }

        public IEnumerable<LevelDescriptionGrid2D<int>> GetResults()
        {
            yield break;
        }
    }
}