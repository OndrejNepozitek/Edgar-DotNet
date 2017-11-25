namespace GeneralAlgorithms.Algorithms.Graphs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Implements Hopcroft Karp algorithm for finding maximum matching for unweighted undirected bipartiate graphs
	/// https://en.wikipedia.org/wiki/Hopcroft%E2%80%93Karp_algorithm
	/// </summary>
	public static class HopcroftKarp
	{
		/// <summary>
		/// Finds maximum matching for unweighted bipartiate graph in O(E sqrt(v)).
		/// The Hopcroft Karp algorithm is as follows:
		/// 1.  Augmented path has end points as two unmatched vertices. 
		///     There may be 0 or more other vertices in the path besides source and destination however they must all be matched vertices.
		///     The edges in the path must alternate between unmatched edge and matched edge. As the first vertice in path is unmatched, first edge is
		///     automatically unmatched. If next vertex is unmatched as well then path ends otherwise we are on matched vertex in which case we must follow
		///     edge from previous matching. After we follow the matched edge, we would be on other side but now we can not have any other matched edge so 
		///     automatically, next edge must be unmatched to go on opposite side. And so the path continues.
		/// 2. Shortest augmented path is just that: Shortest in length of all augmented paths in graph.
		/// 3. There may be multiple shortest augmented paths of the same length.
		/// 4. Hopcroft Karp algorithm requires that we construct the maximal set of shortest augmented paths that don't have any common vertex between them.
		/// 5. Then so symmetric difference of existing matching and all augmented paths to get the new matching.
		/// 6. Repeat until no more augmented paths are found.
		/// 
		/// How do we do #4?
		/// 
		/// Let our graph be two partitions U, V. The key idea is to add a dummy vertex vDummy and connect it to unmatched vertices in V. Similarly add uDummy and 
		/// connect it to all unmatched vertices in U. Now if we run BFS from uDummy to vDummy then we can get shortest path between an unmatched vertex in U to unmatched vertex in V. 
		/// Due to bi-partiate nature of the graph, this path would zig zag from U to V. However we need to make sure that when going from V to U, we always select matched edge. If there
		/// is no matched edge then we abandon further search on that path and continue exploring other paths. If we make sure of this one condition then it meets all criteria for path being an
		/// augmented path as well as the shortest path.
		/// 
		/// Once we have found the shortest path, we want to make sure we ignore any paths that are longer than this shortest paths. BFS algorithm marks nodes in path with
		/// distance with source being 0. Thus, after doing BFS, we can start at each unmatched vertex in U, follow the path by following nodes with distance that increments by 1. 
		/// By the the time we arrive at destination vDummy, if its distance is 1 more than last node in V then we know that the path we followed is the shortest path. In that case,
		/// we just update the matching for each vertex on path in U and V. Note that each vertex in V on path, except for the last one, is matched vertex. 
		/// So updating this matching starting from first unmatched vertex is equivalent to removing previously matched edge and adding new unmatched edge in matching. This is same
		/// is symmetric difference (i.e. remove edges common to previous matching and add non-common edges in augmented path in new matching).
		/// 
		/// How do we make sure augmented paths are vertex disjoint? After we consider each augmented path, reset distance of vertices in path (that was originally set by BFS)
		/// to infinity. This prevents any other augmenting path with same vertice(s) getting considered.
		/// 
		/// Finally, we actually don't need uDummy because it's there just to put all unmatched vertices in queue when BFS starts. That we can do as just initialization. The 
		/// vDummy can be appened in U for convinience however we don't really need to add edges from V to vDummy. Instead, we can just initialize default pairing for all V
		/// to point to vDummy. That way, if final vertex in V doesn't have any matching vertex in U then we end at vDummy which is end of our augmented path. In below code,
		/// vDummy is denoted as iNil.
		/// 
		/// </summary>
		/// <param name="gU">Left vertices, each with edges to right vertices</param>
		/// <param name="gVCount">Count of right side of vertices</param>
		/// <returns>For each left vertice, corresponding ID for the matched right vertice</returns>
		public static IList<int> GetMatching(IList<int[]> gU, int gVCount)
		{
			//Recreate gU with dummy NIL node
			gU = new List<int[]>(gU);
			gU.Add(new int[] { });

			var iNil = gU.Count - 1; //Index of dummy node
			var gUCount = iNil; //Count of nodes in U without dummy node

			//Create lists that would hold matching, by default they point to NIL
			var pairU = Enumerable.Repeat(iNil, gUCount).ToList();
			var pairV = Enumerable.Repeat(iNil, gVCount).ToList();

			var dist = new int[gU.Count];   //Array to store distances from BFS
			var q = new int[gU.Count];      //Array to use as queue during BFS

			//The modified version of BFS marks shortest augmented paths available using dist array
			//When no more paths are available it returns false
			//Note that we need to find augmented paths starting from either only U or V because 
			//that paths ends on other side anyway.
			while (Bfs(gU, pairU, pairV, dist, iNil, gUCount, q))
			{
				//For each unmatched vertex in U
				for (var u = 0; u < gUCount; ++u)
				{
					if (pairU[u] == iNil)
					{
						//See if we have shotest augmnted path. If we do, then
						//Dfs will also mark vertices along that path unusable for 
						//any other paths in next calls so that paths remain vertex disjoint.
						//Also if Dfs does find shortest augmnted path then it will do symmetric
						//difference along that path, setting proper pairs in pairU and pairV
						Dfs(gU, pairU, pairV, dist, iNil, u);
					}
				}
			}

			//Return pairs for U, converting iNil to -1 to indicate no pairing and also eliminate last dummy vertex 
			return pairU.Select(i => i == iNil ? -1 : i).Take(gUCount).ToList();
		}

		public static IList<int> GetMatching(int left, int right, List<Tuple<int, int>> edges)
		{
			var vertices = new List<List<int>>(left + right);

			for (int i = 0; i < right + left; i++)
			{
				vertices.Add(new List<int>());
			}

			foreach (var e in edges)
			{
				vertices[e.Item1].Add(e.Item2);
				vertices[e.Item2].Add(e.Item1);
			}

			return GetMatching(vertices.Select(x => x.ToArray()).ToList(), right);
		}

		/// <summary>
		/// Finds all available shortest augmented paths in the graph
		/// </summary>
		/// <param name="gU">Adjecency list of U</param>
		/// <param name="pairU">Matching so far for U</param>
		/// <param name="pairV">Matching so far for V (should be consistant with pairU)</param>
		/// <param name="dist">dist array marks the next vertex in shortest path from previous vertex</param>
		/// <param name="iNil">Which vertice is our dummy node</param>
		/// <param name="gUCount">This is just alias for iNil for better readability</param>
		/// <param name="q">The array to be used as queue during BFS. It's just passed from outside to avoid recreating it during each call.</param>
		/// <returns>true if atleast one augmented path exists else false</returns>
		private static bool Bfs(IList<int[]> gU, IReadOnlyList<int> pairU, IReadOnlyList<int> pairV, IList<int> dist, int iNil, int gUCount, IList<int> q)
		{
			//Pointers in queue
			int qiEnqueue = 0, qiDequeue = 0;

			//For each vertex in U, initialize distance to 0 if it was unmatched fo far
			//We will find shortest augmented paths from each unmatched vertices
			for (var u = 0; u < gUCount; ++u)
			{
				if (pairU[u] == iNil)
				{
					dist[u] = 0;

					//enqueue all unmatched vertices in queue
					q[qiEnqueue++] = u;
				}
				else
					dist[u] = int.MaxValue;
			}

			//Set distance of dummy node to infinite. When we find the shortest path, we would end at dummy node and BFS
			//would set its distance to the length of shortest path. We can use this length to eliminate any path that are 
			//longer than. If more than one vertex has same length shortest path then for both of them we can follow the 
			//dist array all the way to dummy node and when we get there we can check that length of path so far is the 
			//same value as in dist[iNil].
			dist[iNil] = int.MaxValue;

			//While queue is not empty
			while (qiDequeue < qiEnqueue)
			{
				//Dequeue next node
				var u = q[qiDequeue++];

				//If length of path to this node has exceed the shortest path
				//we already found then ignore this path and move on to next node
				if (dist[u] < dist[iNil])
				{
					//Go through our adjecency list of this node and
					//see if any of our neighbours for this node in V has an existing match in U OR is unmatched.
					//If it has existing matching then we will travel to next node in U and repeat the process.
					//If there was no existing matching then nextU is dummy node. In that case, if we are getting there
					//for the first time then we have found the shortest path and we mark dummy nodes dist as length of 
					//shortest path we found. If it isn't our first time then either we have another shortest path of same
					//length or it's a longer path. In either case we can just ignore because if it was another shortest path
					//then dist of dummy node is already marked correctly. If it was not then dist of dummy node will be less than
					//dist[u] and it would ignored by Dfs.
					foreach (var v in gU[u])
					{
						var nextU = pairV[v];
						if (dist[nextU] == int.MaxValue)
						{
							dist[nextU] = dist[u] + 1;

							//Note that queue will always contain vertices from U. We don't need dist array for V. This is 
							//because from vertex in U we always go to next vertex in U (or dummy node).
							q[qiEnqueue++] = nextU;
						}
					}
				}
			}

			//If we every found a shortest path then dist[iNil] would contain length of the shortest path
			return dist[iNil] != int.MaxValue;
		}

		/// <summary>
		/// Uses values set in dist array to traverse the path from each unmatched node in U. If we arrive at iNil 
		/// and if our path length matches dist[iNil] then we have one of the shortest paths. In that case, we do
		/// symmetric difference on existing matching with shortest path.
		/// </summary>
		/// <param name="gU">Adjecency list of U</param>
		/// <param name="pairU">Matching so far for U</param>
		/// <param name="pairV">Matching so far for V (should be consistant with pairU)</param>
		/// <param name="dist">dist array marks the next vertex in shortest path from previous vertex</param>
		/// <param name="iNil">Which vertice is our dummy node</param>
		/// <param name="u">Node in U from which we need to find shortest augmented path</param>
		/// <returns>Returns true if shortest augmented path was found from u</returns>
		private static bool Dfs(IList<int[]> gU, IList<int> pairU, IList<int> pairV, IList<int> dist, int iNil, int u)
		{
			//Recursion termination: If we arrive at dummy node during traversal then we have shortest augmented path and so terminate.
			if (u != iNil)
			{
				//For each neighbours of u, see if it's next node in the possible path
				foreach (var v in gU[u])
				{
					var nextU = pairV[v];

					//The neigbour is next node in path if it's matching node is our distance + 1
					if (dist[nextU] == dist[u] + 1)
					{
						//Recursively see if for this next node in path, we have shortest augmented path available
						if (Dfs(gU, pairU, pairV, dist, iNil, nextU))
						{
							//If so then time to do symmetric difference! Note that pairV[u] either has previous matching or is unmatched.
							//If it had previous matching then setting pairV[v] to new value removes that matching and then adds a new matching
							//which in essence is symmetric difference.
							pairU[u] = v;
							pairV[v] = u;
							return true;
						}
					}
				}

				//Mark our node unusable for getting included in any other paths so that all shortest augmented paths are
				//vertex disjoint. For bipartiate case (but not for general graphs) it can be proved that doing this simple
				//vertex elimination results in maximal set of vertex disjoint paths. Why not for general graphs? Imagine
				//5 paths horizontally and one path vertical that cuts across the 5. If we choose vertices of vertical path then
				//we need to eliminate all horizontal paths resulting in non-maximal set.
				dist[u] = int.MaxValue;
				return false;
			}
			return true;
		}

	}
}