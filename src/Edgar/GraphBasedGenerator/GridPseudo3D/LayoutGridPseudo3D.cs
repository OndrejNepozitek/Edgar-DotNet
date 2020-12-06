using System.Collections.Generic;
using Edgar.Utils;

namespace Edgar.GraphBasedGenerator.GridPseudo3D
{
    // TODO: keep DRY
    /// <summary>
    /// Represents a final layout produced by the generator.
    /// </summary>
    public class LayoutGridPseudo3D<TRoom>
    {
        /// <summary>
        /// List of rooms in the level.
        /// </summary>
        public List<LayoutRoomGridPseudo3D<TRoom>> Rooms { get; }

        /// <param name="rooms">See the <see cref="Rooms"/> property.</param>
        public LayoutGridPseudo3D(List<LayoutRoomGridPseudo3D<TRoom>> rooms)
        {
            Rooms = rooms;
        }

        /// <summary>
        /// Saves the layout to a JSON file.
        /// </summary>
        /// <param name="filename">Path to the JSON file.</param>
        /// <param name="preserveReferences">Whether to preserve references to objects.</param>
        public void SaveToJson(string filename, bool preserveReferences = true)
        {
            JsonUtils.SaveToFile(this, filename, preserveReferences);
        }

        /// <summary>
        /// Loads a layout from a given JSON file.
        /// </summary>
        /// <param name="filename">Path to the JSON file.</param>
        /// <returns></returns>
        public static LayoutGridPseudo3D<TRoom> LoadFromJson(string filename)
        {
            return JsonUtils.LoadFromFile<LayoutGridPseudo3D<TRoom>>(filename);
        }
    }
}