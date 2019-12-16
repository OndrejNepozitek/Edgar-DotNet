using System;
using MapGeneration.Interfaces.Benchmarks;

namespace MapGeneration.Benchmarks
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