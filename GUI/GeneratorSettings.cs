namespace GUI
{
	using MapGeneration.Core.MapDescriptions;
	using MapGeneration.Interfaces.Core.LayoutGenerator;
	using MapGeneration.Interfaces.Core.MapLayouts;

	public class GeneratorSettings
	{
		public MapDescription<int> MapDescription { get; set; }

		public IObservableGenerator<MapDescription<int>, IMapLayout<int>> LayoutGenerator { get; set; }

		public int NumberOfLayouts { get; set; } = 10;

		public int RandomGeneratorSeed { get; set; } = 0;

		public bool ShowRoomNames { get; set; } = true;

		public bool UseOldPaperStyle { get; set; } = false;

		public bool ShowFinalLayouts { get; set; } = false;

		public int ShowFinalLayoutsTime { get; set; } = 2000;

		public bool ShowPartialValidLayouts { get; set; } = false;

		public int ShowPartialValidLayoutsTime { get; set; } = 500;

		public bool ShowPerturbedLayouts { get; set; } = false;

		public int ShowPerturbedLayoutsTime { get; set; } = 50;
	}
}