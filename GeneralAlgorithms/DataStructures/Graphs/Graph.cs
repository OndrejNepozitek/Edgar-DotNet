namespace GeneralAlgorithms.DataStructures.Graphs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class Graph<TNode>
	{
		public int VerticesCount => vertices.Length;

		private int insertedVerticesCount;
		private TNode[] vertices;
		private bool[][] adjacencyMatrix;
		private List<int>[] adjacencyLists;

		public Graph(int verticesCount)
		{
			vertices = new TNode[verticesCount];
			adjacencyMatrix = new bool[verticesCount][];

			for (var i = 0; i < verticesCount; i++)
			{
				adjacencyMatrix[i] = new bool[verticesCount];
			}

			adjacencyLists = new List<int>[verticesCount];

			for (var i = 0; i < verticesCount; i++)
			{
				adjacencyLists[i] = new List<int>();
			}
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

			var fromNeighbours = GetNeighbours(fromNum);
			var toNeighbours = GetNeighbours(toNum);

			if (fromNeighbours.Contains(toNum) || toNeighbours.Contains(fromNum))
			{
				throw new InvalidOperationException("Edge already exists");
			}

			if (fromNeighbours.Count == 32 || toNeighbours.Count == 32)
			{
				throw new InvalidOperationException("Only vertices with up to 32 neighbours are allowed");
			}

			fromNeighbours.Add(toNum);
			toNeighbours.Add(fromNum);
		}

		// TODO: unsafe method as the caller can alter the collection, but fast
		public List<int> GetNeighbours(int vertex)
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
		public int NeigbourIndex(int original, int neigbour)
		{
			var index = 0;

			foreach (var n in GetNeighbours(neigbour))
			{
				if (n == original)
				{
					return index;
				}

				index++;
			}

			throw new InvalidOperationException();
		}
	}
}