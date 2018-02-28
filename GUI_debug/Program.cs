namespace GUI_debug
{
	using GUI;
	using MapGeneration.Utils;
	using System;
	using System.Windows.Forms;


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

			var mapDescription = MapDescriptionsDatabase.Reference_17Vertices_WithoutRoomShapes;
			MapDescriptionsDatabase.AddClassicRoomShapes(mapDescription);

			var settings = new GeneratorSettings
			{
				MapDescription = mapDescription
			};

			Application.Run(new GeneratorWindow(settings));
		}
	}
}
