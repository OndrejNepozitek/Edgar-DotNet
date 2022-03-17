using System.Collections.Generic;
using Edgar.Legacy.Core.LayoutEvolvers.Interfaces;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.Utils.Interfaces;
using Newtonsoft.Json;

namespace Edgar.Legacy.Utils.MetaOptimization.Evolution.DungeonGeneratorEvolution
{
    public class AdditionalRunData<TNode> : ISimulatedAnnealingData, IWebGuiRunData
    {
        [JsonIgnore] public MapLayout<TNode> GeneratedLayout { get; set; }

        [JsonIgnore] public List<SimulatedAnnealingEventArgs> SimulatedAnnealingEventArgs { get; set; }

        public string GeneratedLayoutSvg { get; set; }
    }
}