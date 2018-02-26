//namespace MapGeneration.Core
//{
//	using System;
//	using System.Collections.Generic;
//	using System.Linq;
//	using GeneralAlgorithms.Algorithms.Polygons;
//	using GeneralAlgorithms.DataStructures.Common;
//	using Interfaces;
//	using Utils;

//	public class SlowLayoutOperations<TNode, TLayout, TConfiguration, TShapeContainer> : IRandomInjectable
//		where TLayout : ILayout<TNode, TConfiguration>
//		where TConfiguration : IMutableConfiguration<TConfiguration, TShapeContainer>
//	{
//		private readonly IConfigurationSpaces<TNode, TShapeContainer, TConfiguration> configurationSpaces;
//		private readonly PolygonOverlap polygonOverlap = new PolygonOverlap();
//		private Random random = new Random();

//		private const float EnergySigma = 300f; // TODO: change

//		public SlowLayoutOperations(IConfigurationSpaces<TNode, TShapeContainer, TConfiguration> configurationSpaces)
//		{
//			this.configurationSpaces = configurationSpaces;
//		}

//		public TLayout PerturbShape(TLayout layout, TNode node, bool updateEnergies = true)
//		{
//			layout.GetConfiguration(node, out var configuration);

//			// Return the current layout if given node cannot be shape-perturbed
//			if (!configurationSpaces.CanPerturbShape(node))
//				return layout;

//			TShapeContainer shape;
//			do
//			{
//				shape = configurationSpaces.GetRandomShape(node);
//			}
//			while (ReferenceEquals(shape, configuration.Shape));

//			var newConfiguration = configuration.SetShape(shape);

//			return UpdateLayoutAfterPerturbation(layout, node, newConfiguration);
//		}

//		public TLayout PerturbShape(TLayout layout, IList<TNode> nodeOptions, bool updateEnergies = true)
//		{
//			var canBePerturbed = nodeOptions.Where(x => configurationSpaces.CanPerturbShape(x)).ToList();

//			if (canBePerturbed.Count == 0)
//				return layout;

//			return PerturbShape(layout, canBePerturbed.GetRandom(random));
//		}

//		protected TLayout UpdateLayoutAfterPerturbation(TLayout layout, TNode node, TConfiguration configuration)
//		{
//			throw new NotImplementedException();
//		}

//		public TLayout PerturbPosition(TLayout layout, bool updateEnergies = true)
//		{
//			throw new NotImplementedException();
//		}

//		/// <summary>
//		/// Recompute validity vectors of all nodes.
//		/// </summary>
//		/// <param name="layout"></param>
//		public void RecomputeValidityVectors(TLayout layout)
//		{
//			foreach (var vertex in layout.Graph.Vertices)
//			{
//				if (!layout.GetConfiguration(vertex, out var configuration))
//					continue;

//				// TODO: could it be faster?
//				var neighbours = layout.Graph.GetNeighbours(vertex).ToList();

//				var validityVector = SimpleBitVector32.StartWithOnes(neighbours.Count);

//				for (var i = 0; i < neighbours.Count; i++)
//				{
//					var neighbour = neighbours[i];

//					// Non-existent neighbour is the same thing as a valid neighbour
//					if (!layout.GetConfiguration(neighbour, out var nc))
//					{
//						validityVector[i] = false;
//						continue;
//					}

//					var isValid = configurationSpaces.HaveValidPosition(configuration, nc);
//					validityVector[i] = !isValid;
//				}

//				layout.SetConfiguration(vertex, configuration.SetValidityVector(validityVector));
//			}
//		}

//		/// <summary>
//		/// Check if the layout is valid.
//		/// </summary>
//		/// <remarks>
//		/// These two conditions must hold:
//		/// 1. no two rooms can overlap
//		/// 2. all pairs of neigbours must be in a valid position with respect to each other
//		/// 
//		/// !! Validity vectors must be up-to-date !!
//		/// </remarks>
//		/// <param name="layout"></param>
//		/// <returns></returns>
//		public bool IsLayoutValid(TLayout layout)
//		{
//			var graph = layout.Graph;

//			// First check if all neighbouring vertices have pair-wise valid position
//			foreach (var vertex in graph.Vertices)
//			{
//				if (layout.GetConfiguration(vertex, out var configuration))
//				{
//					if (configuration.ValidityVector.Data != 0)
//						return false;
//				}
//			}

//			// Then check if any non-neighbouring vertices overlap
//			// TODO: is it okay to call all these ToList() ?
//			var vertices = graph.Vertices.ToList();
//			for (var i = 0; i < vertices.Count; i++)
//			{
//				var v1 = vertices[i];

//				if (!layout.GetConfiguration(v1, out var c1))
//					continue;

//				var neighbours = graph.GetNeighbours(v1).ToList();

//				for (var j = i + 1; j < vertices.Count; j++)
//				{
//					var v2 = vertices[j];

//					if (!layout.GetConfiguration(v2, out var c2))
//						continue;

//					if (neighbours.Contains(v2))
//						continue;

//					if (polygonOverlap.DoOverlap(c1.Shape, c1.Position, c2.Shape, c2.Position))
//						return false;
//				}
//			}

//			return true;
//		}

//		public void RecomputeEnergy(TLayout layout)
//		{
			
//		}

//		/// <summary>
//		/// Compute an energy of given node.
//		/// </summary>
//		/// <param name="layout"></param>
//		/// <param name="node"></param>
//		/// <param name="configuration"></param>
//		/// <returns>Zero if the configuration is valid and a positive number otherwise.</returns>
//		public float GetEnergy(TLayout layout, TNode node, TConfiguration configuration)
//		{
//			var intersection = 0;
//			var distance = 0;
//			var neighbours = layout.Graph.GetNeighbours(node).ToList();

//			foreach (var vertex in layout.Graph.Vertices)
//			{
//				if (!layout.GetConfiguration(vertex, out var c))
//					continue;

//				var area = polygonOverlap.OverlapArea(configuration.Shape, configuration.Position, c.Shape, c.Position);

//				if (area != 0)
//				{
//					intersection += area;
//				} else if (neighbours.Contains(vertex))
//				{
//					if (!configurationSpaces.HaveValidPosition(configuration, c))
//					{
//						// TODO: this is not really accurate when there are more sophisticated door positions (as smaller distance is not always better)
//						distance += IntVector2.ManhattanDistance(configuration.Shape.BoundingRectangle.Center + configuration.Position,
//							c.Shape.BoundingRectangle.Center + c.Position);
//					}
//				}
//			}

//			return (float)(Math.Pow(Math.E, intersection / EnergySigma) * Math.Pow(Math.E, distance / EnergySigma) - 1);
//		}

//		public void InjectRandomGenerator(Random random)
//		{
//			this.random = random;
//			configurationSpaces.InjectRandomGenerator(random);
//		}
//	}
//}
 