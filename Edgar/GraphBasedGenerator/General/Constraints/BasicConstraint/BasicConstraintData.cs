namespace Edgar.GraphBasedGenerator.General.Constraints.BasicConstraint
{
    // TODO: should this be a struct or a class?
    public struct BasicConstraintData
    {
        /// <summary>
        /// Overlap area of the node.
        /// </summary>
        public int Overlap { get; set; }

        /// <summary>
        /// How far is the node from a valid position.
        /// </summary>
        public int MoveDistance { get; set; }
    }
}