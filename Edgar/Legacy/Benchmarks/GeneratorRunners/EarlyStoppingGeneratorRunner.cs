using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Benchmarks.Interfaces;

namespace Edgar.Legacy.Benchmarks.GeneratorRunners
{
    public delegate IGeneratorRun GeneratorRunCreator(bool isSuccessful, double time, int iterations);

    public class EarlyStoppingGeneratorRunner : IGeneratorRunner
    {
        private readonly IGeneratorRunner runner;
        private readonly double averageIterationsBaseline;
        private readonly GeneratorRunCreator generatorRunCreator;
        private readonly List<IGeneratorRun> runs = new List<IGeneratorRun>();
        private bool skipRuns;

        private const int MinimumRunsToStop = 10;
        private const double ThresholdStart = 5;
        private const double ThresholdEnd = 2;
        private const int MaxRuns = 50;

        public EarlyStoppingGeneratorRunner(IGeneratorRunner runner, double averageIterationsBaseline, GeneratorRunCreator generatorRunCreator)
        {
            this.runner = runner;
            this.averageIterationsBaseline = averageIterationsBaseline;
            this.generatorRunCreator = generatorRunCreator;
        }

        public IGeneratorRun Run()
        {
            if (skipRuns)
            {
                return generatorRunCreator(false, runs.Average(x => x.Time), (int) runs.Average(x => x.Iterations));
            }

            var run = runner.Run();

            runs.Add(run);

            CheckIfShouldStop();

            return run;
        }

        private void CheckIfShouldStop()
        {
            if (runs.Count >= MinimumRunsToStop)
            {
                var currentThreshold = ThresholdStart + (ThresholdEnd - ThresholdStart) *
                                       ((runs.Count - MinimumRunsToStop) /
                                        (double)(MaxRuns - MinimumRunsToStop));
                currentThreshold = Math.Max(ThresholdEnd, currentThreshold);

                if (runs.Average(x => x.Iterations) > currentThreshold * averageIterationsBaseline)
                {
                    skipRuns = true;
                }
            }
        }
    }
}