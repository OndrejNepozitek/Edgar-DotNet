using CommandLine;

namespace PerformanceTests
{
    class Program
    {
        public class Options
        {
            [Option("commit", Required = true)]
            public string Commit { get; set; }

            [Option("commitMessage", Required = true)]
            public string CommitMessage { get; set; }

            [Option("branch", Required = true)]
            public string Branch { get; set; }

            [Option("buildNumber", Required = true)]
            public string BuildNumber { get; set; }

            [Option("pullRequest", Required = false)]
            public string PullRequest { get; set; }

            [Option("url", Required = true)]
            public string Url { get; set; }

            [Option("repeats", Required = false, Default = 100)]
            public int Repeats { get; set; }
        }

        internal static void Main(string[] args)
        {
            //Parser.Default.ParseArguments<Options>(args)
            //    .WithParsed(opts =>
            //    {
            //        RunBenchmark(opts).Wait();
            //    });
        }

        //public static async Task RunBenchmark(Options options)
        //{
        //    var scale = new IntVector2(1, 1);
        //    var offsets = new List<int>() { 2 };

        //    var mapDescriptions = GetMapDescriptionsSet(scale, false, offsets);
        //    mapDescriptions.AddRange(GetMapDescriptionsSet(scale, true, offsets));

        //    var benchmarkRunner = BenchmarkRunner.CreateForNodeType<int>();

        //    var scenario = BenchmarkScenario.CreateForNodeType<int>(
        //        "From commit",
        //        input =>
        //        {
        //            if (input.MapDescription.IsWithCorridors)
        //            {
        //                var layoutGenerator = LayoutGeneratorFactory.GetSimpleChainBasedGenerator<int>(input.MapDescription, true, offsets);
        //                layoutGenerator.InjectRandomGenerator(new Random(0));

        //                return layoutGenerator;
        //            }
        //            else
        //            {
        //                var layoutGenerator = LayoutGeneratorFactory.GetSimpleChainBasedGenerator<int>(input.MapDescription, false);
        //                layoutGenerator.InjectRandomGenerator(new Random(0));

        //                return layoutGenerator;
        //            }
        //        });

        //    // Dry run
        //    Console.WriteLine("Dry run");
        //    benchmarkRunner.Run(scenario, mapDescriptions, 5, new BenchmarkOptions()
        //    {
        //        WithConsolePreview = false,
        //    });
        //    Console.WriteLine();

        //    // Real run
        //    Console.WriteLine("Real run");
        //    var scenarioResult = benchmarkRunner.Run(scenario, mapDescriptions, options.Repeats, new BenchmarkOptions()
        //    {
        //        WithConsolePreview = false,
        //    });

        //    var resultSaver = new BenchmarkResultSaver();
        //    var uploadConfig = new UploadConfig(options.Url);

        //    await resultSaver.UploadCommitResult(scenarioResult, uploadConfig, new CommitInfo()
        //    {
        //        Commit = options.Commit,
        //        CommitMessage = options.CommitMessage,
        //        Branch = options.Branch,
        //        BuildNumber = options.BuildNumber,
        //        PullRequest = options.PullRequest,
        //    });
        //}

        //public static List<GeneratorInput<MapDescriptionOld<int>>> GetMapDescriptionsSet(IntVector2 scale, bool enableCorridors, List<int> offsets = null)
        //{
        //    var inputs = new List<GeneratorInput<MapDescriptionOld<int>>>()
        //    {
        //        new GeneratorInput<MapDescriptionOld<int>>("Example 1 (fig. 1)", new MapDescriptionOld<int>()
        //            .SetupWithGraph(GraphsDatabase.GetExample1())
        //            .AddClassicRoomShapes(scale)
        //            .AddCorridorRoomShapes(offsets, enableCorridors)),
        //        new GeneratorInput<MapDescriptionOld<int>>("Example 2 (fig. 7 top)",
        //            new MapDescriptionOld<int>()
        //                .SetupWithGraph(GraphsDatabase.GetExample2())
        //                .AddClassicRoomShapes(scale)
        //                .AddCorridorRoomShapes(offsets, enableCorridors)),
        //        new GeneratorInput<MapDescriptionOld<int>>("Example 3 (fig. 7 bottom)",
        //            new MapDescriptionOld<int>()
        //                .SetupWithGraph(GraphsDatabase.GetExample3())
        //                .AddClassicRoomShapes(scale)
        //                .AddCorridorRoomShapes(offsets, enableCorridors)),
        //        new GeneratorInput<MapDescriptionOld<int>>("Example 4 (fig. 8)",
        //            new MapDescriptionOld<int>()
        //                .SetupWithGraph(GraphsDatabase.GetExample4())
        //                .AddClassicRoomShapes(scale)
        //                .AddCorridorRoomShapes(offsets, enableCorridors)),
        //        new GeneratorInput<MapDescriptionOld<int>>("Example 5 (fig. 9)",
        //            new MapDescriptionOld<int>()
        //                .SetupWithGraph(GraphsDatabase.GetExample5())
        //                .AddClassicRoomShapes(scale)
        //                .AddCorridorRoomShapes(offsets, enableCorridors)),
        //    };

        //    if (enableCorridors)
        //    {
        //        inputs.ForEach(x => x.Name += " wc");
        //    }

        //    return inputs;
        //}
    }
}
