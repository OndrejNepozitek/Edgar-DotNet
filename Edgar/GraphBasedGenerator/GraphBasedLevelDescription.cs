using MapGeneration.Core.MapDescriptions;

namespace Edgar.GraphBasedGenerator
{
    public class GraphBasedLevelDescription<TNode> : MapDescription<TNode>
    {
        public string Name { get; set; }

        public OutlineMode OutlineMode { get; set; } = OutlineMode.Points;

        public int MinimumRoomDistance { get; set; } = 0;
    }
}