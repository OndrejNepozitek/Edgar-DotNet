using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Edgar.Legacy.Core.ChainDecompositions;
using Edgar.Legacy.Core.GeneratorPlanners.Interfaces;
using Edgar.Legacy.Core.LayoutEvolvers.Interfaces;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.Legacy.Core.GeneratorPlanners
{
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
	public class GeneratorPlanner<TLayout, TNode> : IGeneratorPlanner<TLayout, TNode>, ICancellable
        where TLayout : ISmartCloneable<TLayout>
	{
		private TLayout initialLayout;
		private List<NodeRow> rows;
        private int nextId;
        private readonly int numberOfLayoutsFromOneInstance;
        private ILayoutEvolver<TLayout, TNode> layoutEvolver;
        private List<Chain<TNode>> chains;

        protected CancellationToken? CancellationToken;

		public event Action<TLayout> OnLayoutGenerated;

        public GeneratorPlanner(int maximumBranching = 5)
        {
            numberOfLayoutsFromOneInstance = maximumBranching;
        }

        public TLayout Generate(TLayout initialLayout, List<Chain<TNode>> chains, ILayoutEvolver<TLayout, TNode> layoutEvolver)
        {
            // Initialization
            this.initialLayout = initialLayout;
            this.layoutEvolver = layoutEvolver;
            this.chains = chains;

            rows = new List<NodeRow>();
            nextId = 0;
            var chainsCount = chains.Count;

            var finalLayout = default(TLayout);

            AddZeroLevelNode();

            while (true)
            {
                if (CancellationToken.HasValue && CancellationToken.Value.IsCancellationRequested)
                    break;

                // TODO: add some kind of check later
                //if (context.IterationsCount > 1000000)
                //    break;

                // Gets next instance to be extended
                var instance = GetNextInstance(rows);
                var newDepth = instance.Depth + 1;

                // The chosen instance must not be already finished
                if (instance.IsFinished)
                    throw new InvalidOperationException();

                // Tries to generate a new layout from the instance
                // var iterationsBefore = context.IterationsCount;
                var hasLayout = instance.TryGetLayout(out var layout);
                // var iterationsToGenerate = context.IterationsCount - iterationsBefore;
                // instance.AddIterations(iterationsToGenerate);

                // Layout being null means failure and the current iteration is skipped
                if (!hasLayout)
                {
                    // log.Add(new LogEntry(LogType.Fail, instance, iterationsToGenerate));
                    continue;
                }

                // Check if the layout has already all the chains added
                if (newDepth == chainsCount)
                {
                    OnLayoutGenerated?.Invoke(layout);
                    finalLayout = layout;
                    // log.Add(new LogEntry(LogType.Final, instance, iterationsToGenerate));
                    break;
                }

                // A new instance is created from the generated layout and added to the tree.
                var nextInstance = new Node(instance, GetLayouts(layout, newDepth), newDepth, 0 /* TODO: iterationsToGenerate */, nextId++);
                instance.AddChild(nextInstance);
                AddNode(nextInstance, newDepth);
                // log.Add(new LogEntry(LogType.Success, nextInstance, iterationsToGenerate));
            }

            return finalLayout;
        }

        /// <summary>
		/// Returns a node that should be extended next.
		/// </summary>
		/// <remarks>
		/// The returned instance must not be already finished.
		/// </remarks>
		/// <param name="rows"></param>
		/// <returns></returns>
        protected Node GetNextInstance(List<NodeRow> rows)
        {
            var depth = rows.Count - 1;

            while (depth >= 0)
            {
                var row = rows[depth];
                var instance = row.Instances.FirstOrDefault(x => !x.IsFinished);

                if (instance == null)
                {
                    depth--;
                    continue;
                }

                return instance;
            }

            return AddZeroLevelNode();
        }

        /// <summary>
		/// Gets an IEnumerable of extended layouts from given initial layout and chain number.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="chainNumber"></param>
		/// <returns></returns>
		private IEnumerable<TLayout> GetLayouts(TLayout layout, int chainNumber)
		{
            return layoutEvolver.Evolve(layout.SmartClone(), chains[chainNumber], numberOfLayoutsFromOneInstance);
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
			// log.Add(new LogEntry(LogType.Success, instance, 0));

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

		public virtual void SetCancellationToken(CancellationToken? cancellationToken)
		{
			CancellationToken = cancellationToken;
		}
    }
}