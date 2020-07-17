---
title: Performance tips
sidebar_label: Performance tips
---

## Room templates

## Corridors

Instead, a completely different approach was implemented. The algorithm works in 2 stages:
- In the first stage, the algorithm ignores all the corridor rooms, but tries to layout all the basic rooms as if there were corridors between them. When this stage succeeds, the second stage continues.
- In the second stage, corridors are added between the basic rooms. And if that is not possible, go back to stage on.

In order for this to work properly, we need to make sure that the second stage almost always succeeds. For example, if we use very long corridors, it may often happen that the only way of connecting basic rooms with corridors is that the corridors cross one another, which causes the algorithm to go back to the first stage and try again.

## Graphs