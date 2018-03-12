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

			var mapDescription = MapDescriptionsDatabase.Reference_41Vertices_WithoutRoomShapes;
			mapDescription.SetWithCorridors(true);
			MapDescriptionsDatabase.AddClassicRoomShapes(mapDescription);

			{
				var width = 3;
				var room = new RoomDescription(
					GridPolygon.GetRectangle(width, 1),
					new SpecificPositionsMode(new List<OrthogonalLine>()
					{
						new OrthogonalLine(new IntVector2(0, 0), new IntVector2(0, 1)),
						new OrthogonalLine(new IntVector2(width, 0), new IntVector2(width, 1)),
					})
				);

				mapDescription.AddCorridorShapes(room);
			}

			{
				var width = 2;
				var room = new RoomDescription(
					GridPolygon.GetRectangle(width, 1),
					new SpecificPositionsMode(new List<OrthogonalLine>()
					{
						new OrthogonalLine(new IntVector2(0, 0), new IntVector2(0, 1)),
						new OrthogonalLine(new IntVector2(width, 0), new IntVector2(width, 1)),
					})
				);

				mapDescription.AddCorridorShapes(room);
			}

			{
				var width = 2;
				var room = new RoomDescription(
					new GridPolygonBuilder()
						.AddPoint(0, 0)
						.AddPoint(0, 2)
						.AddPoint(2, 2)
						.AddPoint(2, 1)
						.AddPoint(1, 1)
						.AddPoint(1, 0)
						.Build()
					,
					new SpecificPositionsMode(new List<OrthogonalLine>()
					{
						new OrthogonalLine(new IntVector2(0, 0), new IntVector2(1, 0)),
						new OrthogonalLine(new IntVector2(2, 1), new IntVector2(2, 2)),
					})
				);

				mapDescription.AddCorridorShapes(room);
			}

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

			var layoutGenerator = LayoutGeneratorFactory.GetSALayoutGeneratorWithCorridors(new List<int>() { 2 });
			layoutGenerator.InjectRandomGenerator(random);
			layoutGenerator.SetLayoutValidityCheck(false);
			// layoutGenerator.SetSimulatedAnnealing(50, 500, 10);

			var settings = new GeneratorSettings
			{
				MapDescription = mapDescription,
				LayoutGenerator = layoutGenerator,

				NumberOfLayouts = 10,

				ShowPartialValidLayouts = true,
				ShowPartialValidLayoutsTime = 500,
			};

			var benchmark = new Benchmark();
			// benchmark.Execute(layoutGenerator, "Test", new List<Tuple<string, MapDescription<int>>>() { new Tuple<string, MapDescription<int>>("Test", mapDescription)}, 20);

			Application.Run(new GeneratorWindow(settings));
		}
	}
}
