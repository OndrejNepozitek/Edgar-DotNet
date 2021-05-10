using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.GeneratorPlanners.Interfaces;
using Edgar.Legacy.Core.LayoutConverters.Interfaces;
using Edgar.Legacy.Core.LayoutEvolvers.Interfaces;
using Edgar.Legacy.Core.LayoutGenerators.Interfaces;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.Legacy.Core.LayoutGenerators
{
    /// <summary>
    /// Simple chain based procedural level generator. Serves as a base for the dungeon generator.
    /// </summary>
    /// <typeparam name="TMapDescription"></typeparam>
    /// <typeparam name="TLayout"></typeparam>
    /// <typeparam name="TOutputLayout"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public class ChainBasedGenerator<TLayout, TOutputLayout, TNode> : IBenchmarkableLayoutGenerator<TOutputLayout>, IRandomInjectable, ICancellable
    {
        private readonly TLayout initialLayout;
        private readonly IGeneratorPlanner<TLayout, TNode> generatorPlanner;
        private readonly List<Chain<TNode>> chains;
        private readonly ILayoutEvolver<TLayout, TNode> layoutEvolver;
        private readonly ILayoutConverter<TLayout, TOutputLayout> layoutConverter;

        protected Random Random = new Random();
        protected CancellationToken? CancellationToken;

        public event Action<Random> OnRandomInjected; 
        public event Action<CancellationToken> OnCancellationTokenInjected; 

        public ChainBasedGenerator(TLayout initialLayout, IGeneratorPlanner<TLayout, TNode> generatorPlanner, List<Chain<TNode>> chains, ILayoutEvolver<TLayout, TNode> layoutEvolver, ILayoutConverter<TLayout, TOutputLayout> layoutConverter)
        {
            this.initialLayout = initialLayout;
            this.generatorPlanner = generatorPlanner;
            this.chains = chains;
            this.layoutEvolver = layoutEvolver;
            this.layoutConverter = layoutConverter;
        }

        /// <summary>
        /// Generates a layout.
        /// </summary>
        /// <returns></returns>
        public TOutputLayout GenerateLayout()
        {
            IterationsCount = 0;
            layoutEvolver.OnPerturbed += IterationsCounterHandler;

            OnRandomInjected?.Invoke(Random);

            if (CancellationToken.HasValue)
            {
                OnCancellationTokenInjected?.Invoke(CancellationToken.Value);
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var layout = generatorPlanner.Generate(initialLayout, chains, layoutEvolver);

            stopwatch.Stop();

            layoutEvolver.OnPerturbed -= IterationsCounterHandler;
            LayoutsCount = 1;
            TimeTotal = stopwatch.ElapsedMilliseconds;

            // Reset cancellation token if it was already used
            if (CancellationToken.HasValue && CancellationToken.Value.IsCancellationRequested)
            {
                CancellationToken = null;
            }

            var convertedLayout = layout != null ? layoutConverter.Convert(layout, true) : default(TOutputLayout);

            return convertedLayout;
        }

        private void IterationsCounterHandler(object sender, TLayout layout)
        {
            IterationsCount++;
        }

        public long TimeFirst => throw new InvalidOperationException();

        public long TimeTotal { get; private set; }

        public int IterationsCount { get; private set; }

        public int LayoutsCount { get; private set; }

        public void EnableBenchmark(bool enable)
        {
            
        }

        /// <summary>
        /// Checks if a given object is IRandomInjectable and/or ICancellable
        /// and tries to inject a random generator or a cancellation token.
        /// </summary>
        /// <param name="o"></param>
        protected void TryInjectRandomAndCancellationToken(object o)
        {
            if (o is IRandomInjectable randomInjectable)
            {
                randomInjectable.InjectRandomGenerator(Random);
            }

            if (CancellationToken.HasValue && o is ICancellable cancellable)
            {
                cancellable.SetCancellationToken(CancellationToken.Value);
            }
        }

        public void InjectRandomGenerator(Random random)
        {
            Random = random;
        }

        public void SetCancellationToken(CancellationToken? cancellationToken)
        {
            CancellationToken = cancellationToken;
        }
    }
}