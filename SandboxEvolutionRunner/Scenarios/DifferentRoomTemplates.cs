using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.DataStructures.Graphs;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.LayoutEvolvers.SimulatedAnnealing;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Utils;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public class DifferentRoomTemplates : Scenario
    {
        private DungeonGeneratorConfiguration<int> GetConfiguration(NamedMapDescription namedMapDescription, RepeatMode repeatMode = RepeatMode.AllowRepeat)
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

        private void Run(RoomTemplatesSet roomTemplatesSet, RepeatMode repeatMode = RepeatMode.AllowRepeat, bool enhanceRoomTemplates = false)
        {
            var loader = new BetterMapDescriptionLoader(Options, roomTemplatesSet, repeatMode);
            var mapDescriptions = loader.GetMapDescriptions();

            RunBenchmark(mapDescriptions, x => GetConfiguration(x, repeatMode), Options.FinalEvaluationIterations, $"{roomTemplatesSet}_{repeatMode}_{(enhanceRoomTemplates ? "Enhance" : "NoEnhance")}");
        }
        
        
    }
}