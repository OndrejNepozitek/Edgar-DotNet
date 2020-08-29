using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Drawing;

namespace Edgar.Examples.Grid2D
{
    public class MinimumRoomDistanceExample : IExampleGrid2D<int>
    {
        #region no-clean

        public string Name => "Minimum room distance";

        public string DocsFileName => "minimum-room-distance";

        #endregion

        public LevelDescriptionGrid2D<int> GetLevelDescription(int minimumRoomDistance = 0)
        {
            //md By default, the outlines of individual rooms are allowed to overlap. That means that a single point on the integer grid can belong to outlines of multiple rooms. However, that might not always be what we want. For example, we may think that it is more aesthetically pleasing to have gaps between rooms, or we may use a a tileset that does not work if the outline overlap.

            //md It is possible to configure the minimum distance between individual rooms using the `MinimumRoomDistance` property of level descriptions. By default, this distance is set to 0, but it can be set to any non-negative number. The only thing to keep in mind is that the greater the minimum distance, the more constrained is the generator, which may significantly worsen the performance. However, setting the minimum distance to 1 or 2 should be almost always okay.

            //md ## Setup
            //md We will use the level description from the [Corridors](corridors) example:

            // Replace this with your own level description if you want to run this locally
            var levelDescription = new CorridorsExample().GetLevelDescription();

            //md And set the minimum room distance to an integer value that is passed to the example as a parameter:

            levelDescription.MinimumRoomDistance = minimumRoomDistance;

            return levelDescription;
        }

        public void Run()
        {
            var levelDescription = GetLevelDescription();
            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
            var layout = generator.GenerateLayout();

            var drawer = new DungeonDrawer<int>();
            var bitmap = drawer.DrawLayout(layout, new DungeonDrawerOptions()
            {
                Height = 1000,
                Width = 1000,
            });
            bitmap.Save("minimum_room_distance.png");
        }

        #region no-clean

        public IEnumerable<LevelDescriptionGrid2D<int>> GetResults()
        {
            //md Levels with the minimum room distance set to 0:

            yield return GetLevelDescription(0);

            //md Levels with the minimum room distance set to 1:

            yield return GetLevelDescription(1);

            //md Levels with the minimum room distance set to 2:

            yield return GetLevelDescription(2);

            //md Levels with the minimum room distance set to 4:

            yield return GetLevelDescription(4);
            
            //md We can see that performance difference between values 2 and 4 is quite significant.

            //md It is not possible to set the minimum room distance to more than 4 because we would have to use longer corridors in order to be able to generate such levels.
        }

        #endregion
    }
}