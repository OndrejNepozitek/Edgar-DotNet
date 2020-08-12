using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public abstract class OldAndNewScenarioBase : Scenario
    {
        protected abstract DungeonGeneratorConfiguration<int> GetNewConfiguration(NamedMapDescription namedMapDescription);

        protected abstract DungeonGeneratorConfiguration<int> GetOldConfiguration(NamedMapDescription namedMapDescription);

        protected override void Run()
        {
            var mapDescriptions = GetMapDescriptions();

            RunBenchmark(mapDescriptions, GetNewConfiguration, Options.FinalEvaluationIterations, "NewConfiguration");
            RunBenchmark(mapDescriptions, GetOldConfiguration, Options.FinalEvaluationIterations, "OldConfiguration");
        }
    }
}