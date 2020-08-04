namespace Edgar.GraphBasedGenerator.Constraints.CorridorConstraint
{
    public struct CorridorConstraintData
    {
        /// <summary>
        /// How far is the node from valid position with respect to the neighbors
        /// in the original without corridor rooms.
        /// </summary>
        public int CorridorDistance { get; set; }
    }
}