namespace Edgar.Legacy.Utils.ConfigParsing.Models
{
    public class SimpleDoorModeModel : IDoorModeModel
    {
        public int DoorLength { get; set; }

        public int CornerDistance { get; set; }
    }
}