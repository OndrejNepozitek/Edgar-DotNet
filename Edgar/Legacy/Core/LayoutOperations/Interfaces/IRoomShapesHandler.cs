using System.Collections.Generic;

namespace MapGeneration.Core.LayoutOperations.Interfaces
{
    /// <summary>
    /// An interface that computes which room shapes are available for a given node.
    /// </summary>
    public interface IRoomShapesHandler<in TLayout, in TNode, TShape>
    {
        /// <summary>
        /// Gets all the room shapes that are allowed for a given node in the context of a given layout.
        /// </summary>
        /// <param name="layout">Current layout.</param>
        /// <param name="node"></param>
        /// <param name="tryToFixEmpty">Whether to try to return at least some room shapes even though not all requirements are satisfied.</param>
        /// <returns></returns>
        List<TShape> GetPossibleShapesForNode(TLayout layout, TNode node, bool tryToFixEmpty);

        TShape GetRandomShapeWithoutConstraintsDoNotUse(TNode node);

        bool CanPerturbShapeDoNotUse(TNode node);
    }
}