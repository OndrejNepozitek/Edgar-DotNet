using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Common.RoomShapeGeometry;
using Edgar.Graphs;
using Edgar.Legacy.Core.LayoutOperations.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.Common
{
    /// <inheritdoc cref="IChainBasedLayoutOperations{TLayout,TNode}" />
	/// <summary>
	/// Base class for layout operations.
	/// </summary>
	public abstract class AbstractLayoutController<TLayout, TNode, TConfiguration, TShapeContainer> : IChainBasedLayoutOperations<TLayout, TNode>, IRandomInjectable
		where TLayout : ILayout<TNode, TConfiguration>, ISmartCloneable<TLayout>
		where TConfiguration : IConfiguration<TShapeContainer, Vector2Int, TNode>, ISmartCloneable<TConfiguration>
	{
        protected Random Random;
		protected float ShapePerturbChance = 0.4f;
		protected float DifferenceFromAverageScale = 0.4f;
		protected int AverageSize;
        protected readonly ILevelDescription<TNode> LevelDescription;
        protected readonly IGraph<TNode> StageOneGraph;
        protected readonly IRoomShapesHandler<TLayout, TNode, TShapeContainer> RoomShapesHandler;
        protected readonly IRoomShapeGeometry<TConfiguration> RoomShapeGeometry;

		protected AbstractLayoutController(int averageSize, ILevelDescription<TNode> levelDescription, IRoomShapesHandler<TLayout, TNode, TShapeContainer> roomShapesHandler, IRoomShapeGeometry<TConfiguration> roomShapeGeometry)
		{
            AverageSize = averageSize;
            LevelDescription = levelDescription;
            RoomShapesHandler = roomShapesHandler;
            RoomShapeGeometry = roomShapeGeometry;
            StageOneGraph = levelDescription.GetGraphWithoutCorridors();
        }

		/// <inheritdoc />
		public virtual void InjectRandomGenerator(Random random)
		{
			Random = random;
        }

		/// <inheritdoc />
		/// <summary>
		/// Perturbs a node by getting random shapes from configurations spaces until a
		/// different shape is found.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="node"></param>
		/// <param name="updateLayout"></param>
		public virtual void PerturbShape(TLayout layout, TNode node, bool updateLayout)
		{
            throw new NotSupportedException();
		}

        /// <inheritdoc />
		public virtual void PerturbShape(TLayout layout, IList<TNode> chain, bool updateLayout)
		{
            throw new NotSupportedException();
		}

		/// <inheritdoc />
		/// <summary>
		/// Pertubs a position of a given node by getting a random point from a maximum
		/// intersection of configuration space of already laid out neighbours. TODO: is "laid out" ok?
		/// </summary>
		public virtual void PerturbPosition(TLayout layout, TNode node, bool updateLayout)
		{
            throw new NotSupportedException();
		}

		/// <inheritdoc />
		public virtual void PerturbPosition(TLayout layout, IList<TNode> chain, bool updateLayout)
		{
			// TODO: check what would happen if only invalid nodes could be perturbed
			var canBePerturbed = chain
                .Where(x => !LevelDescription.GetRoomDescription(x).IsCorridor) // TODO: handle
                .ToList();

			if (canBePerturbed.Count == 0)
				return;

			PerturbPosition(layout, canBePerturbed.GetRandom(Random), updateLayout);
		}

		/// <inheritdoc />
		/// <summary>
		/// Perturbs a given layout by first choosing whether to perturb a shape or a positions
		/// and than calling corresponding methods.
		/// </summary>
		public virtual void PerturbLayout(TLayout layout, IList<TNode> chain, bool updateLayout)
		{
			if (Random.NextDouble() <= ShapePerturbChance)
			{
				PerturbShape(layout, chain, updateLayout);
			}
			else
			{
				PerturbPosition(layout, chain, updateLayout);
			}
		}

		/// <inheritdoc />
		/// <summary>
		/// Adds a given chain by greedily adding nodes one by one.
		/// </summary>
		public virtual void AddChain(TLayout layout, IList<TNode> chain, bool updateLayout, out int iterationsCount)
        {
            iterationsCount = 0;

			foreach (var node in chain)
			{
				AddNodeGreedily(layout, node, out var addNodeIterations);
                iterationsCount += addNodeIterations;
            }

			if (updateLayout)
			{
				UpdateLayout(layout);
			}
		}

        /// <inheritdoc />
        /// <remarks>
        /// Do nothing. Implementers may override.
        /// </remarks>
        public virtual bool TryCompleteChain(TLayout layout, IList<TNode> chain)
        {
            return true;
        }

        /// <inheritdoc />
		public virtual bool AreDifferentEnough(TLayout layout1, TLayout layout2)
		{
			return AreDifferentEnough(layout1, layout2, layout1.Graph.Vertices.ToList());
		}

		/// <inheritdoc />
		/// <summary>
		/// Checks if two layouts are different enough by comparing positions of corresponding nodes.
		/// </summary>
		public virtual bool AreDifferentEnough(TLayout layout1, TLayout layout2, IList<TNode> chain)
		{
			// TODO: make better
			var diff = 0d;

			foreach (var node in chain)
			{
				if (layout1.GetConfiguration(node, out var c1) && layout2.GetConfiguration(node, out var c2))
				{
					// Changed

					//diff += (float)(Math.Pow(
					//	                5 * IntVector2.ManhattanDistance(c1.Shape.BoundingRectangle.Center + c1.Position,
					//		                c2.Shape.BoundingRectangle.Center + c2.Position) / (float)AverageSize, 2) * (ReferenceEquals(c1.Shape, c2.Shape) ? 1 : 4));

                    diff += (float)(Math.Pow(
                        5 * RoomShapeGeometry.GetCenterDistance(c1, c2) / (float)AverageSize, 2) * (ReferenceEquals(c1.RoomShape, c2.RoomShape) ? 1 : 4));
				}
			}

			diff = diff / (chain.Count());

			return DifferenceFromAverageScale * diff >= 1;
		}

		/// <inheritdoc />
		public abstract bool IsLayoutValid(TLayout layout);

		/// <inheritdoc />
		public abstract bool IsLayoutValid(TLayout layout, IList<TNode> chain);

		/// <inheritdoc />
		public abstract float GetEnergy(TLayout layout);

		/// <inheritdoc />
		public abstract void UpdateLayout(TLayout layout);

		/// <inheritdoc />
		public abstract void AddNodeGreedily(TLayout layout, TNode node, out int iterationsCount);

		/// <summary>
		/// Updates energies after perturbing a given node.
		/// </summary>
		/// <remarks>
		/// This method is responsible for modifying the layout by setting the configuration
		/// to the perturbed node.
		/// </remarks>
		/// <param name="layout">Original layout.</param>
		/// <param name="perturbedNode">Node that was perturbed.</param>
		/// <param name="configuration">New configuration of the perturbed node.</param>
		protected abstract void UpdateLayout(TLayout layout, TNode perturbedNode, TConfiguration configuration);
	}
}