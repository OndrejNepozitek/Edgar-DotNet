namespace GUI
{
	using MapGeneration.Interfaces.Core;
	using MapGeneration.Interfaces.Core.MapDescription;

	public class GeneratorSettings
	{
		public IMapDescription<int> MapDescription { get; set; }

		public int NumberOfLayouts { get; set; }

		public int RandomGeneratorSeed { get; set; }

		public bool ShowFinalLayouts { get; set; }

		public int ShowFinalLayoutsTime { get; set; }

		public bool ShowPartialValidLayouts { get; set; }

		public int ShowPartialValidLayoutsTime { get; set; }

		public bool ShowPerturbedLayouts { get; set; }

		public int ShowPerturbedLayoutsTime { get; set; }
	}
}