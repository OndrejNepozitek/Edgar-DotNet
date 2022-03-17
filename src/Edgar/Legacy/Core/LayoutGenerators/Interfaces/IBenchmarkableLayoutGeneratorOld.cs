﻿namespace Edgar.Legacy.Core.LayoutGenerators.Interfaces
{
    /// <summary>
    /// Represents layout generators that can be benchmarked.
    /// </summary>
    public interface IBenchmarkableLayoutGeneratorOld<out TLayout> : ILayoutGenerator<TLayout>
    {
        /// <summary>
        /// Number of milliseconds until a first layout is generated.
        /// </summary>
        long TimeFirst { get; }

        /// <summary>
        /// Number of milliseconds until all layouts are generated.
        /// </summary>
        long TimeTotal { get; }

        /// <summary>
        /// Number of iterations of the whole run.
        /// </summary>
        int IterationsCount { get; }

        /// <summary>
        /// Number of layouts generated.
        /// </summary>
        int LayoutsCount { get; }

        /// <summary>
        /// Enables collecting information for benchmarking.
        /// </summary>
        /// <param name="enable"></param>
        void EnableBenchmark(bool enable);
    }
}