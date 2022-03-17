using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.Configurations.Interfaces;
using Edgar.Legacy.Core.LayoutEvolvers.Interfaces;
using Edgar.Legacy.Core.LayoutOperations.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.Legacy.Core.LayoutEvolvers.SimulatedAnnealing
{
    /// <summary>
    /// Implementation of a simulated annealing evolver.
    /// </summary>
    /// <typeparam name="TLayout"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TConfiguration"></typeparam>
    public class SimulatedAnnealingEvolver<TLayout, TNode, TConfiguration> : ILayoutEvolver<TLayout, TNode>,
        IRandomInjectable, ICancellable
        where TLayout : ILayout<TNode, TConfiguration>, ISmartCloneable<TLayout>
        where TConfiguration : IConfiguration<IntAlias<PolygonGrid2D>, TNode>
    {
        protected Random Random;
        protected CancellationToken? CancellationToken;

        protected IChainBasedLayoutOperations<TLayout, TNode> LayoutOperations;

        public event EventHandler<TLayout> OnPerturbed;
        public event EventHandler<TLayout> OnValid;
        public event EventHandler<SimulatedAnnealingEventArgs> OnEvent;

        protected SimulatedAnnealingConfiguration Configuration;
        protected SimulatedAnnealingConfigurationProvider ConfigurationProvider;

        private readonly bool addNodesGreedilyBeforeEvolve;

        // TODO: should configuration be null by default?
        public SimulatedAnnealingEvolver(IChainBasedLayoutOperations<TLayout, TNode> layoutOperations,
            SimulatedAnnealingConfiguration configuration = null, bool addNodesGreedilyBeforeEvolve = false)
        {
            LayoutOperations = layoutOperations ?? throw new ArgumentNullException(nameof(layoutOperations));
            Configuration = configuration ?? SimulatedAnnealingConfiguration.GetDefaultConfiguration();
            this.addNodesGreedilyBeforeEvolve = addNodesGreedilyBeforeEvolve;
        }

        public SimulatedAnnealingEvolver(IChainBasedLayoutOperations<TLayout, TNode> layoutOperations,
            SimulatedAnnealingConfigurationProvider configurationProvider, bool addNodesGreedilyBeforeEvolve = false)
        {
            LayoutOperations = layoutOperations ?? throw new ArgumentNullException(nameof(layoutOperations));
            ConfigurationProvider =
                configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            this.addNodesGreedilyBeforeEvolve = addNodesGreedilyBeforeEvolve;
        }

        protected SimulatedAnnealingConfiguration GetConfiguration(int chainNumber)
        {
            if (ConfigurationProvider != null)
            {
                return ConfigurationProvider.GetConfiguration(chainNumber);
            }

            return Configuration;
        }

        /// <inheritdoc />
        public IEnumerable<TLayout> Evolve(TLayout initialLayout, Chain<TNode> chain, int count)
        {
            var configuration = GetConfiguration(chain.Number);

            if (!chain.IsFromFace && configuration.HandleTreesGreedily)
            {
                var iters = 0;
                var lastEventIters = 0;

                for (int i = 0; i < 2; i++)
                {
                    if (CancellationToken.HasValue && CancellationToken.Value.IsCancellationRequested)
                        yield break;

                    for (int k = 0; k < 1; k++)
                    {
                        if (CancellationToken.HasValue && CancellationToken.Value.IsCancellationRequested)
                            yield break;

                        var copy = initialLayout.SmartClone();
                        LayoutOperations.AddChain(copy, chain.Nodes, true, out var addChainIterationsCount);

                        iters += addChainIterationsCount;

                        // An event must be sent in order for the early stopping handler to work
                        OnPerturbed?.Invoke(this, copy);

                        if (CancellationToken.HasValue && CancellationToken.Value.IsCancellationRequested)
                            yield break;

                        if (LayoutOperations.IsLayoutValid(copy))
                        {
                            if (LayoutOperations.TryCompleteChain(copy, chain.Nodes))
                            {
                                OnPerturbed?.Invoke(this, copy);

                                if (CancellationToken.HasValue && CancellationToken.Value.IsCancellationRequested)
                                    yield break;

                                OnEvent?.Invoke(this, new SimulatedAnnealingEventArgs()
                                {
                                    Type = SimulatedAnnealingEventType.LayoutGenerated,
                                    IterationsSinceLastEvent = iters - lastEventIters,
                                    IterationsTotal = iters,
                                    LayoutsGenerated = -1,
                                    ChainNumber = chain.Number,
                                });

                                lastEventIters = iters;

                                yield return copy;
                                break;
                            }
                        }

                        OnEvent?.Invoke(this, new SimulatedAnnealingEventArgs()
                        {
                            Type = SimulatedAnnealingEventType.OutOfIterations,
                            IterationsSinceLastEvent = iters - lastEventIters,
                            IterationsTotal = iters,
                            LayoutsGenerated = -1,
                            ChainNumber = chain.Number,
                        });

                        lastEventIters = iters;
                    }
                }

                yield break;
            }

            if (addNodesGreedilyBeforeEvolve)
            {
                LayoutOperations.AddChain(initialLayout, chain.Nodes, true, out var addChainIterationsCount);
            }

            const double p0 = 0.2d;
            const double p1 = 0.01d;
            var t0 = -1d / Math.Log(p0);
            var t1 = -1d / Math.Log(p1);
            var ratio = Math.Pow(t1 / t0, 1d / (configuration.Cycles - 1));
            var deltaEAvg = 0d;
            var acceptedSolutions = 1;

            var t = t0;

            var layouts = new List<TLayout>();
            var originalLayout = initialLayout;
            var currentLayout = originalLayout;

            #region Debug output

            //if (withDebugOutput)
            //{
            //	Console.WriteLine($"Initial energy: {layoutOperations.GetEnergy(currentLayout)}");
            //}

            #endregion

            var numberOfFailures = 0;
            var stageTwoFailures = 0;

            var iterations = 0;
            var lastEventIterations = 0;

            var shouldStop = false;

            for (var i = 0; i < configuration.Cycles; i++)
            {
                var wasAccepted = false;

                #region Random restarts

                if (enableRandomRestarts)
                {
                    if (ShouldRestart(numberOfFailures))
                    {
                        OnEvent?.Invoke(this, new SimulatedAnnealingEventArgs()
                        {
                            Type = SimulatedAnnealingEventType.RandomRestart,
                            IterationsSinceLastEvent = iterations - lastEventIterations,
                            IterationsTotal = iterations,
                            LayoutsGenerated = layouts.Count,
                            ChainNumber = chain.Number,
                        });
                        yield break;
                    }
                }

                #endregion

                if (iterations - lastEventIterations > configuration.MaxIterationsWithoutSuccess)
                {
                    break;
                }

                if (shouldStop)
                {
                    break;
                }

                for (var j = 0; j < configuration.TrialsPerCycle; j++)
                {
                    if (CancellationToken.HasValue && CancellationToken.Value.IsCancellationRequested)
                        yield break;

                    if (stageTwoFailures > configuration.MaxStageTwoFailures)
                    {
                        shouldStop = true;
                        break;
                    }

                    iterations++;
                    var perturbedLayout = PerturbLayout(currentLayout, chain.Nodes, out var energyDelta);

                    OnPerturbed?.Invoke(this, perturbedLayout);

                    // TODO: can we check the energy instead?
                    if (LayoutOperations.IsLayoutValid(perturbedLayout, chain.Nodes))
                    {
                        #region Random restarts

                        if (enableRandomRestarts && randomRestartsSuccessPlace == RestartSuccessPlace.OnValid)
                        {
                            wasAccepted = true;

                            if (randomRestartsResetCounter)
                            {
                                numberOfFailures = 0;
                            }
                        }

                        #endregion

                        // TODO: wouldn't it be too slow to compare againts all?
                        if (IsDifferentEnough(perturbedLayout, layouts))
                        {
                            // TODO 2SG: should we clone before TryCompleteChain or should TryCompleteChain not change the layout?
                            var newLayout = perturbedLayout.SmartClone();
                            var shouldContinue = LayoutOperations.TryCompleteChain(newLayout, chain.Nodes);

                            if (shouldContinue)
                            {
                                layouts.Add(newLayout);
                                OnValid?.Invoke(this, newLayout);

                                #region Random restarts

                                if (enableRandomRestarts &&
                                    randomRestartsSuccessPlace == RestartSuccessPlace.OnValidAndDifferent)
                                {
                                    wasAccepted = true;

                                    if (randomRestartsResetCounter)
                                    {
                                        numberOfFailures = 0;
                                    }
                                }

                                #endregion

                                OnEvent?.Invoke(this, new SimulatedAnnealingEventArgs()
                                {
                                    Type = SimulatedAnnealingEventType.LayoutGenerated,
                                    IterationsSinceLastEvent = iterations - lastEventIterations,
                                    IterationsTotal = iterations,
                                    LayoutsGenerated = layouts.Count,
                                    ChainNumber = chain.Number,
                                });

                                yield return newLayout;

                                lastEventIterations = iterations;
                                stageTwoFailures = 0;

                                #region Debug output

                                //if (withDebugOutput)
                                //{
                                //	Console.WriteLine($"Found layout, cycle {i}, trial {j}, energy {layoutOperations.GetEnergy(perturbedLayout)}");
                                //}

                                #endregion

                                if (layouts.Count >= count)
                                {
                                    #region Debug output

                                    //if (withDebugOutput)
                                    //{
                                    //	Console.WriteLine($"Returning {layouts.Count} partial layouts");
                                    //}

                                    #endregion

                                    yield break;
                                }
                            }
                            else
                            {
                                stageTwoFailures++;

                                OnEvent?.Invoke(this, new SimulatedAnnealingEventArgs()
                                {
                                    Type = SimulatedAnnealingEventType.StageTwoFailure,
                                    IterationsSinceLastEvent = iterations - lastEventIterations,
                                    IterationsTotal = iterations,
                                    LayoutsGenerated = layouts.Count,
                                    ChainNumber = chain.Number,
                                    ResetsIterationsSinceLastEvent = false,
                                });
                            }
                        }
                    }

                    var deltaAbs = Math.Abs(energyDelta);
                    var accept = false;

                    if (energyDelta > 0)
                    {
                        if (i == 0 && j == 0)
                        {
                            deltaEAvg = deltaAbs * 15;
                        }

                        var p = Math.Pow(Math.E, -deltaAbs / (deltaEAvg * t));
                        if (Random.NextDouble() < p)
                            accept = true;
                    }
                    else
                    {
                        accept = true;
                    }

                    if (accept)
                    {
                        acceptedSolutions++;
                        currentLayout = perturbedLayout;
                        deltaEAvg = (deltaEAvg * (acceptedSolutions - 1) + deltaAbs) / acceptedSolutions;

                        #region Random restarts

                        if (enableRandomRestarts && randomRestartsSuccessPlace == RestartSuccessPlace.OnAccepted)
                        {
                            wasAccepted = true;

                            if (randomRestartsResetCounter)
                            {
                                numberOfFailures = 0;
                            }
                        }

                        #endregion
                    }
                }

                if (!wasAccepted)
                {
                    numberOfFailures++;
                }

                t = t * ratio;
            }

            OnEvent?.Invoke(this, new SimulatedAnnealingEventArgs()
            {
                Type = SimulatedAnnealingEventType.OutOfIterations,
                IterationsSinceLastEvent = iterations - lastEventIterations,
                IterationsTotal = iterations,
                LayoutsGenerated = layouts.Count,
                ChainNumber = chain.Number,
            });
        }

        private TLayout PerturbLayout(TLayout layout, IList<TNode> chain, out double energyDelta)
        {
            // TODO: sometimes perturb a node that is not in the current chain?
            var newLayout = layout.SmartClone();

            var energy = LayoutOperations.GetEnergy(newLayout);

            LayoutOperations.PerturbLayout(newLayout, chain, true);

            var newEnergy = LayoutOperations.GetEnergy(newLayout);
            energyDelta = newEnergy - energy;

            return newLayout;
        }

        private bool IsDifferentEnough(TLayout layout, IList<TLayout> layouts)
        {
            return layouts.All(x => LayoutOperations.AreDifferentEnough(layout, x));
        }

        #region Random restarts

        private bool enableRandomRestarts = true;
        private RestartSuccessPlace randomRestartsSuccessPlace = RestartSuccessPlace.OnValidAndDifferent;
        private bool randomRestartsResetCounter = false;
        private float randomRestartsScale = 1;
        private List<int> randomRestartProbabilities = new List<int>() {2, 3, 5, 7};

        /// <summary>
        /// Set up random restarts strategy.
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="successPlace"></param>
        /// <param name="resetCounter"></param>
        /// <param name="scale"></param>
        public void SetRandomRestarts(bool enable,
            RestartSuccessPlace successPlace = RestartSuccessPlace.OnValidAndDifferent, bool resetCounter = false,
            float scale = 1f)
        {
            enableRandomRestarts = enable;
            randomRestartsSuccessPlace = successPlace;
            randomRestartsResetCounter = resetCounter;

            if (scale <= 0)
                throw new ArgumentException();

            randomRestartsScale = scale;
            randomRestartProbabilities =
                (new List<int>() {2, 3, 5, 7}).Select(x => (int) (x * randomRestartsScale)).ToList();
        }

        private bool ShouldRestart(int numberOfFailures)
        {
            // ReSharper disable once ReplaceWithSingleAssignment.False
            var shouldRestart = false;

            if (numberOfFailures > 8 && Random.Next(0, randomRestartProbabilities[0]) == 0)
            {
                shouldRestart = true;
            }
            else if (numberOfFailures > 6 && Random.Next(0, randomRestartProbabilities[1]) == 0)
            {
                shouldRestart = true;
            }
            else if (numberOfFailures > 4 && Random.Next(0, randomRestartProbabilities[2]) == 0)
            {
                shouldRestart = true;
            }
            else if (numberOfFailures > 2 && Random.Next(0, randomRestartProbabilities[3]) == 0)
            {
                shouldRestart = true;
            }

            return shouldRestart;
        }

        public enum RestartSuccessPlace
        {
            OnValid,
            OnValidAndDifferent,
            OnAccepted
        }

        #endregion

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