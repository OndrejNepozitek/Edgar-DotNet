namespace GUI
{
	using MapGeneration.Core.Interfaces;

	public class GeneratorSettings
	{
		public IMapDescription<string> MapDescription { get; set; }

		public int NumberOfLayouts { get; set; }

		public int RandomGeneratorSeed { get; set; }

		public bool ShowFinalLayouts { get; set; }

		public int ShowFinalLayoutsTime { get; set; }

		public bool ShowAcceptedLayouts { get; set; }

		public int ShowAcceptedLayoutsTime { get; set; }

		public bool ShowPerturbedLayouts { get; set; }

		public int ShowPerturbedLayoutsTime { get; set; }
	}
}