using System.Collections.Generic;
using MapGeneration.Core.LayoutEvolvers.Interfaces;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.MapLayouts;
using MapGeneration.Utils.Interfaces;
using Newtonsoft.Json;

namespace MapGeneration.MetaOptimization.Evolution.DungeonGeneratorEvolution
{
    public class AdditionalRunData<TNode> : ISimulatedAnnealingData, IWebGuiRunData
    {
        [JsonIgnore]
        public MapLayout<TNode> GeneratedLayout { get; set; }

        [JsonIgnore]
        public List<SimulatedAnnealingEventArgs> SimulatedAnnealingEventArgs { get; set; }

        public string GeneratedLayoutSvg { get; set; }
    }
}