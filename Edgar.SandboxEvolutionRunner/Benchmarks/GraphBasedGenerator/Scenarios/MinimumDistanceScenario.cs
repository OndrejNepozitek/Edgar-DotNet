using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;
using SandboxEvolutionRunner.Utils;

namespace Edgar.SandboxEvolutionRunner.Benchmarks.GraphBasedGenerator.Scenarios
{
    public class MinimumDistanceScenario
    {
        public BenchmarkScenarioGroup<int> GetScenario(List<NamedGraph<int>> graphs, Options options)
        {
            var scenarios = new List<BenchmarkScenario<int>>();

            for (int i = options.MinimumDistance; i <= options.MaximumDistance; i++)
            {
                scenarios.Add(GetScenario(graphs, i));
            }

            return new BenchmarkScenarioGroup<int>("Various minimum distances", scenarios);
        }

        private BenchmarkScenario<int> GetScenario(List<NamedGraph<int>> graphs, int minimumDistance)
        {
            var levelDescriptionLoader = new LevelDescriptionLoader(RoomTemplatesSet.Smart, new IntVector2(1, 1));
            var levelDescriptions = levelDescriptionLoader.GetLevelDescriptions(graphs, new List<int>() { 2, 4, 8 });

            foreach (var graphBasedLevelDescription in levelDescriptions)
            {
                graphBasedLevelDescription.MinimumRoomDistance = minimumDistance;
            }

            return new BenchmarkScenario<int>($"Minimum distance {minimumDistance}", levelDescriptions);
        }

        public class Options
        {
            public int MinimumDistance { get; set; } = 0;

            public int MaximumDistance { get; set; } = 5;
        }
    }
}