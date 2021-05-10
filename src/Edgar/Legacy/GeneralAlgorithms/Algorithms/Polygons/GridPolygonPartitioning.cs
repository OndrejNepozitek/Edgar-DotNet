using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.Graphs;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Graphs;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using RangeTree;

namespace Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons
{
    /// <remarks>
	/// Adapted from https://github.com/mikolalysenko/rectangle-decomposition
	/// </remarks>>
	public class GridPolygonPartitioning : IPolygonPartitioning
	{
		private readonly HopcroftKarp<int> hopcroftKarp = new HopcroftKarp<int>();

		/// <inheritdoc />
		public List<RectangleGrid2D> GetPartitions(PolygonGrid2D polygon)
		{
			IList<Vector2Int> points = polygon.GetPoints();

			var vertices = new List<Vertex>();

			var prev = points[points.Count - 3];
			var curr = points[points.Count - 2];
			var next = points[points.Count - 1];

			// Process vertices
			for (var i = 0; i < points.Count; i++)
			{
				prev = curr;
				curr = next;
				next = points[i];
				bool concave;

				if (prev.X == curr.X)
				{
					if (curr.X == next.X)
					{
						throw new InvalidOperationException("Should not happen as polygons must be normalized");
					}

					var dir0 = prev.Y < curr.Y;
					var dir1 = curr.X > next.X;

					concave = dir0 == dir1;
				}
				else
				{
					if (next.Y == curr.Y)
					{
						throw new InvalidOperationException("Should not happen as polygons must be normalized");

					}

					var dir0 = prev.X < curr.X;
					var dir1 = curr.Y > next.Y;

					concave = dir0 != dir1;
				}

				var vertex = new Vertex(curr, i - 1, concave);
				vertices.Add(vertex);
			}

			// Build interval trees for segments
			var horizontalSegments = new List<Segment>();
			var verticalSegments = new List<Segment>();

			for (var i = 0; i < vertices.Count; i++)
			{
				var from = vertices[i];
				var to = vertices[(i + 1) % vertices.Count];

				if (from.Point.X == to.Point.X)
				{
					verticalSegments.Add(new Segment(from, to, false));
				}
				else
				{
					horizontalSegments.Add(new Segment(from, to, true));
				}

				from.Next = to;
				to.Prev = from;
			}
			
			var horizontalTree = new RangeTree<int, Segment>(horizontalSegments, new SegmentComparer());
			var verticalTree = new RangeTree<int, Segment>(verticalSegments, new SegmentComparer());

			//Find horizontal and vertical diagonals
			var horizontalDiagonals = GetDiagonals(vertices, verticalTree, false);
			var verticalDiagonals = GetDiagonals(vertices, horizontalTree, true);

			//Find all splitting edges
			var splitters = FindSplitters(horizontalDiagonals, verticalDiagonals);

			foreach (var splitter in splitters)
			{
				SplitSegment(splitter);
			}

			//Split all concave vertices
			SplitConvave(vertices);

			var rectangles = FindRegions(vertices);

			return rectangles;
		}

		/// <summary>
		/// Construct the partitioning.
		/// </summary>
		/// <param name="vertices"></param>
		/// <returns></returns>
		private List<RectangleGrid2D> FindRegions(List<Vertex> vertices)
		{
			var n = vertices.Count;
			foreach (var v in vertices)
			{
				v.Visited = false;
			}

			var rectangles = new List<RectangleGrid2D>();

			foreach (var vertex in vertices)
			{
				if (vertex.Visited) continue;
				var v = vertex;

				var minx = int.MaxValue;
				var miny = int.MaxValue;
				var maxx = int.MinValue;
				var maxy = int.MinValue;

				while (!v.Visited)
				{
					minx = System.Math.Min(v.Point.X, minx);
					miny = System.Math.Min(v.Point.Y, miny);
					maxx = System.Math.Max(v.Point.X, maxx);
					maxy = System.Math.Max(v.Point.Y, maxy);

					v.Visited = true;
					v = v.Next;
				}

				if (minx == maxx || miny == maxy)
					throw new InvalidOperationException("Degenerated rectangle. Must not happen.");

				rectangles.Add(new RectangleGrid2D(new Vector2Int(minx, miny), new Vector2Int(maxx, maxy)));
			}

			return rectangles;
		}

		/// <summary>
		/// Split all concave vertices.
		/// </summary>
		/// <param name="vertices"></param>
		private void SplitConvave(List<Vertex> vertices)
		{
			var leftSegments = new List<Segment>();
			var rightSegments = new List<Segment>();

			foreach (var v in vertices)
			{
				if (v.Next.Point.X == v.Point.X) // TODO: prob his mistake
				{
					if (v.Next.Point.Y > v.Point.Y)
					{
						leftSegments.Add(new Segment(v, v.Next, false));
					}
					else
					{
						rightSegments.Add(new Segment(v, v.Next, false));
					}
				}
			}

			var leftTree = new RangeTree<int, Segment>(leftSegments, new SegmentComparer());
			var rightTree = new RangeTree<int, Segment>(rightSegments, new SegmentComparer());

			var i = 0;
			while (i < vertices.Count)
			{
				var v = vertices[i];
				i++;

				if (!v.Concave) continue;

				var y = v.Point.Y;
				bool dir;

				if (v.Prev.Point.X == v.Point.X)
				{
					dir = v.Prev.Point.Y < y;

				}
				else
				{
					dir = v.Next.Point.Y > y;

				}

				var direction = dir ? 1 : -1;

				Segment closestSegment = null;
				var closestDistance = direction == 1 ? int.MaxValue : int.MinValue;

				if (direction > 0) // TODO: prob his mistake
				{
					foreach (var s in rightTree.Query(v.Point.Y))
					{
						var x = s.From.Point.X;

						if (x > v.Point.X && x < closestDistance)
						{
							closestDistance = x;
							closestSegment = s;
						}
					}
				}
				else
				{
					foreach (var s in leftTree.Query(v.Point.Y))
					{
						var x = s.From.Point.X;

						if (x < v.Point.X && x > closestDistance)
						{
							closestDistance = x;
							closestSegment = s;
						}
					}
				}

				//Create two splitting vertices
				var splitA = new Vertex(new Vector2Int(closestDistance, v.Point.Y), 0, false);
				var splitB = new Vertex(new Vector2Int(closestDistance, v.Point.Y), 0, false);

				//Clear concavity flag
				v.Concave = false;

				//Split vertices
				splitA.Prev = closestSegment.From;
				closestSegment.From.Next = splitA;

				splitB.Next = closestSegment.To;
				closestSegment.To.Prev = splitB;

				var tree = leftTree;
				if (direction > 0)
				{
					tree = rightTree;
				}

				tree.Remove(closestSegment);
				tree.Add(new Segment(closestSegment.From, splitA, false));
				tree.Add(new Segment(splitB, closestSegment.To, false));

				vertices.Add(splitA);
				vertices.Add(splitB);

				//Cut v, 2 different cases
				if (v.Prev.Point.X == v.Point.X)
				{
					// Case 1
					//             ^
					//             |
					// --->*+++++++X
					//     |       |
					//     V       |
					splitA.Next = v.Next;
					splitB.Prev = v;
				}
				else
				{
					// Case 2
					//     |       ^
					//     V       |
					// <---*+++++++X
					//             |
					//             |
					splitA.Next = v;
					splitB.Prev = v.Prev;

				}

				//Fix up links
				splitA.Next.Prev = splitA;
				splitB.Prev.Next = splitB;
			}
		}

		private void SplitSegment(Segment segment)
		{
			//Store references
			var a = segment.From;
			var b = segment.To;
			var pa = a.Prev;
			var na = a.Next;
			var pb = b.Prev;
			var nb = b.Next;

			//Fix concavity
			a.Concave = false;
			b.Concave = false;

			//Compute orientation
			var ao = pa.GetCoord(!segment.Horizontal) == a.GetCoord(!segment.Horizontal);
			var bo = pb.GetCoord(!segment.Horizontal) == b.GetCoord(!segment.Horizontal);

			if (ao && bo)
			{
				//Case 1:
				//            ^
				//            |
				//  --->A+++++B<---
				//      |
				//      V
				a.Prev = pb;
				pb.Next = a;
				b.Prev = pa;
				pa.Next = b;
			}
			else if (ao && !bo)
			{
				//Case 2:
				//      ^     |
				//      |     V
				//  --->A+++++B--->
				//            
				//            
				a.Prev = b;
				b.Next = a;
				pa.Next = nb;
				nb.Prev = pa;
			}
			else if (!ao && bo)
			{
				//Case 3:
				//            
				//            
				//  <---A+++++B<---
				//      ^     |
				//      |     V
				a.Next = b;
				b.Prev = a;
				na.Prev = pb;
				pb.Next = na;

			}
			else if (!ao && !bo)
			{
				//Case 3:
				//            |
				//            V
				//  <---A+++++B--->
				//      ^     
				//      |     
				a.Next = nb;
				nb.Prev = a;
				b.Next = na;
				na.Prev = b;
			}
		}

		private List<Segment> GetDiagonals(List<Vertex> vertices, RangeTree<int, Segment> tree, bool horizontal)
		{
			var concave = vertices.Where(x => x.Concave).ToList();
			concave.Sort((x, y) => x.CompareTo(y, horizontal));
			var diagonals = new List<Segment>();

			for (var i = 0; i < concave.Count - 1; i++)
			{
				var from = concave[i];
				var to = concave[i + 1];

				if (from.GetCoord(horizontal) == to.GetCoord(horizontal))
				{
					// We do not want adjacent vertices - they cannot make a diagonal
					var diff = (from.Index - to.Index + vertices.Count) % vertices.Count;
					if (diff == 1 || diff == vertices.Count - 1) continue;

					if (IsDiagonal(from, to, tree, horizontal))
					{
						diagonals.Add(new Segment(from, to, !horizontal));
					}
				}
			}

			return diagonals;
		}

		private List<Segment> FindSplitters(List<Segment> horizontalDiagonals, List<Segment> verticalDiagonals)
		{
			var crossings = FindCrossings(horizontalDiagonals, verticalDiagonals);

			var c = 0;
			foreach (var diag in horizontalDiagonals)
			{
				diag.Number = c;
				c++;
			}
			foreach (var diag in verticalDiagonals)
			{
				diag.Number = c;
				c++;
			}

			// CHECK: the graph must be built in such a way that edges go always from first partition to the second one
			var selected = BipartiteIndependentSet(horizontalDiagonals.Count, verticalDiagonals.Count,
				crossings.Select(x => new Tuple<int, int>(x.Item1.Number, x.Item2.Number)).ToList());

			var result = new List<Segment>();

			foreach (var s in selected)
			{
				if (s < horizontalDiagonals.Count)
				{
					result.Add(horizontalDiagonals[s]);
				}
				else
				{
					result.Add(verticalDiagonals[s - horizontalDiagonals.Count]);
				}
			}

			return result;
		}

		/// <summary>
		/// Compute a maximum independent set of a given bipartite graph.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="edges"></param>
		/// <returns></returns>
		public List<int> BipartiteIndependentSet(int left, int right, List<Tuple<int, int>> edges)
		{
			var cover = BipartiteVertexCover(left, right, edges);
			var set = new List<int>();

			for (var i = 0; i < left; i++)
			{
				if (!cover.Item1.Contains(i))
				{
					set.Add(i);
				}
			}

			for (var i = left; i < left + right; i++)
			{
				if (!cover.Item2.Contains(i))
				{
					set.Add(i);
				}
			}

			return set;
		}

		/// <summary>
		/// Compute a minimum vertex cover of a given bipartite graph.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="edges"></param>
		/// <returns></returns>
		public Tuple<List<int>, List<int>> BipartiteVertexCover(int left, int right, List<Tuple<int, int>> edges)
		{
			var graph = new UndirectedAdjacencyListGraph<int>();
			for (var i = 0; i < left + right; i++)
			{
				graph.AddVertex(i);
			}
			foreach (var e in edges)
			{
				graph.AddEdge(e.Item1, e.Item2);
			}

			var matching = hopcroftKarp.GetMaximumMatching(graph);

			var matchV = new int?[left + right];
			var matchU = new int?[left + right];

			foreach (var e in matching)
			{
				matchV[e.From] = e.To;
				matchU[e.To] = e.From;
			}

			var visitU = new bool[left + right];
			var visitV = new bool[left + right];

			for (var i = left; i < left + right; i++)
			{
				if (!matchU[i].HasValue)
				{
					Alternate(i, graph, visitU, visitV, matchV);
				}
			}

			var leftNodes = new List<int>();
			for (var i = 0; i < right; i++)
			{
				if (visitV[i])
				{
					leftNodes.Add(i);
				}
			}

			var rightNodes = new List<int>();
			for (var i = left; i < left + right; i++)
			{
				if (!visitU[i])
				{
					rightNodes.Add(i);
				}
			}

			return new Tuple<List<int>, List<int>>(leftNodes, rightNodes);
		}

		private void Alternate(int u, IGraph<int> graph, bool[] visitU, bool[] visitV, int?[] matchV)
		{
			visitU[u] = true;
			foreach (var v in graph.GetNeighbors(u))
			{
				if (!visitV[v])
				{
					visitV[v] = true;

					if (!matchV[v].HasValue) throw new InvalidOperationException();

					Alternate(matchV[v].Value, graph, visitU, visitV, matchV);
				}
			}
		}

		private List<Tuple<Segment, Segment>> FindCrossings(List<Segment> horizontalDiagonals, List<Segment> verticalDiagonals)
		{
			if (horizontalDiagonals.Count == 0 || verticalDiagonals.Count == 0)
			{
				return new List<Tuple<Segment, Segment>>();
			}

			var horizontalTree = new RangeTree<int, Segment>(horizontalDiagonals, new SegmentComparer());
			var crosssings = new List<Tuple<Segment, Segment>>();

			foreach (var v in verticalDiagonals)
			{
				var ax = v.From.Point.Y;
				var bx = v.To.Point.Y;

				foreach (var h in horizontalTree.Query(v.From.Point.X))
				{
					var start = h.From.Point.Y;

					if ((ax <= start && start <= bx) || (bx <= start && start <= ax))
					{
						crosssings.Add(new Tuple<Segment, Segment>(h, v));
					}
				}
			}

			return crosssings;
		}

		private bool IsDiagonal(Vertex from, Vertex to, RangeTree<int, Segment> tree, bool horizontal)
		{
			var ax = from.GetCoord(!horizontal);
			var bx = to.GetCoord(!horizontal);

			foreach (var segment in tree.Query(from.GetCoord(horizontal)))
			{
				var start = segment.From.GetCoord(!horizontal);

				if ((ax < start && start < bx) || (bx < start && start < ax))
				{
					return false;
				}
			}

			return true;
		}

		private class SegmentComparer : IComparer<Segment>
		{
			public int Compare(Segment x, Segment y)
			{
				return x.Range.CompareTo(y.Range);
			}
		}

		private class Segment : IRangeProvider<int>
		{
			public readonly Vertex From;
			public readonly Vertex To;
			public readonly bool Horizontal;
			public int Number;

			public Range<int> Range { get; }

			public Segment(Vertex from, Vertex to, bool horizontal)
			{
				From = from;
				To = to;
				Horizontal = horizontal;

				if (horizontal)
				{
					Range = new Range<int>(System.Math.Min(from.Point.X, to.Point.X), System.Math.Max(from.Point.X, to.Point.X));
				}
				else
				{
					Range = new Range<int>(System.Math.Min(from.Point.Y, to.Point.Y), System.Math.Max(from.Point.Y, to.Point.Y));
				}
			}

			public override string ToString()
			{
				return $"{From} -> {To}";
			}
		}

		private class Vertex
		{
			public Vector2Int Point;
			public int Index;
			public bool Concave;

			public bool Visited;
			public int Number;

			private Vertex next;
			private Vertex prev;

			public Vertex Next;

			public Vertex Prev;

			public Vertex(Vector2Int point, int index, bool concave)
			{
				Point = point;
				Index = index;
				Concave = concave;
			}

			public int GetCoord(bool isX = true)
			{
				return isX ? Point.X : Point.Y;
			}

			public int CompareTo(Vertex other, bool xFirst)
			{
				if (xFirst)
				{
					var diff = Point.X - other.Point.X;

					if (diff != 0)
					{
						return diff;
					}

					return Point.Y - other.Point.Y;
				}
				else
				{
					var diff = Point.Y - other.Point.Y;

					if (diff != 0)
					{
						return diff;
					}

					return Point.X - other.Point.X;
				}
			}

			public override string ToString()
			{
				return $"[{Point.X},{Point.Y}]";
			}
		}
	}
}