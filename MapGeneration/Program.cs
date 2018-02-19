namespace MapGeneration
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Benchmarks;
	using Core;
	using Core.ConfigurationSpaces;
	using Core.Doors;
	using Core.Doors.DoorModes;
	using Core.GraphDecomposition;
	using Core.Interfaces;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.Algorithms.Polygons;
	using Utils;
	using Utils.ConfigParsing;
	using Utils.ConfigParsing.Deserializers;
	using Utils.ConfigParsing.Models;
	using YamlDotNet.RepresentationModel;
	using YamlDotNet.Serialization;
	using YamlDotNet.Serialization.NamingConventions;
	using YamlDotNet.Serialization.NodeDeserializers;

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
				setups1.AddSetup("Breadth first", generator => { generator.SetChainDecomposition(new BreadthFirstLongerChainsDecomposition<int>(new GraphDecomposer<int>())); });
				// setups1.AddSetup("Handmade decomposition", generator => { generator.SetChainDecomposition(new DummyChainsDecomposition()); });
				//setups1.AddSetup("Basic decomposition", generator => { generator.SetChainDecomposition(new BasicChainsDecomposition<int>(new GraphDecomposer<int>())); });
				//setups1.AddSetup("Longer chains", generator => { generator.SetChainDecomposition(new LongerChainsDecomposition<int>(new GraphDecomposer<int>())); });
				
				var setups2 = scenario.MakeSetupsGroup();
				setups2.AddSetup("Lazy", generator => { generator.EnableLazyProcessing(true);});
				//setups2.AddSetup("Not lazy", generator => { generator.EnableLazyProcessing(false); });

				//var setups3 = scenario.MakeSetupsGroup();
				//setups3.AddSetup("Perturb pos", generator => { generator.EnablePerturbPositionAfterShape(true);});
				//setups3.AddSetup("No perturb", generator => { generator.EnablePerturbPositionAfterShape(false);});

				var setups4 = scenario.MakeSetupsGroup();
				//for (var i = 0; i < 10; i++)
				//{
				//	var sigma = (int)Math.Pow(10, i);
				//	setups4.AddSetup($"Sigma avg {sigma}", generator => { generator.EnableSigmaFromAvg(true, sigma); });
				//}
				//setups4.AddSetup("Sigma constant", generator => { generator.EnableSigmaFromAvg(false); });
				setups4.AddSetup($"Sigma avg {100}", generator => { generator.EnableSigmaFromAvg(true, 10); });

				var setups5 = scenario.MakeSetupsGroup();
				//for (var i = 1; i < 10; i++)
				//{
				//	var scale = (float) (i * 0.2);
				//	setups5.AddSetup($"Difference scale {scale}", generator => { generator.EnableDifferenceFromAvg(true, scale); });
				//}
				//setups5.AddSetup("Difference old", generator => { generator.EnableDifferenceFromAvg(false); });
				setups5.AddSetup("Difference from avg", generator => { generator.EnableDifferenceFromAvg(true, 0.4f); });

				var setups6 = scenario.MakeSetupsGroup();
				//for (var i = 1; i < 10; i++)
				//{
				//	var chance = i * 0.03f;
				//	setups6.AddSetup($"Perturb outside with chance {chance}", generator => { generator.EnablePerturbOutsideChain(true, chance); });
				//}
				setups6.AddSetup("Perturb inside", generator => { generator.EnablePerturbOutsideChain(false); });

				benchmark.Execute(layoutGenerator, scenario, MapDescriptionsDatabase.ReferenceSet, 30);
			}
		}
	}
}
