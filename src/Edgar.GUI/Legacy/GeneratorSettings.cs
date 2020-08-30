using Edgar.Legacy.Core.LayoutGenerators.Interfaces;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.Core.MapLayouts;

namespace Edgar.GUI.Legacy
{
    public class GeneratorSettings
	{
		public IMapDescription<int> MapDescription { get; set; }

		public IObservableGenerator<MapLayout<int>> LayoutGenerator { get; set; }

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

		public bool ExportShownLayouts { get; set; } = false;

		public bool FixedFontSize { get; set; } = false;

		public int FixedFontSizeValue { get; set; } = 12;

		public bool FidexSquareExport { get; set; } = false;

		public int FixedSquareExportValue { get; set; } = 800;

		public bool FixedPositionsAndScale { get; set; } = false;

		public decimal FixedPositionsAndScaleValue { get; set; } = 35;
	}
}