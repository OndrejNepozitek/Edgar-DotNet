using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Utils;
using SandboxEvolutionRunner.Evolution;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public class BasicRoomsInsteadOfCorridorsScenario : Scenario
    {
        private DungeonGeneratorInput<int> GetInput(NamedMapDescription namedMapDescription)
        {
            return new DungeonGeneratorInput<int>(namedMapDescription.Name, namedMapDescription.MapDescription, new DungeonGeneratorConfiguration<int>()
            {
                RoomsCanTouch = Options.CanTouch,
                EarlyStopIfIterationsExceeded = 20000,
            });
        }

        protected override void Run()
        {
            if (GetCorridorOffsets().Any(x => x[0] == 0))
            {
                throw new ArgumentException("Corridors must be enabled");
            }

            if (Options.MapDescriptions.Any())
            {
                throw new ArgumentException("There must not be custom map descriptions");
            }

            var mapDescriptionsNormal = GetMapDescriptions();

            var customLoader = new BasicRoomsInsteadOfCorridorsLoader(Options);
            var mapDescriptionsDifferent = customLoader.GetMapDescriptions();

            RunBenchmark(mapDescriptionsNormal.Select(GetInput), Options.FinalEvaluationIterations, "NormalMapDescriptions");
            RunBenchmark(mapDescriptionsDifferent.Select(GetInput), Options.FinalEvaluationIterations, "DifferentMapDescriptions");
        }

        public class BasicRoomsInsteadOfCorridorsLoader : MapDescriptionLoader
        {
            public BasicRoomsInsteadOfCorridorsLoader(Options options) : base(options)
            {
            }

            protected override IRoomDescription GetCorridorRoomDescription(List<int> corridorOffsets, int width = 1)
            {
                var corridorRoomTemplates = MapDescriptionUtils.GetCorridorRoomTemplates(corridorOffsets);

                return new BasicRoomDescription(corridorRoomTemplates);
            }
        }
    }
}