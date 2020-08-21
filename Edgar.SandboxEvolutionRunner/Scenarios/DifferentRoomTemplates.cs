using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Common;
using Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapDescriptions;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public class DifferentRoomTemplates : Scenario
    {
        private DungeonGeneratorConfiguration<int> GetConfiguration(NamedMapDescription namedMapDescription, RoomTemplateRepeatMode repeatMode = RoomTemplateRepeatMode.AllowRepeat)
        {
            var configuration = GetBasicConfiguration(namedMapDescription);
            configuration.SimulatedAnnealingConfiguration = new SimulatedAnnealingConfigurationProvider(new SimulatedAnnealingConfiguration()
            {
                MaxIterationsWithoutSuccess = 100,
                HandleTreesGreedily = true,
            });
            configuration.RepeatModeOverride = repeatMode;

            return configuration;
        }

        protected override void Run()
        {
            Run(RoomTemplatesSet.Smart);
            //Run(Mode.Smart, RepeatMode.NoImmediate);
            //Run(Mode.Smart, RepeatMode.NoImmediate, true);
            //Run(Mode.SmallAndMedium);
            //Run(Mode.SmallAndMedium, RepeatMode.NoImmediate);
            //Run(Mode.SmallAndMedium, RepeatMode.NoImmediate, true);
            //Run(Mode.Medium);
            Run(RoomTemplatesSet.Original);
        }

        private void Run(RoomTemplatesSet roomTemplatesSet, RoomTemplateRepeatMode repeatMode = RoomTemplateRepeatMode.AllowRepeat, bool enhanceRoomTemplates = false)
        {
            var loader = new BetterMapDescriptionLoader(Options, roomTemplatesSet, repeatMode);
            var mapDescriptions = loader.GetMapDescriptions();

            RunBenchmark(mapDescriptions, x => GetConfiguration(x, repeatMode), Options.FinalEvaluationIterations, $"{roomTemplatesSet}_{repeatMode}_{(enhanceRoomTemplates ? "Enhance" : "NoEnhance")}");
        }
        
        
    }
}