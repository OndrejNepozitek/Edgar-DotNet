using System.Collections.Generic;
using MapGeneration.Interfaces.Core.LayoutGenerator;

namespace MapGeneration.Core.LayoutGenerators
{
    public class SimpleChainBasedGenerator<TMapDescription, TLayout, TNode> : ILayoutGenerator<TMapDescription, TLayout>
    {
        public IList<TLayout> GetLayouts(TMapDescription mapDescription, int numberOfLayouts)
        {
            throw new System.NotImplementedException();
        }
    }
}