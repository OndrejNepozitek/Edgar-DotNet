namespace MapGeneration.Core.SimulatedAnnealing.GeneratorPlanner
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Base class for generator planners.
	/// Inheriting classes must implement just GetNextInstance() that is called
	/// when a new layout should be chosen to be extended.
	/// </summary>
	public abstract class GeneratorPlannerBase<TLayout> : IGeneratorPlanner<TLayout>
	{
		private TLayout initialLayout;
		private List<List<int>> chains;
		private Func<TLayout, List<int>, IEnumerable<TLayout>> simulatedAnnealing;
		private List<InstanceRow> rows;
		private List<LogEntry> log;
		private int nextId;

		public event Action<TLayout> OnLayoutGenerated;

		/// <summary>
		/// Calls GetNextInstance() and builds the tree data structure until a given number of layouts is generated.
		/// </summary>
		/// <param name="initialLayout"></param>
		/// <param name="chains"></param>
		/// <param name="simulatedAnnealing"></param>
		/// <param name="context"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public List<TLayout> Generate(TLayout initialLayout, List<List<int>> chains, Func<TLayout, List<int>, IEnumerable<TLayout>> simulatedAnnealing, ISAContext context, int count)
		{
			// Initialization
			this.initialLayout = initialLayout;
			this.chains = chains;
			this.simulatedAnnealing = simulatedAnnealing;
			rows = new List<InstanceRow>();
			log = new List<LogEntry>();
			nextId = 0;
			var layouts = new List<TLayout>();

			BeforeGeneration();
			AddZeroLevelInstance();

			while (layouts.Count < count)
			{
				if (context.CancellationToken.HasValue && context.CancellationToken.Value.IsCancellationRequested)
					break;

				if (context.IterationsCount > 1000000)
					break;

				// Gets next instance to be extended
				var instance = GetNextInstance(rows);
				var newDepth = instance.Depth + 1;

				// The chosen instance must not be already finished
				if (instance.IsFinished)
					throw new InvalidOperationException();

				// Tries to generate a new layout from the instance
				var iterationsBefore = context.IterationsCount;
				var hasLayout = instance.TryGetLayout(out var layout);
				var iterationsToGenerate = context.IterationsCount - iterationsBefore;
				instance.AddIterations(iterationsToGenerate);

				// Layout being null means failure and the current iteration is skipped
				if (!hasLayout)
				{
					log.Add(new LogEntry(LogType.Fail, instance, iterationsToGenerate));
					continue;
				}

				// Check if the layout has already all the chains added
				if (newDepth == chains.Count)
				{
					OnLayoutGenerated?.Invoke(layout);
					layouts.Add(layout);
					log.Add(new LogEntry(LogType.Final, instance, iterationsToGenerate));
					continue;
				}
				
				// A new instance is created from the generated layout and added to the tree.
				var saInstance = GetSAInstance(layout, chains[newDepth]);
				var nextInstance = new Instance(instance, saInstance, newDepth, iterationsToGenerate, nextId++);
				instance.AddChild(nextInstance);
				AddInstance(nextInstance, newDepth);
				log.Add(new LogEntry(LogType.Success, nextInstance, iterationsToGenerate));
			}

			AfterGeneration();

			return layouts;
		}

		/// <summary>
		/// Returns a next instance to be extended with a corresponding chain.
		/// </summary>
		/// <remarks>
		/// The returned instance must not be already finished.
		/// </remarks>
		/// <param name="rows"></param>
		/// <returns></returns>
		protected abstract Instance GetNextInstance(List<InstanceRow> rows);

		/// <summary>
		/// Is called before the generation.
		/// </summary>
		/// Should be called before any custom implementation.
		protected virtual void BeforeGeneration()
		{

		}

		/// <summary>
		/// Is called after the generation.
		/// </summary>
		/// <remarks>
		/// Should be called before any custom implementation.
		/// </remarks>
		protected virtual void AfterGeneration()
		{

		}

		/// <summary>
		/// Creates a SAInstance from given layout with given chain.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chain"></param>
		/// <returns></returns>
		private SAInstance<TLayout> GetSAInstance(TLayout layout, List<int> chain)
		{
			var layouts = simulatedAnnealing(layout, chain);

			return new SAInstance<TLayout>(layouts);
		}

		/// <inheritdoc />
		/// <summary>
		/// Returns a tree-like structure of the planning.
		/// </summary>
		/// <returns></returns>
		public virtual string GetLog()
		{
			var builder = new StringBuilder();

			Instance previousInstance = null;

			// Process every log entry
			foreach (var logEntry in log)
			{
				var instance = logEntry.Instance;

				// TODO: maybe change the output?
				if (instance.Depth == 0)
				{
					builder.AppendLine(GetLogChain(instance, previousInstance));
					previousInstance = instance;
					continue;
				}

				var childIndex = instance.Parent.Children.FindIndex(x => x == instance);
				string id;

				switch (logEntry.Type)
				{
					case LogType.Fail:
						id = $"FAIL";
						break;
					case LogType.Success:
						id = $"Id: {instance.Id.ToString().PadLeft(3, '0')}";
						break;
					case LogType.Final:
						id = $"FINAL";
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				var detailedInfo = logEntry.Type == LogType.Fail ? string.Empty :
					$", {childIndex + 1}/{instance.Parent.Children.Count}, Parent finished: {childIndex == instance.Parent.Children.Count - 1 && instance.Parent.IsFinished}";
				var info = $"[{id}, Iterations: {logEntry.IterationsCount}{detailedInfo}]";
				var prefix = GetLogChain(logEntry.Type == LogType.Fail ? instance : instance.Parent, previousInstance);

				builder.AppendLine(prefix + info);
				previousInstance = instance;
			}

			return builder.ToString();
		}

		/// <summary>
		/// Returns a prefix for a current log entry.
		/// </summary>
		/// The goal is to make it as readable as possible so if two following entries
		/// have part of the prefix equal, that part is then skipped.
		/// <param name="instance"></param>
		/// <param name="previousInstance"></param>
		/// <returns></returns>
		protected string GetLogChain(Instance instance, Instance previousInstance)
		{
			var output = "";

			if (previousInstance != null && instance != null)
			{
				while (previousInstance != null && previousInstance.Depth > instance.Depth)
				{
					previousInstance = previousInstance.Parent;
				}
			}

			while (instance != null)
			{
				if (previousInstance != null && instance.Depth < previousInstance.Depth)
				{
					previousInstance = previousInstance.Parent;
				}

				if (previousInstance != null && instance == previousInstance)
				{
					output = new string(' ', (previousInstance.Depth + 1) * 6) + output;
					break;
				}

				output = $"[{instance.Id.ToString().PadLeft(3, '0')}] " + output;
				instance = instance.Parent;
			}

			return output;
		}

		/// <summary>
		/// Helper method to add a new instance to the data structure at a given depth.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="depth"></param>
		private void AddInstance(Instance instance, int depth)
		{
			if (rows.Count <= depth)
			{
				rows.Add(new InstanceRow());
			}

			rows[depth].Instances.Add(instance);
		}

		/// <summary>
		/// Uses the initial layout to create an instance on the zero-th level.
		/// </summary>
		/// <returns></returns>
		protected Instance AddZeroLevelInstance()
		{
			var instance = new Instance(null, GetSAInstance(initialLayout, chains[0]), 0, 0, nextId++);
			AddInstance(instance, 0);
			log.Add(new LogEntry(LogType.Success, instance, 0));

			return instance;
		}

		/// <summary>
		/// Class holding an instance of the simulated annealing together with information
		/// that can be used for better logging or for smarter choosing of the next instance.
		/// </summary>
		protected class Instance
		{
			/// <summary>
			/// Parent of this instance. Null if on the zero level.
			/// </summary>
			public Instance Parent { get; }

			/// <summary>
			/// All instance that were generated from this instance.
			/// </summary>
			public List<Instance> Children { get; } = new List<Instance>();

			private SAInstance<TLayout> SAInstance { get; }

			/// <summary>
			/// The depth of the layout. It means that Depth + 1 chains were already added.
			/// </summary>
			public int Depth { get; }

			/// <summary>
			/// How many iterations were needed to generated this instance.
			/// </summary>
			public int IterationsToGenerate { get; }

			/// <summary>
			/// How many iterations were needed to generated all children of this instance.
			/// </summary>
			public int IterationsGeneratingChildren { get; private set; }

			/// <summary>
			/// Whether all layouts were already generated from this instance.
			/// </summary>
			public bool IsFinished { get; private set; }

			/// <summary>
			/// Id of this instance for logging purposes.
			/// </summary>
			public int Id { get; }

			public Instance(Instance parent, SAInstance<TLayout> saInstance, int depth, int iterationsToGenerate, int id)
			{
				Parent = parent;
				SAInstance = saInstance;
				Depth = depth;
				IterationsToGenerate = iterationsToGenerate;
				Id = id;
			}

			/// <summary>
			/// Adds a child to the instance.
			/// </summary>
			/// <param name="instance"></param>
			public void AddChild(Instance instance)
			{
				Children.Add(instance);
			}

			/// <summary>
			/// Tries to get next layout.
			/// </summary>
			/// <returns>Null if not successful.</returns>
			public bool TryGetLayout(out TLayout layout)
			{
				if (SAInstance.TryGetLayout(out var layoutInner))
				{
					layout = layoutInner;
					return true;
				}

				IsFinished = true;
				layout = default(TLayout);
				return false;
			}

			/// <summary>
			/// Method used to log how many iterations were used when generating
			/// layouts from this instance.
			/// </summary>
			/// <param name="count"></param>
			public void AddIterations(int count)
			{
				IterationsGeneratingChildren += count;
			}
		}

		/// <summary>
		/// Class holding a row of instances.
		/// </summary>
		protected class InstanceRow
		{
			public List<Instance> Instances { get; } = new List<Instance>();
		}

		private class LogEntry
		{
			public LogType Type { get; }

			public Instance Instance { get; }

			public int IterationsCount { get; }

			public LogEntry(LogType type, Instance instance, int iterationsCount)
			{
				Type = type;
				Instance = instance;
				IterationsCount = iterationsCount;
			}
		}

		private enum LogType
		{
			Fail, Success, Final
		}
	}
}