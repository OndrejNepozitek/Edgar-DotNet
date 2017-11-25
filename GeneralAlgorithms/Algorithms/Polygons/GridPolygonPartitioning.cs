namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Cryptography.X509Certificates;
	using Advanced.Algorithms.DataStructures.Graph.AdjacencyList;
	using Advanced.Algorithms.GraphAlgorithms.Matching;
	using DataStructures.Common;
	using DataStructures.Polygons;
	using Graphs;
	using RangeTree;

	public class GridPolygonPartitioning
	{
		private readonly GridPolygonUtils utils = new GridPolygonUtils();

		public List<GridRectangle> GetRectangles(GridPolygon polygon)
		{
			var normalized = utils.NormalizePolygon(polygon);

			if (!normalized.Equals(polygon))
			{
				throw new InvalidOperationException("Polygon must be normalized");
			}

			var points = normalized.GetPoints();
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

				// TODO: is this right?
				from.Next = to;
				to.Prev = from;
			}

			var horizontalTree = new RangeTree<int, Segment>(horizontalSegments, new SegmentComparer());
			var verticalTree = new RangeTree<int, Segment>(verticalSegments, new SegmentComparer());

			var horizontalDiagonals = GetDiagonals(vertices, verticalTree, false);
			var verticalDiagonals = GetDiagonals(vertices, horizontalTree, true);

			if (horizontalDiagonals.Count != 0)
			{
				var splitters = FindSplitters(horizontalDiagonals, verticalDiagonals);

				foreach (var splitter in splitters)
				{
					SplitSegment(splitter);
				}
			}

			SplitConvave(vertices);

			var rectangles = FindRegions(vertices);

			return rectangles;
		}

		private List<GridRectangle> FindRegions(List<Vertex> vertices)
		{
			var n = vertices.Count;
			foreach (var v in vertices)
			{
				v.Visited = false;
			}

			var rectangles = new List<GridRectangle>();

			foreach (var vertex in vertices)
			{
				if (vertex.Visited) continue;
				var v = vertex;

				var backups = new List<Vertex>();

				var minx = int.MaxValue;
				var miny = int.MaxValue;
				var maxx = int.MinValue;
				var maxy = int.MinValue;

				while (!v.Visited)
				{
					backups.Add(v.BackupNext);
					backups.Add(v.BackupPrev);

					minx = Math.Min(v.Point.X, minx);
					miny = Math.Min(v.Point.Y, miny);
					maxx = Math.Max(v.Point.X, maxx);
					maxy = Math.Max(v.Point.Y, maxy);

					v.Visited = true;
					v = v.Next;
				}

				if (minx == maxx || miny == maxy)
				{
					foreach (var b in backups)
					{
						v = b;

						if (v == null) continue;

						minx = Math.Min(v.Point.X, minx);
						miny = Math.Min(v.Point.Y, miny);
						maxx = Math.Max(v.Point.X, maxx);
						maxy = Math.Max(v.Point.Y, maxy);
					}
				}

				rectangles.Add(new GridRectangle(new IntVector2(minx, miny), new IntVector2(maxx, maxy)));
			}

			return rectangles;
		}

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
				var splitA = new Vertex(new IntVector2(closestDistance, v.Point.Y), 0, false);
				var splitB = new Vertex(new IntVector2(closestDistance, v.Point.Y), 0, false);

				//Clear concavity flag
				v.Concave = false;

				//Split vertices
				// TODO: maybe make backup
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
				// TODO: maybe make backup
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

			na.SaveBackup();
			pa.SaveBackup();

			//Fix concavity
			a.Concave = false;
			b.Concave = false;

			//Compute orientation
			var ao = pa.GetCoord(segment.Horizontal) == a.GetCoord(segment.Horizontal);
			var bo = pb.GetCoord(segment.Horizontal) == b.GetCoord(segment.Horizontal);

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
			concave.Sort((x, y) => x.CompareTo(y, horizontal)); // TODO: test
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

			// TODO: the graph must be built in such a way that edges go always from first partition to the second one
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

		private List<int> Complement(List<int> list, int n)
		{
			var result = new List<int>();
			var a = 0;
			var b = 0;
			var sorted = list.OrderBy(x => x).ToList();
			for (var i = 0; i < n; ++i)
			{
				if (a < list.Count && sorted[a] == i)
				{
					a += 1;

				}
				else
				{
					result.Add(i);

				}
			}

			return result;
		}

		public Tuple<List<int>, List<int>> BipartiteVertexCover(int left, int right, List<Tuple<int, int>> edges)
		{
			var graph = new Graph<int>();
			for (var i = 0; i < left + right; i++)
			{
				graph.AddVertex(i);
			}
			foreach (var e in edges)
			{
				graph.AddEdge(e.Item1, e.Item2);
			}

			var matching = new Graphs.HopcroftKarpMatching<int>(null);
			var result = matching.GetMaxBiPartiteMatching(graph);

			var matchV = new int?[left + right];
			var matchU = new int?[left + right];

			foreach (var e in result)
			{
				matchV[e.Source] = e.Target;
				matchU[e.Target] = e.Source;
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

		private void Alternate(int u, Graph<int> graph, bool[] visitU, bool[] visitV, int?[] matchV)
		{
			visitU[u] = true;
			foreach (var v in graph.FindVertex(u).Edges)
			{
				if (!visitV[v.Value])
				{
					visitV[v.Value] = true;

					if (!matchV[v.Value].HasValue) throw new InvalidOperationException();

					Alternate(matchV[v.Value].Value, graph, visitU, visitV, matchV);
				}
			}
		}

		private List<Tuple<Segment, Segment>> FindCrossings(List<Segment> horizontalDiagonals, List<Segment> verticalDiagonals)
		{
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
					Range = new Range<int>(Math.Min(from.Point.X, to.Point.X), Math.Max(from.Point.X, to.Point.X));
				}
				else
				{
					Range = new Range<int>(Math.Min(from.Point.Y, to.Point.Y), Math.Max(from.Point.Y, to.Point.Y));
				}
			}

			public override string ToString()
			{
				return $"{From} -> {To}";
			}
		}

		private class Vertex
		{
			public IntVector2 Point;
			public int Index;
			public bool Concave;
			public Vertex Next;
			public Vertex Prev;
			public bool Visited;
			public int Number;
			public Vertex BackupNext;
			public Vertex BackupPrev;

			public Vertex(IntVector2 point, int index, bool concave)
			{
				Point = point;
				Index = index;
				Concave = concave;
			}

			public void SaveBackup()
			{
				BackupNext = Next;
				BackupPrev = Prev;
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