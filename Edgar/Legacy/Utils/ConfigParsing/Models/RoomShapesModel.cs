using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Utils.ConfigParsing.Models
{
    public class RoomShapesModel
	{
		public string SetName { get; set; }

		public string RoomDescriptionName { get; set; }

		public bool? Rotate { get; set; }

		public double? Probability { get; set; }

		public bool? NormalizeProbabilities { get; set; }

        public Vector2Int? Scale { get; set; }
	}
}