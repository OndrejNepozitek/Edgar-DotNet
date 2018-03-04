namespace GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common;
	using DataStructures.Graphs;

	public class GraphDecomposer<TNode> : IGraphDecomposer<TNode>
	{
		public List<List<TNode>> GetFaces(IGraph<TNode> graph)
		{
			var usedVerticesCount = 0;
			var mapping = new Dictionary<int, TNode>();
			var edges = new List<Tuple<int, int>>();

			foreach (var vertex in graph.Vertices)
			{
				mapping.Add(usedVerticesCount, vertex);
				usedVerticesCount++;
			}

			foreach (var edge in graph.Edges)
			{
				var v1 = mapping.FirstOrDefault(x => x.Value.Equals(edge.From)).Key;
				var v2 = mapping.FirstOrDefault(x => x.Value.Equals(edge.To)).Key;

				edges.Add(new Tuple<int, int>(v1, v2));
			}

			var facesRaw = GetFaces(graph.VerticesCount, edges.ToList());
			var faces = new List<List<TNode>>();

			if (facesRaw == null)
			{
				throw new InvalidOperationException("Graph is not planar");
			}

			foreach (var faceRaw in facesRaw)
			{
				var face = new List<TNode>();

				foreach (var vertex in faceRaw.Distinct())
				{
					face.Add(mapping[vertex]);
				}

				faces.Add(face);
			}

			return faces;
		}

		private int[][] GetFaces(int verticesCount, List<Tuple<int, int>> edges)
		{
			var edgesSerialized = new int[edges.Count * 2];

			for (var i = 0; i < edges.Count; i++)
			{
				var edge = edges[i];
				edgesSerialized[2 * i] = edge.Item1;
				edgesSerialized[2 * i + 1] = edge.Item2;
			}

			var faces = new int[2 * edges.Count];
			var facesBorders = new int[2 * edges.Count];

			var success = BoostWrapper.GetFaces(edgesSerialized, edgesSerialized.Length, verticesCount, faces, facesBorders, out var facesCount);

			if (!success)
			{
				return null;
			}

			var result = new int[facesCount][];
			var counter = 0;
			for (var i = 0; i < facesCount; i++)
			{
				result[i] = new int[facesBorders[i]];

				for (var j = 0; j < facesBorders[i]; j++)
				{
					result[i][j] = faces[counter++];
				}
			}

			return result;
		}
	}
}