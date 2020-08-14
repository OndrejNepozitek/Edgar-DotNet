using CommandLine;

namespace Edgar.Examples
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            Parser
                .Default
                .ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        private static void Run(Options options)
        {
            var examplesGenerator = new ExamplesGenerator(options.SourceFolder, options.OutputFolder);
            examplesGenerator.Run();
        }
    }
}