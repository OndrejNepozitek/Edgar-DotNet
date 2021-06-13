using System.Linq;
using System.Reflection;
using CommandLine;
using Edgar.Extras.Samples;

namespace Edgar.ConsoleApp
{
    internal static class Program
    {
        [Verb("compareToReference")]
        private class CompareToReferenceOptions
        {

        }

        [Verb("runSample")]
        private class RunSampleOptions
        {
            [Value(0, HelpText = "Sample class")]
            public string SampleClass { get; set; }

            [Value(1, HelpText = "Sample method")]
            public string SampleMethod { get; set; }
        }

        internal static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<CompareToReferenceOptions, RunSampleOptions>(args)
                .MapResult(
                    (CompareToReferenceOptions opts) => RunCompareToReference(opts),
                    (RunSampleOptions opts) => RunSample(opts),
                    errs => 1);
        }

        private static int RunCompareToReference(CompareToReferenceOptions opts)
        {
            return 0;
        }

        /// <summary>
        /// Example usage args:
        /// runSample RunBenchmarkSample MinimumExample
        /// </summary>
        /// <param name="opts"></param>
        /// <returns></returns>
        private static int RunSample(RunSampleOptions opts)
        {
            var assembly = typeof(RunBenchmarkSample).Assembly;
            var type = assembly.GetTypes().Single(x => x.Name == opts.SampleClass);
            var method = type.GetMethod(opts.SampleMethod, BindingFlags.Static | BindingFlags.Public);
            method.Invoke(null, new object[] { });

            return 0;
        }
    }
}
