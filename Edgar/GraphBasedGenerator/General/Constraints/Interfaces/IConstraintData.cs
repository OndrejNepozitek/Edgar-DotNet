namespace Edgar.GraphBasedGenerator.General.Constraints.Interfaces
{
    public interface IConstraintData
    {
        /// <summary>
        /// Energy of the node.
        /// </summary>
        float Energy { get; set; }

        /// <summary>
        /// Whether the energy data is valid.
        /// </summary>
        bool IsValid { get; set; }
    }
}