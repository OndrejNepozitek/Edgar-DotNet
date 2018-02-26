namespace MapGeneration.Core.GraphDecomposition
{
	using System;
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Graphs;
	using Interfaces.Core;

	public class DummyChainsDecomposition : IChainDecomposition<int>
	{
		public List<List<int>> GetChains(IGraph<int> graph)
		{
			switch (graph.VerticesCount)
			{
				case 9:
				{
					var c1 = new List<int>() { 6, 7, 8 };
					var c2 = new List<int>() { 3, 0, 2, 1 };
					var c3 = new List<int>() { 4, 5 };

					return new List<List<int>>()
					{
						c1,
						c2,
						c3,
					};
				}
				case 17:
				{
					var c1 = new List<int>() { 11, 12, 14, 15 };
					var c2 = new List<int>() { 6, 3, 5, 0, 2 };
					var c3 = new List<int>() { 16, 7, 13, 4, 8 };
					var c4 = new List<int>() { 1, 10, 9 };

					return new List<List<int>>()
					{
						c1,
						c2,
						c3,
						c4,
					};
				}
				case 41:
				{
					var c1 = new List<int>() { 11, 12, 18, 19 };
					var c2 = new List<int>() { 7, 20, 4, 8, 13 };
					var c3 = new List<int>() { 6, 5, 2, 0, 3 };
					var c4 = new List<int>() { 1, 9, 10 };
					var c5 = new List<int>() { 23, 24, 30 };
					var c5s = new List<int>() { 21 };
					var c5ss = new List<int>() { 22, 17, 29 };
					var c5sss = new List<int>() { 16, 27, 28 };
					var c6 = new List<int>() { 14, 15, 26, 25 };
					var c7 = new List<int>() { 31, 32, 33 };
					var c8 = new List<int>() { 35, 34, 36 };
					var c9 = new List<int>() { 38, 39, 37 };
					var c10 = new List<int>() { 40 };

					var graphChains = new List<List<int>>()
					{
						c1,
						c2,
						c3,
						c4,
						c5,
						c5s,
						c5ss,
						c5sss,
						c6,
						c7,
						c8,
						c9,
						c10,
					};

					return graphChains;
				}
			}

			throw new NotSupportedException();
		}
	}
}