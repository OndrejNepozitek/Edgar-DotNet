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
        private RoomDescriptionMode mode;

        protected override IRoomDescription GetCorridorRoomDescription(List<int> corridorOffsets)
        {
            var corridorRoomTemplates = MapDescriptionUtils.GetCorridorRoomTemplates(corridorOffsets);

            if (mode == RoomDescriptionMode.Basic)
            {
                return new BasicRoomDescription(corridorRoomTemplates);
            }

            return new CorridorRoomDescription(corridorRoomTemplates);;
        }

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

            mode = RoomDescriptionMode.Corridor;
            var mapDescriptionsNormal = GetMapDescriptions();
            mode = RoomDescriptionMode.Basic;
            var mapDescriptionsDifferent = GetMapDescriptions();

            RunBenchmark(mapDescriptionsNormal.Select(GetInput), Options.FinalEvaluationIterations, "NormalMapDescriptions");
            RunBenchmark(mapDescriptionsDifferent.Select(GetInput), Options.FinalEvaluationIterations, "DifferentMapDescriptions");
        }

        public enum RoomDescriptionMode
        {
            Corridor, Basic
        }
    }
}