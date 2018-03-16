namespace Sandbox
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;
	using Examples;
	using GUI;
	using MapGeneration.Utils;

	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var layoutGenerator = LayoutGeneratorFactory.GetDefaultChainBasedGenerator();
			// var layoutGenerator = LayoutGeneratorFactory.GetChainBasedGeneratorWithCorridors(new List<int>() {1});
			layoutGenerator.InjectRandomGenerator(new Random(0));

			// var mapDescription = new BasicsExample().GetMapDescription();
			// var mapDescription = new CorridorsExample().GetMapDescription();
			var mapDescription = new DifferentShapesExample().GetMapDescription();

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
