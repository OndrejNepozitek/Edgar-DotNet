namespace MapGeneration.Core.Experimental
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using ConfigurationSpaces;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Benchmarks;
	using Interfaces.Core;
	using Interfaces.Core.ConfigurationSpaces;
	using Interfaces.Core.LayoutGenerator;
	using Interfaces.Core.MapDescription;

	public class ChainBasedGenerator<TMapDescription, TLayout, TNode, TConfiguration> : IObservableGenerator<TMapDescription, TNode>, ICancellable, IBenchmarkable, IRandomInjectable
		where TMapDescription : IMapDescription<TNode>
		where TLayout : ILayout<TNode, TConfiguration>, ISmartCloneable<TLayout>
	{
		// Algorithms
		private IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace> configurationSpaces;
		private IChainDecomposition<TNode> chainDecomposition;
		private ILayoutEvolver<TLayout, TNode> layoutEvolver;
		private ILayoutOperations<TLayout, TNode> layoutOperations;
		private IGeneratorPlanner<TLayout> generatorPlanner;
		private ILayoutConverter<TLayout, IMapLayout<TNode>> layoutConverter;

		// Creators
		private Func<
			TMapDescription,
			IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace>
		> configurationSpacesCreator;

		private Func<
			TMapDescription,
			IChainDecomposition<TNode>
		> chainDecompositionCreator;

		private Func<
			TMapDescription,
			ILayoutOperations<TLayout, TNode>,
			ILayoutEvolver<TLayout, TNode>
		> layoutEvolverCreator;

		private Func<
			TMapDescription,
			IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace>,
			ILayoutOperations<TLayout, TNode>
		> layoutOperationsCreator;

		private Func<
			TMapDescription,
			IGeneratorPlanner<TLayout>
		> generatorPlannerCreator;

		private Func<
			TMapDescription,
			IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace>,
			ILayoutConverter<TLayout, IMapLayout<TNode>>
		> layoutConverterCreator;
		
		private Func<
			TMapDescription,
			TLayout
		> initialLayoutCreator;


		// Debug and benchmark variables
		private long timeFirst;
		private long timeTotal;
		private int layoutsCount;
		private readonly Stopwatch stopwatch = new Stopwatch();

		protected Random Random;
		protected CancellationToken? CancellationToken;

		private List<List<TNode>> chains;
		private TLayout initialLayout;
		private GeneratorContext context;

		// Settings
		protected bool BenchmarkEnabled;
		protected bool LayoutValidityCheckEnabled;

		// Events
		public event Action<IMapLayout<TNode>> OnPerturbed;
		public event Action<IMapLayout<TNode>> OnPartialValid;
		public event Action<IMapLayout<TNode>> OnValid;

		public IList<IMapLayout<TNode>> GetLayouts(TMapDescription mapDescription, int numberOfLayouts = 10)
		{
			// Create instances and inject the random generator and the cancellation token if possible
			configurationSpaces = configurationSpacesCreator(mapDescription);
			TryInjectRandomAndCancellationToken(configurationSpaces);

			chainDecomposition = chainDecompositionCreator(mapDescription);
			TryInjectRandomAndCancellationToken(chainDecomposition);

			initialLayout = initialLayoutCreator(mapDescription);
			TryInjectRandomAndCancellationToken(initialLayout);

			generatorPlanner = generatorPlannerCreator(mapDescription);
			TryInjectRandomAndCancellationToken(generatorPlanner);

			layoutOperations = layoutOperationsCreator(mapDescription, configurationSpaces);
			TryInjectRandomAndCancellationToken(layoutOperations);

			layoutConverter = layoutConverterCreator(mapDescription, configurationSpaces);
			TryInjectRandomAndCancellationToken(layoutConverter);

			layoutEvolver = layoutEvolverCreator(mapDescription, layoutOperations);
			TryInjectRandomAndCancellationToken(layoutEvolver);


			var graph = mapDescription.GetGraph();
			chains = chainDecomposition.GetChains(graph);
			context = new GeneratorContext();

			RegisterEventHandlers();

			// Restart stopwatch
			stopwatch.Restart();

			// TODO: handle number of layouts to be evolved - who should control that? generator or planner?
			// TODO: handle context.. this is ugly
			var layouts = generatorPlanner.Generate(initialLayout, numberOfLayouts, chains.Count,
				(layout, chainNumber) => layoutEvolver.Evolve(AddChain(layout, chainNumber), chains[chainNumber], 5), context);

			// Stop stopwatch and prepare benchmark info
			stopwatch.Stop();
			timeTotal = stopwatch.ElapsedMilliseconds;
			layoutsCount = layouts.Count;

			// Reset cancellation token if it was already used
			if (CancellationToken.HasValue && CancellationToken.Value.IsCancellationRequested)
			{
				CancellationToken = null;
			}

			return layouts.Select(x => layoutConverter.Convert(x, true)).ToList();
		}

		protected virtual TLayout AddChain(TLayout layout, int chainNumber)
		{
			if (chainNumber >= chains.Count)
				throw new ArgumentException("Chain number is bigger than then number of chains.", nameof(chainNumber));

			layout = layout.SmartClone();

			layoutOperations.AddChain(layout, chains[chainNumber], true);

			return layout;
		}


		#region Event handlers

		private void RegisterEventHandlers()
		{
			// Register validity checks
			layoutEvolver.OnPerturbed -= CheckLayoutValidity;
			if (LayoutValidityCheckEnabled)
			{
				layoutEvolver.OnPerturbed += CheckLayoutValidity;
			}

			// Register iterations counting
			layoutEvolver.OnPerturbed -= IterationsCounterHandler;
			layoutEvolver.OnPerturbed += IterationsCounterHandler;

			layoutEvolver.OnPerturbed -= PerturbedLayoutsHandler;
			layoutEvolver.OnPerturbed += PerturbedLayoutsHandler;

			layoutEvolver.OnValid -= PartialValidLayoutsHandler;
			layoutEvolver.OnValid += PartialValidLayoutsHandler;

			// Setup first layout timer
			timeFirst = -1;
			generatorPlanner.OnLayoutGenerated -= FirstLayoutTimeHandler;
			generatorPlanner.OnLayoutGenerated += FirstLayoutTimeHandler;

			generatorPlanner.OnLayoutGenerated -= ValidLayoutsHandler;
			generatorPlanner.OnLayoutGenerated += ValidLayoutsHandler;
		}

		private void FirstLayoutTimeHandler(TLayout layout)
		{
			if (timeFirst == -1)
			{
				timeFirst = stopwatch.ElapsedMilliseconds;
			}
		}

		private void IterationsCounterHandler(TLayout layout)
		{
			context.IterationsCount++;
		}

		private void ValidLayoutsHandler(TLayout layout)
		{
			OnValid?.Invoke(layoutConverter.Convert(layout, true));
		}

		private void PartialValidLayoutsHandler(TLayout layout)
		{
			OnPartialValid?.Invoke(layoutConverter.Convert(layout, true));
		}

		private void PerturbedLayoutsHandler(TLayout layout)
		{
			OnPerturbed?.Invoke(layoutConverter.Convert(layout, false));
		}

		#endregion


		#region Creators

		public void SetLayoutEvolverCreator(Func<TMapDescription, ILayoutOperations<TLayout, TNode>, ILayoutEvolver<TLayout, TNode>> creator)
		{
			layoutEvolverCreator = creator;
		}

		public void SetLayoutConverterCreator(Func<TMapDescription, IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace>, ILayoutConverter<TLayout, IMapLayout<TNode>>> creator)
		{
			layoutConverterCreator = creator;
		}

		public void SetGeneratorPlannerCreator(Func<TMapDescription, IGeneratorPlanner<TLayout>> creator)
		{
			generatorPlannerCreator = creator;
		}

		public void SetInitialLayoutCreator(Func<TMapDescription, TLayout> creator)
		{
			initialLayoutCreator = creator;
		}

		public void SetLayoutOperationsCreator(Func<TMapDescription, IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace>, ILayoutOperations<TLayout, TNode>> creator)
		{
			layoutOperationsCreator = creator;
		}

		public void SetConfigurationSpacesCreator(Func<TMapDescription, IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace>> creator)
		{
			configurationSpacesCreator = creator;
		}

		public void SetChainDecompositionCreator(Func<TMapDescription, IChainDecomposition<TNode>> creator)
		{
			chainDecompositionCreator = creator;
		}

		public void SetGeneratorPlanner(IGeneratorPlanner<TLayout> planner)
		{
			generatorPlanner = planner;
		}

		#endregion


		/// <summary>
		/// Checks whether energies and validity vectors are the same as if they are all recomputed.
		/// </summary>
		/// <remarks>
		/// This check significantly slows down the generator.
		/// </remarks>
		/// <param name="enable"></param>
		public void SetLayoutValidityCheck(bool enable)
		{
			LayoutValidityCheckEnabled = enable;
		}

		private void CheckLayoutValidity(TLayout layout)
		{
			var copy = layout.SmartClone();

			layoutOperations.UpdateLayout(copy);

			foreach (var vertex in layout.Graph.Vertices)
			{
				var isInLayout = layout.GetConfiguration(vertex, out var configurationLayout);
				var isInCopy = copy.GetConfiguration(vertex, out var configurationCopy);

				if (isInLayout != isInCopy)
					throw new InvalidOperationException("Vertices must be either set in both or absent in both");

				// Skip the check if the configuration is not set
				if (!isInLayout)
					continue;

				if (!configurationCopy.Equals(configurationLayout))
					throw new InvalidOperationException("Configurations must be equal");
			}
		}

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

		public void SetCancellationToken(CancellationToken? cancellationToken)
		{
			CancellationToken = cancellationToken;
		}

		long IBenchmarkable.TimeFirst => timeFirst;

		long IBenchmarkable.TimeTotal => timeTotal;

		int IBenchmarkable.IterationsCount => context.IterationsCount;

		int IBenchmarkable.LayoutsCount => layoutsCount;

		public void EnableBenchmark(bool enable)
		{
			BenchmarkEnabled = true;
		}

		public void InjectRandomGenerator(Random random)
		{
			Random = random;
		}
	}
}