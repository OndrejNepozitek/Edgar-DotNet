namespace MapGeneration
{
	using System;
	using System.Collections.Generic;
	using Benchmarks;
	using Core;
	using Core.ConfigurationSpaces;
	using Core.Doors;
	using Core.GraphDecomposition;
	using Core.Interfaces;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.Algorithms.Polygons;
	using Utils;

	internal class Program
	{
		private static void Main(string[] args)
		{
			var benchmark = new Benchmark();

			{
				var layoutGenerator = new SALayoutGenerator<int>();
				layoutGenerator.InjectRandomGenerator(new Random(0));

				var scenario = new BenchmarkScenario<SALayoutGenerator<int>, int>();
				scenario.SetRunsCount(1);

				var setups1 = scenario.MakeSetupsGroup();
				setups1.AddSetup("Handmade decomposition", generator => { generator.SetChainDecomposition(new DummyChainsDecomposition()); });
				//setups1.AddSetup("Basic decomposition", generator => { generator.SetChainDecomposition(new BasicChainsDecomposition<int>(new GraphDecomposer<int>())); });
				//setups1.AddSetup("Longer chains", generator => { generator.SetChainDecomposition(new LongerChainsDecomposition<int>(new GraphDecomposer<int>())); });
				//setups1.AddSetup("Breadth first", generator => { generator.SetChainDecomposition(new BreadthFirstLongerChainsDecomposition<int>()); });

				var setups2 = scenario.MakeSetupsGroup();
				setups2.AddSetup("Lazy", generator => { generator.EnableLazyProcessing(true);});
				//setups2.AddSetup("Not lazy", generator => { generator.EnableLazyProcessing(false); });

				//var setups3 = scenario.MakeSetupsGroup();
				//setups3.AddSetup("Perturb pos", generator => { generator.EnablePerturbPositionAfterShape(true);});
				//setups3.AddSetup("No perturb", generator => { generator.EnablePerturbPositionAfterShape(false);});

				var setups4 = scenario.MakeSetupsGroup();
				for (var i = 0; i < 10; i++)
				{
					var sigma = (i + 1);
					setups4.AddSetup($"Sigma avg {sigma}", generator => { generator.EnableSigmaFromAvg(true, sigma); });
				}
				setups4.AddSetup("Sigma constant", generator => { generator.EnableSigmaFromAvg(false); });

				benchmark.Execute(layoutGenerator, scenario, MapDescriptionsDatabase.Reference_17Vertices_ScaledSet, 30);
			}
		}
	}
}
