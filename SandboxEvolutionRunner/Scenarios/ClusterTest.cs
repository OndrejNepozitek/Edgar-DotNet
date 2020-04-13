using System;
using System.Threading;
using SandboxEvolutionRunner.Utils;

namespace SandboxEvolutionRunner.Scenarios
{
    public class ClusterTest : Scenario
    {
        protected override void Run()
        {
            Console.WriteLine($"ProcessorCount: {Environment.ProcessorCount}");
            ThreadPool.GetMaxThreads(out var workerThreads, out var completionPortThreads);
            Console.WriteLine($"MaxWorkerThreads: {workerThreads}");
            Console.WriteLine($"MaxCompletionPortThreads: {completionPortThreads}");
            ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
            Console.WriteLine($"MinWorkerThreads: {minWorkerThreads}");
            Console.WriteLine($"MinCompletionPortThreads: {minCompletionPortThreads}");
        }
    }
}