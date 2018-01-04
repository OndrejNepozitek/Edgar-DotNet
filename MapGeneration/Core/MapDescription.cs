namespace MapGeneration.Core
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Graphs;

	public class MapDescription<TNode>
	{
		private readonly List<RoomContainer> roomShapes = new List<RoomContainer>();
		private readonly Dictionary<TNode, List<RoomContainer>> roomShapesForNodes = new Dictionary<TNode, List<RoomContainer>>();
		private readonly List<Tuple<TNode, TNode>> passages = new List<Tuple<TNode, TNode>>();

		public ReadOnlyCollection<RoomContainer> RoomShapes => roomShapes.AsReadOnly();

		public ReadOnlyDictionary<TNode, ReadOnlyCollection<RoomContainer>> RoomShapesForNodes =>
			new ReadOnlyDictionary<TNode, ReadOnlyCollection<RoomContainer>>(
				roomShapesForNodes.ToDictionary(x => x.Key, x => x.Value?.AsReadOnly())
			);

		public ReadOnlyCollection<Tuple<TNode, TNode>> Passages => passages.AsReadOnly();

		/// <summary>
		/// Add shapes that will be used for nodes that do not have specific shapes assigned.
		/// </summary>
		/// <param name="shapes"></param>
		/// <param name="rotate"></param>
		/// <param name="probability"></param>
		public void AddRoomShapes(List<RoomDescription> shapes, bool rotate = true, double probability = 1)
		{
			shapes.ForEach(x => AddRoomShapes(x, rotate, probability));
		}

		/// <summary>
		/// Add shape that will be used for nodes that do not have specific shapes assigned.
		/// </summary>
		/// <param name="shape"></param>
		/// <param name="rotate"></param>
		/// <param name="probability"></param>
		public void AddRoomShapes(RoomDescription shape, bool rotate = true, double probability = 1)
		{
			if (probability <= 0)
				throw new ArgumentException("Probability should be greater than zero", nameof(probability));

			if (roomShapes.Any(x => shape.Equals(x.RoomDescription)))
				throw new InvalidOperationException("Every RoomDescription can be added at most once");

			roomShapes.Add(new RoomContainer(shape, rotate, probability));
		}

		public void AddRoomShapes(TNode node, List<RoomDescription> shapes, bool rotate = true, double probability = 1)
		{
			shapes.ForEach(x => AddRoomShapes(node, x, rotate, probability));
		}

		public void AddRoomShapes(TNode node, RoomDescription shape, bool rotate = true, double probability = 1)
		{
			if (probability <= 0)
				throw new ArgumentException("Probability should be greater than zero", nameof(probability));

			if (!roomShapesForNodes.TryGetValue(node, out var roomShapesForNode))
				throw new InvalidOperationException("Room must be first added to add shapes");

			if (roomShapesForNode == null)
			{
				roomShapesForNode = new List<RoomContainer>();
				roomShapesForNodes[node] = roomShapesForNode;
			}

			if (roomShapesForNode.Any(x => shape.Equals(x.RoomDescription)))
				throw new InvalidOperationException("Every RoomDescription can be added at most once");

			roomShapesForNode.Add(new RoomContainer(shape, rotate, probability));
		}

		public void AddRoom(TNode node)
		{
			if (roomShapesForNodes.ContainsKey(node))
				throw new InvalidOperationException("Given node was already added");

			roomShapesForNodes.Add(node, null);
		}

		public void AddPassage(TNode node1, TNode node2)
		{
			var passage = Tuple.Create(node1, node2);

			if (passages.Any(x => x.Equals(passage)))
				throw new InvalidOperationException("Given passage was already added");

			passages.Add(passage);
		}

		// TODO: should it be compute every time or precompute once? What should happen if it were then modified?
		public FastGraph<TNode> GetGraph()
		{
			var vertices = roomShapesForNodes.Keys;
			var graph = new FastGraph<TNode>(vertices.Count);

			foreach (var vertex in vertices)
			{
				graph.AddVertex(vertex);
			}

			foreach (var passage in passages)
			{
				graph.AddEdge(passage.Item1, passage.Item2);
			}

			return graph;
		}

		public class RoomContainer
		{
			public RoomDescription RoomDescription { get; }

			public bool ShouldRotate { get; }

			public double Probability { get; }

			public RoomContainer(RoomDescription roomDescription, bool shouldRotate, double probability)
			{
				RoomDescription = roomDescription;
				ShouldRotate = shouldRotate;
				Probability = probability;
			}
		}
	}
}