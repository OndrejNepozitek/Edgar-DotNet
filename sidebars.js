/**
 * Copyright (c) 2017-present, Facebook, Inc.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

/*
   someSidebar: {
    Introduction: ["introduction", "installation"],
    Guides: ["guides-introduction", "basicMapDescription", "differentShapesMapDescription", "differentProbabilitiesMapDescription",  "corridorsMapDescription"],
    Misc: ["gui", "configFormat"]
  },
  */

module.exports = {
  docs: [
    {
      type: "category",
      label: "Introduction",
      items: ["introduction", "installation"]
    },
    {
      type: "category",
      label: "Guides",
      items: ["guides/introduction", "guides/basics", "guides/different-room-descriptions", "guides/corridors", "guides/export"]
    },
    {
      type: "category",
      label: "Guides",
      items: ["grid2d/introduction", "grid2d/basics", /*"examples/repeating-room-templates",*/ "grid2d/corridors", "grid2d/minimum-room-distance", "grid2d/real-life"]
    },
    {
      type: "category",
      label: "API reference",
      items: ["api/dungeon-generator"]
    }
  ]
};
