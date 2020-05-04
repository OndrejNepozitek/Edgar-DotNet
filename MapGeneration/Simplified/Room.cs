namespace MapGeneration.Simplified
{
    public class Room
    {
        public string Identifier { get; }

        public Room(string identifier = "Room")
        {
            Identifier = identifier;
        }

        public override string ToString()
        {
            return Identifier;
        }
    }
}