---
title: Minimum room distance
---

import { Gallery, GalleryImage } from "@theme/Gallery";


By default, the outlines of individual rooms are allowed to overlap. That means that a single point on the integer grid can belong to outlines of multiple rooms. However, that might not always be what we want. For example, we may think that it is more aesthetically pleasing to have gaps between rooms, or we may use a a tileset that does not work if the outline overlap.

It is possible to configure the minimum distance between individual rooms using the `MinimumRoomDistance` property of level descriptions. By default, this distance is set to 0, but it can be set to any non-negative number. The only thing to keep in mind is that the greater the minimum distance, the more constrained is the generator, which may significantly worsen the performance. However, setting the minimum distance to 1 or 2 should be almost always okay.

## Setup
We will use the level description from the [Corridors](corridors) example:


```

// Replace this with your own level description if you want to run this locally
var levelDescription = new CorridorsExample().GetLevelDescription();


```

And set the minimum room distance to an integer value that is passed to the example as a parameter:


```

levelDescription.MinimumRoomDistance = minimumRoomDistance;


```

## Results

Levels with the minimum room distance set to 0:


<Gallery cols={2}>
<GalleryImage src={require('./minimum-room-distance/0_0.png').default} />
<GalleryImage src={require('./minimum-room-distance/0_1.png').default} />
<GalleryImage src={require('./minimum-room-distance/0_2.png').default} />
<GalleryImage src={require('./minimum-room-distance/0_3.png').default} />
</Gallery>

<div style={{ textAlign: 'center', marginTop: '-15px' }}>

*Average time to generate the level: 0.01s (0.00s init, 0.01s generation itself)*

</div>


Levels with the minimum room distance set to 1:


<Gallery cols={2}>
<GalleryImage src={require('./minimum-room-distance/1_0.png').default} />
<GalleryImage src={require('./minimum-room-distance/1_1.png').default} />
<GalleryImage src={require('./minimum-room-distance/1_2.png').default} />
<GalleryImage src={require('./minimum-room-distance/1_3.png').default} />
</Gallery>

<div style={{ textAlign: 'center', marginTop: '-15px' }}>

*Average time to generate the level: 0.02s (0.01s init, 0.01s generation itself)*

</div>


Levels with the minimum room distance set to 2:


<Gallery cols={2}>
<GalleryImage src={require('./minimum-room-distance/2_0.png').default} />
<GalleryImage src={require('./minimum-room-distance/2_1.png').default} />
<GalleryImage src={require('./minimum-room-distance/2_2.png').default} />
<GalleryImage src={require('./minimum-room-distance/2_3.png').default} />
</Gallery>

<div style={{ textAlign: 'center', marginTop: '-15px' }}>

*Average time to generate the level: 0.03s (0.00s init, 0.03s generation itself)*

</div>


Levels with the minimum room distance set to 4:


<Gallery cols={2}>
<GalleryImage src={require('./minimum-room-distance/3_0.png').default} />
<GalleryImage src={require('./minimum-room-distance/3_1.png').default} />
<GalleryImage src={require('./minimum-room-distance/3_2.png').default} />
<GalleryImage src={require('./minimum-room-distance/3_3.png').default} />
</Gallery>

<div style={{ textAlign: 'center', marginTop: '-15px' }}>

*Average time to generate the level: 0.45s (0.00s init, 0.45s generation itself)*

</div>


We can see that performance difference between values 2 and 4 is quite significant.

It is not possible to set the minimum room distance to more than 4 because we would have to use longer corridors in order to be able to generate such levels.
## Source code

```
using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Drawing;

namespace Examples
{
    public class MinimumRoomDistanceExample 
    {
        public LevelDescriptionGrid2D<int> GetLevelDescription(int minimumRoomDistance = 0)
        {
            // Replace this with your own level description if you want to run this locally
            var levelDescription = new CorridorsExample().GetLevelDescription();

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
    }
}
```

