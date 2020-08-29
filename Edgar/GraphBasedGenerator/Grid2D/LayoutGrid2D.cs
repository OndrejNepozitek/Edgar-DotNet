using System.Collections.Generic;
using Edgar.Utils;

namespace Edgar.GraphBasedGenerator.Grid2D
{
	/// <summary>
	/// Represents a final layout produced by the generator.
	/// </summary>
    public class LayoutGrid2D<TRoom>
	{
		/// <summary>
		/// List of rooms in the level.
		/// </summary>
		public List<LayoutRoomGrid2D<TRoom>> Rooms { get; }

		/// <param name="rooms">See the <see cref="Rooms"/> property.</param>
		public LayoutGrid2D(List<LayoutRoomGrid2D<TRoom>> rooms)
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
        public static LayoutGrid2D<TRoom> LoadFromJson(string filename)
        {
            return JsonUtils.LoadFromFile<LayoutGrid2D<TRoom>>(filename);
        }
	}
}