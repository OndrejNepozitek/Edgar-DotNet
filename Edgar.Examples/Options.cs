using CommandLine;

namespace Edgar.Examples
{
    public class Options
    {
        [Option("sourceFolder", Required = true)] 
        public string SourceFolder { get; set; }

        [Option("outputFolder", Required = true)] 
        public string OutputFolder { get; set; }
    }
}