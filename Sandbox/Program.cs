namespace Sandbox
{
	using System;
	using System.Windows.Forms;
	using GUI;
	using MapGeneration.Utils;
	using MapGeneration.Utils.ConfigParsing;

	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			GuiExceptionHandler.SetupCatching();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			RunExample();
		}

		/// <summary>
		/// Runs one of the examples presented in the Tutorial.
		/// </summary>
		public static void RunExample()
		{
			var configLoader = new ConfigLoader();
			var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator<int>();
			// var layoutGenerator = LayoutGeneratorFactory.GetChainBasedGeneratorWithCorridors<int>(new List<int>() {1});
			layoutGenerator.InjectRandomGenerator(new Random(0));

			// var mapDescription = new BasicsExample().GetMapDescription();
			// var mapDescription = configLoader.LoadMapDescription("Resources/Maps/tutorial_basicDescription.yml");

			// var mapDescription = new DifferentShapesExample().GetMapDescription();
			// var mapDescription = configLoader.LoadMapDescription("Resources/Maps/tutorial_differentShapes.yml");

			// var mapDescription = new DIfferentProbabilitiesExample().GetMapDescription();
			var mapDescription = configLoader.LoadMapDescription("Resources/Maps/tutorial_differentProbabilities.yml");

			// var mapDescription = new CorridorsExample().GetMapDescription();
			// var mapDescription = configLoader.LoadMapDescription("Resources/Maps/tutorial_corridors.yml");

			var settings = new GeneratorSettings
			{
				MapDescription = mapDescription,
				LayoutGenerator = layoutGenerator,

				NumberOfLayouts = 10,

				ShowPartialValidLayouts = false,
				ShowPartialValidLayoutsTime = 500,
			};

			Application.Run(new GeneratorWindow(settings));
		}
	}
}
