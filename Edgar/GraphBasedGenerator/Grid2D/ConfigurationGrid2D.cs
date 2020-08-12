using Edgar.GraphBasedGenerator.General;
using Edgar.GraphBasedGenerator.General.Configurations;
using GeneralAlgorithms.DataStructures.Common;
using MapGeneration.Core.Configurations.Interfaces.EnergyData;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.Grid2D
{
	public class ConfigurationGrid2D<TNode, TEnergyData> : IConfiguration<RoomTemplateInstance, Vector2Int, RoomNode<TNode>>, IEnergyConfiguration<TEnergyData>,  ISmartCloneable<ConfigurationGrid2D<TNode, TEnergyData>>
		where TEnergyData : IEnergyData, ISmartCloneable<TEnergyData>
	{
		public Vector2Int Position { get; set; }

		public TEnergyData EnergyData { get; set; }

        public RoomNode<TNode> Room { get; set; }

        public RoomTemplateInstance RoomShape { get; set; }

        public ConfigurationGrid2D()
		{
			/* empty */
		}

		public ConfigurationGrid2D(RoomTemplateInstance shape, Vector2Int position, TEnergyData energyData, RoomNode<TNode> node)
		{
			RoomShape = shape;
			Position = position;
			EnergyData = energyData;
            Room = node;
        }

		public ConfigurationGrid2D<TNode, TEnergyData> SmartClone()
		{
			return new ConfigurationGrid2D<TNode, TEnergyData>(
				RoomShape,
				Position,
				EnergyData.SmartClone(),
				Room
			);
		}
    }
}