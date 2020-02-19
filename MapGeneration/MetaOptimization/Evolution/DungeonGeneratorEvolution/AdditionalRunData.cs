using System.Collections.Generic;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Interfaces.Core.MapLayouts;
using Newtonsoft.Json;

namespace MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution
{
    public class AdditionalRunData
    {
        public List<SimulatedAnnealingEventArgs> SimulatedAnnealingEventArgs { get; set; }

        [JsonIgnore]
        public IMapLayout<int> GeneratedLayout { get; set; }

        public string GeneratedLayoutSvg { get; set; }
    }
}