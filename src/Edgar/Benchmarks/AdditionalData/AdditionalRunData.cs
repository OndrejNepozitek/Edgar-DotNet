using System.Collections.Generic;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.MapLayouts;
using Newtonsoft.Json;

namespace Edgar.Benchmarks.AdditionalData
{
    public class AdditionalRunData
    {
        public List<SimulatedAnnealingEventArgs> SimulatedAnnealingEventArgs { get; set; }

        [JsonIgnore]
        public MapLayout<int> GeneratedLayout { get; set; }

        public string GeneratedLayoutSvg { get; set; }
    }
}