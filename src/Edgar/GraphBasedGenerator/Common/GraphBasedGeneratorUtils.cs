using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Common.ChainDecomposition;
using Edgar.Graphs;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.GraphBasedGenerator.Common
{
    public static class GraphBasedGeneratorUtils
    {
        public static List<Chain<TRoomAlias>> GetChains<TRoom, TRoomAlias>(
            IChainDecomposition<TRoomAlias> chainDecomposition,
            IGraph<TRoomAlias> graph,
            TwoWayDictionary<TRoom, TRoomAlias> roomToAliasMapping,
            List<Chain<TRoom>> chainsOverride
        )
        {
            // If chainsOverride is not null, just map it to TRoomAlias
            if (chainsOverride != null)
            {
                return chainsOverride
                    .Select(x => new Chain<TRoomAlias>(
                        x.Nodes.Select(y => roomToAliasMapping[y]).ToList(),
                        x.Number,
                        x.IsFromFace))
                    .ToList();
            }

            // Otherwise just compute the decomposition from a given graph
            var chains = chainDecomposition.GetChains(graph);

            return chains;
        }
    }
}