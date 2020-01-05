using System.Collections.Generic;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Interfaces.Core.MapLayouts;

namespace MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution
{
    public class AdditionalRunData
    {
        public List<SimulatedAnnealingEventArgs> SimulatedAnnealingEventArgs { get; set; }

        public IMapLayout<int> GeneratedLayout { get; set; }

        public string GeneratedLayoutSvg { get; set; }
    }
}