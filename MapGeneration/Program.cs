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

			/*{
				var mapDescription = MapDescriptionsDatabase.Reference_17Vertices_WithoutRoomShapes;
				MapDescriptionsDatabase.AddClassicRoomShapes(mapDescription);

				var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler,
					new OrthogonalLineIntersection(), new GridPolygonUtils());

				var configurationSpaces = configurationSpacesGenerator.Generate(mapDescription);
				var layoutGenerator = new SALayoutGenerator<int>();
				layoutGenerator.InjectRandomGenerator(new Random(0));

				benchmark.Execute(layoutGenerator, "Basic generator");
			}*/

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
				setups2.AddSetup("Not lazy", generator => { generator.EnableLazyProcessing(false);});

				//var setups3 = scenario.MakeSetupsGroup();
				//setups3.AddSetup("Perturb pos", generator => { generator.EnablePerturbPositionAfterShape(true);});
				//setups3.AddSetup("No perturb", generator => { generator.EnablePerturbPositionAfterShape(false);});

				benchmark.Execute(layoutGenerator, scenario, MapDescriptionsDatabase.BasicSet, 15);
			}
		}
	}
}
