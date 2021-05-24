using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.GraphBasedGenerator.Common.Constraints.FixedConfigurationConstraint;
using Edgar.Legacy.Core.ChainDecompositions;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal
{
    public static class GraphBasedGeneratorGrid2DUtils
    {
        public static IChainDecomposition<TRoom> GetChainDecomposition<TRoom>(
            ILevelDescription<TRoom> levelDescription,
            IFixedConfigurationConstraint<RoomTemplateInstanceGrid2D, Vector2Int, TRoom> fixedConfigurationConstraint,
            ChainDecompositionConfiguration chainDecompositionConfiguration)
        {
            var fixedPositionRooms = levelDescription
                .GetGraph()
                .Vertices
                .Where(fixedConfigurationConstraint.IsFixedPosition)
                .ToList();
            var fixedRooms = fixedPositionRooms
                .Where(fixedConfigurationConstraint.IsFixedShape)
                .ToList();

            // Prepare chain decomposition algorithm
            chainDecompositionConfiguration = chainDecompositionConfiguration ?? new ChainDecompositionConfiguration();
            var chainDecomposition = new BreadthFirstChainDecomposition<TRoom>(chainDecompositionConfiguration, fixedRooms: fixedPositionRooms);
            var twoStageChainDecomposition = new Common.ChainDecomposition.TwoStageChainDecomposition<TRoom>(
                levelDescription,
                chainDecomposition
            );
            var fixedRoomsChainDecomposition = new FixedRoomChainDecompositionPreprocessing<TRoom>(fixedRooms, twoStageChainDecomposition);

            return fixedRoomsChainDecomposition;
        }
    }
}