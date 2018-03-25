namespace GUI_debug
{
	using MapGeneration.Utils;
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using GUI;
	using MapGeneration.Benchmarks;
	using MapGeneration.Core;
	using MapGeneration.Core.ConfigurationSpaces;
	using MapGeneration.Core.Constraints;
	using MapGeneration.Core.Doors;
	using MapGeneration.Core.Doors.DoorModes;
	using MapGeneration.Core.LayoutOperations;
	using MapGeneration.Core.MapDescriptions;

	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var offsets = new List<int>() {2};
			var mapDescription = new MapDescription<string>();

			mapDescription.AddRoom("0");
			mapDescription.AddRoom("1");
			mapDescription.AddRoom("2");
			mapDescription.AddRoom("3");

			mapDescription.AddPassage("0", "1");
			mapDescription.AddPassage("1", "2");
			mapDescription.AddPassage("2", "3");
			mapDescription.AddPassage("3", "0");


			mapDescription.SetWithCorridors(true, offsets);

			MapDescriptionsDatabase.AddClassicRoomShapes(mapDescription);
			MapDescriptionsDatabase.AddCorridorRoomShapes(mapDescription, offsets[0]);

			var random = new Random(0);
			var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
			//var corridorConfigurationSpaces = configurationSpacesGenerator.Generate<int, Configuration<EnergyDataCorridors>>(mapDescription, 3);
			//corridorConfigurationSpaces.InjectRandomGenerator(random);

			//var layoutGenerator = new SALayoutGenerator<Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>>(
			//	graph => new Layout<Configuration<EnergyDataCorridors>>(graph),
			//	(configurationSpaces, sigma) =>
			//	{
			//		var operations =  new
			//			LayoutOperationsWithCorridors<Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>, IntAlias<GridPolygon>, EnergyDataCorridors>(
			//				configurationSpaces,
			//				new PolygonOverlap(),
			//				sigma,
			//				mapDescription,
			//				corridorConfigurationSpaces);

			//		operations.AddContraints(new CorridorConstraints<Layout<Configuration<EnergyDataCorridors>>, int, Configuration<EnergyDataCorridors>, EnergyDataCorridors, IntAlias<GridPolygon>>(
			//			mapDescription,
			//			sigma,
			//			corridorConfigurationSpaces
			//		));

			//		return operations;
			//	});

			var layoutGenerator = LayoutGeneratorFactory.GetChainBasedGeneratorWithCorridors<string>(new List<int>() { 2 });
			layoutGenerator.InjectRandomGenerator(random);
			layoutGenerator.SetLayoutValidityCheck(false);
			// layoutGenerator.SetSimulatedAnnealing(50, 500, 10);

			//var settings = new GeneratorSettings
			//{
			//	MapDescription = mapDescription,
			//	LayoutGenerator = layoutGenerator,

			//	NumberOfLayouts = 10,

			//	ShowPartialValidLayouts = true,
			//	ShowPartialValidLayoutsTime = 500,
			//};

			var benchmark = Benchmark.CreateFor(layoutGenerator);
			benchmark.Execute(layoutGenerator, "Test", new List<Tuple<string, MapDescription<string>>>() { new Tuple<string, MapDescription<string>>("Test", mapDescription)}, 20);

			//Application.Run(new GeneratorWindow(settings));
		}
	}
}
