namespace GeneralAlgorithms.Algorithms.Graphs
{
	using System;
	using System.Collections.Generic;
	using DataStructures.Graphs;

	public class HopcroftKarp<T>
	{
		private readonly BipartiteCheck<T> bipartiteCheck = new BipartiteCheck<T>();
		private IntGraph<T> convertedGraph;

		private const int Nil = -1;
		private const int Inf = int.MaxValue;

		private int sizeU;
		private int sizeV;

		private int[] pairU;
		private int[] pairV;
		private int[] dist;

		public List<Edge<T>> GetMaximumMatching(IGraph<T> graph)
		{
			if (!bipartiteCheck.IsBipartite(graph, out var parts))
				throw new InvalidOperationException("Given graph is not bipartite");

			convertedGraph = ConvertGraph(graph, parts.Item1, parts.Item2);

			sizeU = parts.Item1.Count;
			sizeV = parts.Item2.Count;

			pairU = new int[sizeU + 1];
			pairV = new int[sizeV + 1];
			dist = new int[sizeU + 1];

			for (var i = 0; i < pairU.Length; i++)
			{
				SetPairU(i, Nil);
			}

			for (var i = 0; i < pairV.Length; i++)
			{
				SetPairV(i + sizeU, Nil);
			}

			var result = 0;

			while (BFS())
			{
				for (var u = 0; u < sizeU; u++)
				{
					if (GetPairU(u) == Nil && DFS(u))
					{
						result++;
					}
				}
			}

			var matching = new List<Edge<T>>();

			for (var u = 0; u < sizeU; u++)
			{
				if (GetPairU(u) != Nil)
				{
					matching.Add(new Edge<T>(convertedGraph.GetOriginalVertex(u), convertedGraph.GetOriginalVertex(pairU[u])));
				}
			}

			return matching;
		}

		private bool BFS()
		{
			var queue = new Queue<int>();

			// First layer of vertices (set distance as 0)
			for (var u = 0; u < sizeU; u++)
			{
				// If this is a free vertex, add it to queue
				if (GetPairU(u) == Nil)
				{
					// u is not matched
					SetDistance(u, 0);
					queue.Enqueue(u);
				}
				// Else set distance as infinite so that this vertex
				// is considered next time
				else
				{
					SetDistance(u, Inf);
				}
			}

			// Initialize distance to NIL as infinite
			SetDistance(Nil, Inf);

			// Queue is going to contain vertices of left side only. 
			while (queue.Count != 0)
			{
				// Dequeue a vertex
				var u = queue.Dequeue();

				// If this node is not NIL and can provide a shorter path to NIL
				if (GetDistance(u) < GetDistance(Nil))
				{
					// Get all adjacent vertices of the dequeued vertex u
					foreach (var v in GetNeighbours(u))
					{
						// If pair of v is not considered so far
						// (v, pairV[V]) is not yet explored edge.
						if (GetDistance(GetPairV(v)) == Inf)
						{
							// Consider the pair and add it to queue
							SetDistance(GetPairV(v), GetDistance(u) + 1);
							queue.Enqueue(GetPairV(v));
						}
					}
				}
			}

			// If we could come back to NIL using alternating path of distinct
			// vertices then there is an augmenting path
			return GetDistance(Nil) != Inf;
		}

		private IEnumerable<int> GetNeighbours(int vertex)
		{
			if (vertex == Nil)
				return convertedGraph.Vertices;

			return convertedGraph.GetNeighbours(vertex);
		}

		private bool DFS(int u)
		{
			if (u != Nil)
			{
				foreach (var v in GetNeighbours(u))
				{
					// Follow the distances set by BFS
					if (GetDistance(GetPairV(v)) == GetDistance(u) + 1)
					{
						// If dfs for pair of v also returns true
						if (DFS(GetPairV(v)))
						{
							SetPairV(v, u);
							SetPairU(u, v);
							return true;
						}
					}
				}

				// If there is no augmenting path beginning with u.
				SetDistance(u, Inf);
				return false;
			}

			return true;
		}

		private IntGraph<T> ConvertGraph(IGraph<T> originalGraph, List<T> left, List<T> right)
		{
			var graph = new IntGraph<T>(() => new UndirectedAdjacencyListGraph<int>());

			foreach (var vertex in left)
			{
				graph.AddVertex(vertex);
			}

			foreach (var vertex in right)
			{
				graph.AddVertex(vertex);
			}

			foreach (var edge in originalGraph.Edges)
			{
				graph.AddEdge(edge.From, edge.To);
			}

			return graph;
		}

		#region Getters and setters

		private int GetDistance(int vertex)
		{
			return vertex == Nil ? dist[sizeU] : dist[vertex];
		}

		private void SetDistance(int vertex, int distance)
		{
			if (vertex == Nil)
			{
				dist[sizeU] = distance;
				return;
			}

			dist[vertex] = distance;
		}

		private int GetPairU(int vertex)
		{
			return vertex == Nil ? pairU[sizeU] : pairU[vertex];
		}

		private void SetPairU(int vertex, int pair)
		{
			if (vertex == Nil)
			{
				pairU[sizeU] = pair;
				return;
			}
				
			pairU[vertex] = pair;
		}

		private int GetPairV(int vertex)
		{
			return vertex == Nil ? pairV[sizeV] : pairV[vertex - sizeU];
		}

		private void SetPairV(int vertex, int pair)
		{
			if (vertex == Nil)
			{
				pairV[sizeV] = pair;
				return;
			}
				
			pairV[vertex - sizeU] = pair;
		}

		#endregion
	}
}