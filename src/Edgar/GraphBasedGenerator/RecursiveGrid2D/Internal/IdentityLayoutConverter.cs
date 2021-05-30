using Edgar.Legacy.Core.LayoutConverters.Interfaces;

namespace Edgar.GraphBasedGenerator.RecursiveGrid2D.Internal
{
    public class IdentityLayoutConverter<TLayout> : ILayoutConverter<TLayout, TLayout>
    {
        public TLayout Convert(TLayout layout, bool addDoors)
        {
            return layout;
        }
    }
}