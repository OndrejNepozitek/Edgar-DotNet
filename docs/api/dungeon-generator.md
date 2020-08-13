---
title: Dungeon generator
---

The `DungeonGenerator` class encapsulates the whole high-level API of the generator. It takes an instance of `MapDescription` and `DungeonGeneratorConfiguration` and produces a level.

## Configuration

Individual properties of the configuration can be seen below:

    /// <summary>
    /// Configuration of the dungeon generator.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class DungeonGeneratorConfiguration<TNode> : IChainDecompositionConfiguration<TNode>,
        ISimulatedAnnealingConfiguration, ISmartCloneable<DungeonGeneratorConfiguration<TNode>>
    {
        /// <summary>
        /// Whether non-neighboring rooms may touch (share walls) or not.
        /// The setting is applied only to non-neighboring rooms because all neighbors
        /// share walls as they must be connected by doors.
        /// It is recommended to allow touching rooms if the dungeon is without corridors.
        /// </summary>
        public bool RoomsCanTouch { get; set; } = true;

        /// <summary>
        /// The number of iterations after which the algorithm is stopped even if it
        /// was not able to generate a layout.
        /// Defaults to null - algorithm is not stopped early.
        /// </summary>
        public int? EarlyStopIfIterationsExceeded { get; set; }

        /// <summary>
        /// The time interval after which the algorithm is stopped even if it
        /// was not able to generate a layout.
        /// Defaults to null - algorithm is not stopped early.
        /// </summary>
        public TimeSpan? EarlyStopIfTimeExceeded { get; set; }

        /// <summary>
        /// Whether to override repeat mode of individual room templates.
        /// If this property is set, the chosen repeat mode will be used for all room templates
        /// no matter what their own repeat mode was.
        /// </summary>
        public RepeatMode? RepeatModeOverride { get; set; }

        /// <summary>
        /// Whether to throw an exception when the algorithm is not able to satisfy all the repeat
        /// mode requirements. If set to false, the algorithm will try to lower the requirements,
        /// e.g. instead of no rooms being repeated, it at least tries to not repeat neighbors.
        /// </summary>
        public bool ThrowIfRepeatModeNotSatisfied { get; set; }

        /// <summary>
        /// Chain decomposition configuration that will be used to compute the chains from the input graph.
        /// </summary>
        /// <remarks>
        /// Should not be changed manually.
        /// </remarks>
        public ChainDecompositionConfiguration ChainDecompositionConfiguration { get; set; }

        /// <summary>
        /// Decomposition of the input graph into disjunct subgraphs (chains).
        /// In most cases, this property should not be set and the algorithm will use the ChainDecompositionConfiguration
        /// to compute the chains. 
        /// </summary>
        /// <remarks>
        /// Should not be changed manually.
        /// </remarks>
        public List<Chain<TNode>> Chains { get; set; }

        /// <summary>
        /// Simulated annealing configuration.
        /// </summary>
        /// <remarks>
        /// Should not be changed manually.
        /// </remarks>
        public SimulatedAnnealingConfigurationProvider SimulatedAnnealingConfiguration { get; set; }

        /// <summary>
        /// Maximum branching factor of simulated annealing.
        /// </summary>
        /// <remarks>
        /// Should not be changed manually.
        /// </remarks>
        public int SimulatedAnnealingMaxBranching { get; set; } = 5;

        /* ... */

    }