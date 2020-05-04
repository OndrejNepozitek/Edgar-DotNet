namespace MapGeneration.Core.Configurations.Interfaces
{
    // TODO: do we need this?
    public interface IShapeConfiguration<out TShapeContainer>
    {
        /// <summary>
        /// Shape container of the node.
        /// </summary>
        TShapeContainer ShapeContainer { get; }
    }
}