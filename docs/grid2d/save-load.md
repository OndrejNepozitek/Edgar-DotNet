---
title: Save and load
---

import { Gallery, GalleryImage } from "@theme/Gallery";



In this tutorial, we will see how to save and load level descriptions and generated layouts.

## Level descriptions (JSON)
To save a level description to a JSON file, we can call the [`SaveToJson()`][LevelDescriptionGrid2D.SaveToJson(String, Boolean)] method:


```

levelDescription.SaveToJson("levelDescription.json");


```

And to load a level description from a JSON file, we can use the [`LoadFromJson()`][LevelDescriptionGrid2D.LoadFromJson(String)] method:


```

levelDescription = LevelDescriptionGrid2D<int>.LoadFromJson("levelDescription.json");


```

By default, the JSON serializer is configured to preserve references to objects. That means that when it first encounters an object, it assigns a unique id to id and when it encounters the same object later, it only references that id and does not serialize the object itself. This is good for when we want load the level description back in C# later, but it may cause problems if we want to use the JSON outside C#. Therefore, it is possible to disable this feature:


```

levelDescription.SaveToJson("levelDescription.json", preserveReferences: false);


```

> **Note:**: API reference for the `LevelDescriptionGrid2D` class can be found [here][LevelDescriptionGrid2D].

## Layouts (JSON)


It is possible to save a generated layout to JSON and then load it back:


```

layout.SaveToJson("layout.json");
layout = LayoutGrid2D<int>.LoadFromJson("layout.json");


```

And it is also possible to disable the `preserveReferences` feature:


```

layout.SaveToJson("layout.json", preserveReferences: false);


```

> **Note:**: API reference for the `LayoutGrid2D` class can be found [here][LayoutGrid2D].

## Layouts (PNG)


It is possible to save a generated layout as a PNG. To do that, we have to create an instance of the `DungeonDrawer` class:


```

var dungeonDrawer = new DungeonDrawer<int>();


```

Then we can save the layout as a PNG:


```

dungeonDrawer.DrawLayoutAndSave(layout, "dungeon.png", new DungeonDrawerOptions()
{
    Width = 2000,
    Height = 2000,
});


```

The dungeon drawer produces images that can be seen in all the examples in this documentation. The API reference of the `DungeonDrawerOptions` class can be found [here][DungeonDrawerOptions].
