using System;
using MapGeneration.Benchmarks.Interfaces;

namespace MapGeneration.Benchmarks.GeneratorRunners
{
    public class LambdaGeneratorRunner : IGeneratorRunner
    {
        private readonly Func<IGeneratorRun> runLambda;

        public LambdaGeneratorRunner(Func<IGeneratorRun> runLambda)
        {
            this.runLambda = runLambda;
        }

        public IGeneratorRun Run()
        {
            return runLambda();
        }
    }
}