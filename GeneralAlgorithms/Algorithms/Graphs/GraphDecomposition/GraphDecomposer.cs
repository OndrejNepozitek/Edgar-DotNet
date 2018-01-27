namespace GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using CppCliWrapper;
	using DataStructures.Graphs;

	public class GraphDecomposer<TNode> : IGraphDecomposer<TNode>
		where TNode : IEquatable<TNode>
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

			var facesRaw = BoostWrapper.GetFaces(graph.VerticesCount, edges.ToArray());
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
	}
}