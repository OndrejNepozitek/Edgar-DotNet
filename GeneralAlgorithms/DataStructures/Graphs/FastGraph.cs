namespace GeneralAlgorithms.DataStructures.Graphs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	// TODO: is it worth having this class? 
	public class FastGraph<TNode> : IGraph<int>
	{
		public bool IsDirected { get; } = false;

		public IEnumerable<int> Vertices { get; }

		public IEnumerable<IEdge<int>> Edges => edges;

		public int VerticesCount => vertices.Length;

		private int insertedVerticesCount;
		private readonly TNode[] vertices;
		private readonly List<int>[] adjacencyLists;

		private readonly List<Edge<int>> edges = new List<Edge<int>>();

		public FastGraph(int verticesCount)
		{
			vertices = new TNode[verticesCount];

			adjacencyLists = new List<int>[verticesCount];

			for (var i = 0; i < verticesCount; i++)
			{
				adjacencyLists[i] = new List<int>();
			}

			Vertices = Enumerable.Range(0, verticesCount);
		}

		// TODO: unsafe method as the caller can alter the collection, but fast
		public IEnumerable<int> GetNeighbours(int vertex)
		{
			return GetNeighboursInternal(vertex);
		}

		public List<int> GetNeighboursInternal(int vertex)
		{
			return adjacencyLists[vertex];
		}

		public TNode GetVertex(int number)
		{
			return vertices[number];
		}

		// TODO: slow
		private int GetVertexNumber(TNode vertex)
		{
			for (var i = 0; i < VerticesCount; i++)
			{
				// TODO: is this right?
				if (vertices[i].Equals(vertex))
				{
					return i;
				}
			}

			throw new InvalidOperationException();
		}

		// TODO: should be replace with a lookup table
		public int GetNeighbourIndex(int original, int neighbour)
		{
			var index = 0;

			foreach (var n in GetNeighbours(original))
			{
				if (n == neighbour)
				{
					return index;
				}

				index++;
			}

			throw new InvalidOperationException();
		}

		public bool HasEdge(int from, int to)
		{
			foreach (var neighbour in GetNeighbours(from))
			{
				if (neighbour.Equals(to))
					return true;
			}

			return false;
		}

		public void AddVertex(TNode vertex)
		{
			if (insertedVerticesCount == VerticesCount)
			{
				throw new InvalidOperationException("Cannot add more than VerticesCount vertices");
			}

			vertices[insertedVerticesCount++] = vertex;
		}

		public void AddEdge(TNode from, TNode to)
		{
			if (!vertices.Contains(from) || !vertices.Contains(to))
			{
				throw new InvalidOperationException("At least one of the vertices does not exist");
			}

			var fromNum = GetVertexNumber(from);
			var toNum = GetVertexNumber(to);

			var fromNeighbours = GetNeighboursInternal(fromNum);
			var toNeighbours = GetNeighboursInternal(toNum);

			if (fromNeighbours.Contains(toNum) || toNeighbours.Contains(fromNum))
			{
				throw new InvalidOperationException("Edge already exists");
			}

			// TODO: is it needed here?
			//if (fromNeighbours.Count == 32 || toNeighbours.Count == 32)
			//{
			//	throw new InvalidOperationException("Only vertices with up to 32 neighbours are allowed");
			//}

			fromNeighbours.Add(toNum);
			toNeighbours.Add(fromNum);

			edges.Add(new Edge<int>(fromNum, toNum));
		}

		void IGraph<int>.AddVertex(int vertex)
		{
			throw new NotSupportedException();
		}

		void IGraph<int>.AddEdge(int from, int to)
		{
			throw new NotSupportedException();
		}
	}
}