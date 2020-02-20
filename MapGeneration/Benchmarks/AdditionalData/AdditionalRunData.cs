using System.Collections.Generic;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.MapLayouts;
using Newtonsoft.Json;

namespace MapGeneration.Benchmarks.AdditionalData
{
    // TODO: handle better later
    public class AdditionalRunData
    {
        public List<SimulatedAnnealingEventArgs> SimulatedAnnealingEventArgs { get; set; }

        [JsonIgnore]
        public MapLayout<int> GeneratedLayout { get; set; }

        public string GeneratedLayoutSvg { get; set; }
    }
}