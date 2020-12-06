namespace Edgar.Geometry
{
    public struct Vector3Int
    {
        public readonly int X;

        public readonly int Y;

        public readonly int Z;

        public Vector2Int Vector2Int => new Vector2Int(X, Y);

        public Vector3Int(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}