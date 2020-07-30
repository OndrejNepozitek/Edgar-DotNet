using MapGeneration.Core.MapDescriptions.Interfaces;

namespace MapGeneration.Utils.ConfigParsing.Models
{
	using GeneralAlgorithms.DataStructures.Common;

	public class RoomShapesModel
	{
		public string SetName { get; set; }

		public string RoomDescriptionName { get; set; }

		public bool? Rotate { get; set; }

		public double? Probability { get; set; }

		public bool? NormalizeProbabilities { get; set; }

        public IntVector2? Scale { get; set; }
	}
}