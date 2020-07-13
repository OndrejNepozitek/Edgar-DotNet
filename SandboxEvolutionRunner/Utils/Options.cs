using System.Collections.Generic;
using CommandLine;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.MetaOptimization.Evolution;

namespace SandboxEvolutionRunner.Utils
{
    public class Options
    {
        [Option("graphs", Required = false)]
        public IEnumerable<string> Graphs { get; set; } = null;

        [Option("graphSets", Required = false)]
        public IEnumerable<string> GraphSets { get; set; } = null;

        [Option("graphSetCount", Required = false)]
        public int GraphSetCount { get; set; } = 100;

        [Option("mapDescriptions", Required = false)]
        public IEnumerable<string> MapDescriptions { get; set; } = null;

        [Option("corridorOffsets", Required = false)]
        public IEnumerable<string> CorridorOffsets { get; set; }

        [Option("mutations", Required = false)]
        public IEnumerable<string> Mutations { get; set; } = null;

        [Option("canTouch")] 
        public bool CanTouch { get; set; } = false;

        [Option("eval")] 
        public bool Eval { get; set; } = false;

        [Option("evolutionIterations")] 
        public int EvolutionIterations { get; set; } = 250;

        [Option("finalEvaluationIterations")] 
        public int FinalEvaluationIterations { get; set; } = 250;

        [Option("maxThreads")] 
        public int MaxThreads { get; set; } = 10;

        [Option("name")] 
        public string Name { get; set; }

        [Option("scenario")] 
        public string Scenario { get; set; } = "Evolution";

        [Option("withConsolePreview")] 
        public bool WithConsolePreview { get; set; } = false;

        [Option("earlyStopIterations")] 
        public int? EarlyStopIterations { get; set; } = null;

        [Option("earlyStopTime")] 
        public int? EarlyStopTime { get; set; } = null;

        [Option("fitnessType")] 
        public FitnessType FitnessType { get; set; } = FitnessType.Iterations;

        [Option("asyncBenchmark")]
        public bool AsyncBenchmark { get; set; } = false;

        public IntVector2 Scale { get; set; } = new IntVector2(1, 1);

        [Option("roomTemplates")]
        public RoomTemplatesSet RoomTemplatesSet { get; set; } = RoomTemplatesSet.Original;
    }
}