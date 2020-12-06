using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.GraphBasedGenerator.Grid2D.Internal;
using Edgar.Legacy.Core.Configurations.Interfaces.EnergyData;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.GridPseudo3D.Internal
{
	public class ConfigurationGridPseudo3D<TNode, TEnergyData> : 
            IConfiguration<RoomTemplateInstanceGrid2D, Vector2Int, RoomNode<TNode>>,
            IConfiguration<RoomTemplateInstanceGrid2D, Vector3Int, RoomNode<TNode>>,
            IEnergyConfiguration<TEnergyData>,
            ISmartCloneable<ConfigurationGridPseudo3D<TNode, TEnergyData>>
		where TEnergyData : IEnergyData, ISmartCloneable<TEnergyData>
	{
        public Vector2Int Position { get; set; }

        private int Z;

        Vector3Int IPositionConfiguration<Vector3Int>.Position
        {
            get => new Vector3Int(Position.X, Position.Y, Z);
            set
            {
                Position = new Vector2Int(value.X, value.Y);
                Z = value.Z;
            }
        }

        public TEnergyData EnergyData { get; set; }

        public RoomNode<TNode> Room { get; set; }

        public RoomTemplateInstanceGrid2D RoomShape { get; set; }

        public ConfigurationGridPseudo3D()
		{
			/* empty */
		}

		public ConfigurationGridPseudo3D(RoomTemplateInstanceGrid2D shape, Vector2Int position, TEnergyData energyData, RoomNode<TNode> node)
		{
			RoomShape = shape;
			Position = position;
			EnergyData = energyData;
            Room = node;
        }

		public ConfigurationGridPseudo3D<TNode, TEnergyData> SmartClone()
		{
			return new ConfigurationGridPseudo3D<TNode, TEnergyData>(
				RoomShape,
				Position,
				EnergyData.SmartClone(),
				Room
			);
		}
    }
}