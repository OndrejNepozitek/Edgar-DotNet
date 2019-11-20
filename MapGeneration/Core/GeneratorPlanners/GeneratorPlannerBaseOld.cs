namespace MapGeneration.Core.GeneratorPlanners
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Threading;
	using Interfaces.Core;
	using Interfaces.Core.GeneratorPlanners;
	using Interfaces.Core.LayoutGenerator;
	using Interfaces.Utils;

	/// <summary>
	/// Class that builds a tree of nodes where each node represent a layout that can be
	/// used as an initial layout to generate more layouts until a full layout is generated.
	/// </summary>
	/// <remarks>
	/// Each node in the tree has the number of added chains equal to the number of the row
	/// on which it is located.
	/// 
	/// Inheriting classes can shape the tree by specifying in each step which node should
	/// be extended.
	/// </remarks>
	public abstract class GeneratorPlannerBaseOld<TLayout> : IGeneratorPlannerOld<TLayout>, ICancellable
	{
		private TLayout initialLayout;
		private List<NodeRow> rows;
		private List<LogEntry> log;
		private int nextId;
		private LayoutGeneratorFunction<TLayout> layoutGeneratorFunc;
		private int numberOfLayoutsFromOneInstance = 5;

		protected CancellationToken? CancellationToken;

		public event Action<TLayout> OnLayoutGenerated;

		/// <inheritdoc />
		/// <remarks>
		/// Calls abstract GetNextInstance() to build a tree of nodes until a given number of layouts is generated.
		/// </remarks>
		public List<TLayout> Generate(TLayout initialLayout, int count, int chainsCount, LayoutGeneratorFunction<TLayout> layoutGeneratorFunc, IGeneratorContext context)
		{
			// Initialization
			this.initialLayout = initialLayout;
			rows = new List<NodeRow>();
			log = new List<LogEntry>();
			nextId = 0;
			this.layoutGeneratorFunc = layoutGeneratorFunc;
			var layouts = new List<TLayout>();

			BeforeGeneration();
			AddZeroLevelNode();

			while (layouts.Count < count)
			{
				if (CancellationToken.HasValue && CancellationToken.Value.IsCancellationRequested)
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
				if (newDepth == chainsCount)
				{
					OnLayoutGenerated?.Invoke(layout);
					layouts.Add(layout);
					log.Add(new LogEntry(LogType.Final, instance, iterationsToGenerate));
					AfterValid();
					continue;
				}
				
				// A new instance is created from the generated layout and added to the tree.
				var nextInstance = new Node(instance, GetLayouts(layout, newDepth), newDepth, iterationsToGenerate, nextId++);
				instance.AddChild(nextInstance);
				AddNode(nextInstance, newDepth);
				log.Add(new LogEntry(LogType.Success, nextInstance, iterationsToGenerate));
			}

			AfterGeneration();

			return layouts;
		}

		/// <summary>
		/// Returns a node that should be extended next.
		/// </summary>
		/// <remarks>
		/// The returned instance must not be already finished.
		/// </remarks>
		/// <param name="rows"></param>
		/// <returns></returns>
		protected abstract Node GetNextInstance(List<NodeRow> rows);

		/// <summary>
		/// Is called before the generation.
		/// </summary>
		/// Should be called before any custom implementation.
		protected virtual void BeforeGeneration()
		{
			/* empty */
		}

		/// <summary>
		/// Is called after the generation.
		/// </summary>
		/// <remarks>
		/// Should be called before any custom implementation.
		/// </remarks>
		protected virtual void AfterGeneration()
		{
			/* empty */
		}

		/// <summary>
		/// Is called after a layout with all chains is generated.
		/// </summary>
		/// <remarks>
		/// Should be called before any custom implementation.
		/// </remarks>
		protected virtual void AfterValid()
		{
			/* empty */
		}

		/// <summary>
		/// Clears the whole tree.
		/// </summary>
		/// <remarks>
		/// Useful when one want to start over when a valid layout is generated.
		/// </remarks>
		protected void ResetRows()
		{
			rows = new List<NodeRow>();
		}

		/// <summary>
		/// Sets how many layouts should be generated from each instance.
		/// </summary>
		/// <param name="numberOfLayouts"></param>
		protected void SetNumberOfLayoutsToGenerate(int numberOfLayouts)
		{
			numberOfLayoutsFromOneInstance = numberOfLayouts;
		}

		/// <summary>
		/// Gets an IEnumerable of extended layouts from given initial layout and chain number.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chainNumber"></param>
		/// <returns></returns>
		private IEnumerable<TLayout> GetLayouts(TLayout layout, int chainNumber)
		{
			var layouts = layoutGeneratorFunc(layout, chainNumber, numberOfLayoutsFromOneInstance);

			return layouts;
		}

		/// <inheritdoc />
		/// <summary>
		/// Returns a tree-like structure of the planning.
		/// </summary>
		/// <returns></returns>
		public virtual string GetLog()
		{
			var builder = new StringBuilder();

			Node previousNode = null;

			// Process every log entry
			foreach (var logEntry in log)
			{
				var instance = logEntry.Node;

				// TODO: maybe change the output?
				if (instance.Depth == 0)
				{
					builder.AppendLine(GetLogChain(instance, previousNode));
					previousNode = instance;
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
				var prefix = GetLogChain(logEntry.Type == LogType.Fail ? instance : instance.Parent, previousNode);

				builder.AppendLine(prefix + info);
				previousNode = instance;
			}

			return builder.ToString();
		}

		/// <summary>
		/// Returns a prefix for a current log entry.
		/// </summary>
		/// The goal is to make it as readable as possible so if two following entries
		/// have part of the prefix equal, that part is then skipped.
		/// <param name="node"></param>
		/// <param name="previousNode"></param>
		/// <returns></returns>
		protected string GetLogChain(Node node, Node previousNode)
		{
			var output = "";

			if (previousNode != null && node != null)
			{
				while (previousNode != null && previousNode.Depth > node.Depth)
				{
					previousNode = previousNode.Parent;
				}
			}

			while (node != null)
			{
				if (previousNode != null && node.Depth < previousNode.Depth)
				{
					previousNode = previousNode.Parent;
				}

				if (previousNode != null && node == previousNode)
				{
					output = new string(' ', (previousNode.Depth + 1) * 6) + output;
					break;
				}

				output = $"[{node.Id.ToString().PadLeft(3, '0')}] " + output;
				node = node.Parent;
			}

			return output;
		}

		/// <summary>
		/// Helper method to add a new node to the data structure at a given depth.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="depth"></param>
		private void AddNode(Node node, int depth)
		{
			if (rows.Count <= depth)
			{
				rows.Add(new NodeRow());
			}

			rows[depth].Instances.Add(node);
		}

		/// <summary>
		/// Uses the initial layout to create a node on the zero-th level.
		/// </summary>
		/// <returns></returns>
		protected Node AddZeroLevelNode()
		{
			var instance = new Node(null, GetLayouts(initialLayout, 0), 0, 0, nextId++);
			AddNode(instance, 0);
			log.Add(new LogEntry(LogType.Success, instance, 0));

			return instance;
		}

		/// <summary>
		/// Class holding an enumerator created from a layout evolver.
		/// It represents a node in a tree.
		/// </summary>
		protected class Node
		{
			private readonly IEnumerator<TLayout> enumerator;

			/// <summary>
			/// Parent of this instance. Null if on the zero level.
			/// </summary>
			public Node Parent { get; }

			/// <summary>
			/// All instance that were generated from this instance.
			/// </summary>
			public List<Node> Children { get; } = new List<Node>();

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

			public Node(Node parent, IEnumerable<TLayout> layouts, int depth, int iterationsToGenerate, int id)
			{
				Parent = parent;
				enumerator = layouts.GetEnumerator();
				Depth = depth;
				IterationsToGenerate = iterationsToGenerate;
				Id = id;
			}

			/// <summary>
			/// Adds a child to the instance.
			/// </summary>
			/// <param name="node"></param>
			public void AddChild(Node node)
			{
				Children.Add(node);
			}

			/// <summary>
			/// Tries to get next layout.
			/// </summary>
			/// <returns>Null if not successful.</returns>
			public bool TryGetLayout(out TLayout layout)
			{
				var hasMore = enumerator.MoveNext();
				layout = hasMore ? enumerator.Current : default(TLayout);

				if (!hasMore)
				{
					IsFinished = true;
				}

				return hasMore;
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
		/// Class holding a row of nodes.
		/// </summary>
		protected class NodeRow
		{
			public List<Node> Instances { get; } = new List<Node>();
		}

		private class LogEntry
		{
			public LogType Type { get; }

			public Node Node { get; }

			public int IterationsCount { get; }

			public LogEntry(LogType type, Node node, int iterationsCount)
			{
				Type = type;
				Node = node;
				IterationsCount = iterationsCount;
			}
		}

		private enum LogType
		{
			Fail, Success, Final
		}

		public virtual void SetCancellationToken(CancellationToken? cancellationToken)
		{
			CancellationToken = cancellationToken;
		}
	}
}