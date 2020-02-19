using MapGeneration.Core.MapDescriptions.Interfaces;

namespace MapGeneration.Core.LayoutOperations
{
    public class RoomTemplateRepeatConfig
    {
        public RepeatMode RepeatMode { get; set; } = RepeatMode.AllowRepeat;

        public bool ThrowIfNotSatisfied { get; set; } = true;
    }
}