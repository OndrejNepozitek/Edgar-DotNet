namespace MapGeneration.Core
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces.Core;
	using Interfaces.Core.MapDescription;

	public class MapDescription<TNode> : ICorridorMapDescription<int>
	{
		private readonly List<RoomContainer> roomShapes = new List<RoomContainer>();
		private readonly List<RoomContainer> corridorShapes = new List<RoomContainer>();
		private readonly List<List<RoomContainer>> roomShapesForNodes = new List<List<RoomContainer>>();
		private readonly List<Tuple<TNode, TNode>> passages = new List<Tuple<TNode, TNode>>();
		private readonly Dictionary<TNode, int> rooms = new Dictionary<TNode, int>();
		private bool isLocked = false;

		public bool IsWithCorridors { get; private set; }

		public List<int> CorridorsOffsets { get; private set; }

		public ReadOnlyCollection<RoomContainer> RoomShapes => roomShapes.AsReadOnly();

		public ReadOnlyCollection<RoomContainer> CorridorShapes => corridorShapes.AsReadOnly();

		public ReadOnlyCollection<ReadOnlyCollection<RoomContainer>> RoomShapesForNodes =>
			roomShapesForNodes.Select(x => x?.AsReadOnly()).ToList().AsReadOnly();

		/// <summary>
		/// Add shapes that will be used for nodes that do not have specific shapes assigned.
		/// </summary>
		/// <param name="shapes"></param>
		/// <param name="rotate"></param>
		/// <param name="probability"></param>
		/// <param name="normalizeProbabilities"></param>
		public void AddRoomShapes(List<RoomDescription> shapes, bool rotate = true, double probability = 1, bool normalizeProbabilities = true)
		{
			if (isLocked)
				throw new InvalidOperationException("MapDescription is locked");

			shapes.ForEach(x => AddRoomShapes(x, rotate, probability, normalizeProbabilities));
		}

		/// <summary>
		/// Add shape that will be used for nodes that do not have specific shapes assigned.
		/// </summary>
		/// <param name="shape"></param>
		/// <param name="rotate"></param>
		/// <param name="probability"></param>
		/// <param name="normalizeProbabilities"></param>
		public void AddRoomShapes(RoomDescription shape, bool rotate = true, double probability = 1, bool normalizeProbabilities = true)
		{
			if (isLocked)
				throw new InvalidOperationException("MapDescription is locked");

			if (probability <= 0)
				throw new ArgumentException("Probability should be greater than zero", nameof(probability));

			if (roomShapes.Any(x => shape.Equals(x.RoomDescription)))
				throw new InvalidOperationException("Every RoomDescription can be added at most once");

			roomShapes.Add(new RoomContainer(shape, rotate, probability, normalizeProbabilities));
		}

		public void AddCorridorShapes(RoomDescription shape, bool rotate = true, double probability = 1, bool normalizeProbabilities = true)
		{
			if (isLocked)
				throw new InvalidOperationException("MapDescription is locked");

			if (probability <= 0)
				throw new ArgumentException("Probability should be greater than zero", nameof(probability));

			if (roomShapes.Any(x => shape.Equals(x.RoomDescription)))
				throw new InvalidOperationException("Every RoomDescription can be added at most once");

			corridorShapes.Add(new RoomContainer(shape, rotate, probability, normalizeProbabilities));
		}

		public void AddCorridorShapes(List<RoomDescription> shapes, bool rotate = true, double probability = 1, bool normalizeProbabilities = true)
		{
			if (isLocked)
				throw new InvalidOperationException("MapDescription is locked");

			shapes.ForEach(x => AddCorridorShapes(x, rotate, probability, normalizeProbabilities));
		}

		public void AddRoomShapes(TNode node, List<RoomDescription> shapes, bool rotate = true, double probability = 1, bool normalizeProbabilities = true)
		{
			if (isLocked)
				throw new InvalidOperationException("MapDescription is locked");

			shapes.ForEach(x => AddRoomShapes(node, x, rotate, probability, normalizeProbabilities));
		}

		public void AddRoomShapes(TNode node, RoomDescription shape, bool rotate = true, double probability = 1, bool normalizeProbabilities = true)
		{
			if (isLocked)
				throw new InvalidOperationException("MapDescription is locked");

			if (probability <= 0)
				throw new ArgumentException("Probability should be greater than zero", nameof(probability));

			if (!rooms.TryGetValue(node, out var alias))
				throw new InvalidOperationException("Room must be first added to add shapes");

			var roomShapesForNode = roomShapesForNodes[alias];

			if (roomShapesForNode == null)
			{
				roomShapesForNode = new List<RoomContainer>();
				roomShapesForNodes[alias] = roomShapesForNode;
			}

			if (roomShapesForNode.Any(x => shape.Equals(x.RoomDescription)))
				throw new InvalidOperationException("Every RoomDescription can be added at most once");

			roomShapesForNode.Add(new RoomContainer(shape, rotate, probability, normalizeProbabilities));
		}

		public void AddRoom(TNode node)
		{
			if (isLocked)
				throw new InvalidOperationException("MapDescription is locked");

			if (rooms.ContainsKey(node))
				throw new InvalidOperationException("Given node was already added");

			rooms.Add(node, rooms.Count);
			roomShapesForNodes.Add(null);
		}

		public void AddPassage(TNode node1, TNode node2)
		{
			if (isLocked)
				throw new InvalidOperationException("MapDescription is locked");

			var passage = Tuple.Create(node1, node2);

			if (passages.Any(x => x.Equals(passage)))
				throw new InvalidOperationException("Given passage was already added");

			passages.Add(passage);
		}

		public IGraph<int> GetGraph()
		{
			isLocked = true;
			var vertices = Enumerable.Range(0, rooms.Count);
			var graph = new UndirectedAdjacencyListGraph<int>();
			var corridorCounter = rooms.Count;

			foreach (var vertex in vertices)
			{
				graph.AddVertex(vertex);
			}

			foreach (var passage in passages)
			{
				if (IsWithCorridors)
				{
					graph.AddVertex(corridorCounter);
					graph.AddEdge(rooms[passage.Item1], corridorCounter);
					graph.AddEdge(corridorCounter, rooms[passage.Item2]);
					corridorCounter++;
				}
				else
				{
					graph.AddEdge(rooms[passage.Item1], rooms[passage.Item2]);
				}
			}

			return graph;
		}

		public void SetWithCorridors(bool enable, List<int> offsets = null)
		{
			if (isLocked)
				throw new InvalidOperationException("MapDescription is locked");

			if (enable && (offsets == null || offsets.Count == 0))
				throw new ArgumentException("At least one offset must be set if corridors are enabled");

			IsWithCorridors = enable;
			CorridorsOffsets = offsets;
		}

		public bool IsCorridorRoom(int room)
		{
			return room >= rooms.Count;
		}

		public IGraph<int> GetGraphWithoutCorrridors()
		{
			// TODO: keep dry
			isLocked = true;
			var vertices = Enumerable.Range(0, rooms.Count);
			var graph = new UndirectedAdjacencyListGraph<int>();

			foreach (var vertex in vertices)
			{
				graph.AddVertex(vertex);
			}

			foreach (var passage in passages)
			{
				graph.AddEdge(rooms[passage.Item1], rooms[passage.Item2]);
			}

			return graph;
		}

		public class RoomContainer
		{
			public RoomDescription RoomDescription { get; }

			public bool ShouldRotate { get; }

			public double Probability { get; }

			public bool NormalizeProbabilities { get; }

			public RoomContainer(RoomDescription roomDescription, bool shouldRotate, double probability, bool normalizeProbabilities)
			{
				RoomDescription = roomDescription;
				ShouldRotate = shouldRotate;
				Probability = probability;
				NormalizeProbabilities = normalizeProbabilities;
			}
		}
	}
}