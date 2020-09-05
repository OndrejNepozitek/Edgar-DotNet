using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Priority_Queue;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public class CorridorsPathfinder
    {
        public (List<Vector2Int> path, Dictionary<Vector2Int, int> costs) FindPath(List<Vector2Int> startPoints, List<Vector2Int> goalPoints, Dictionary<Vector2Int, OrthogonalLineGrid2D> fromDoorMapping, Dictionary<Vector2Int, OrthogonalLineGrid2D> toDoorMapping, ITilemap<Vector2Int> tilemap, int? maxLength = null, bool canChangeDirection = true)
        {
            var queue = new SimplePriorityQueue<Vector2Int>();

            var cameFrom = new Dictionary<Vector2Int?, Vector2Int?>();
            var previousDirection = new Dictionary<Vector2Int, Vector2Int>();
            var costSoFar = new Dictionary<Vector2Int, int>();

            foreach (var point in startPoints)
            {
                if (tilemap.IsEmpty(point))
                {
                    
                    // TODO: slow
                    queue.EnqueueWithoutDuplicates(point, 0);

                    cameFrom.Add(point, null);
                    costSoFar.Add(point, 0);
                    previousDirection[point] = fromDoorMapping[point].GetDirectionVector().RotateAroundCenter(270);
                }
            }

            while (queue.Count != 0)
            {
                var item = queue.Dequeue();

                if (goalPoints.Contains(item))
                {
                    return (GetPath(item, cameFrom), costSoFar);
                }

                foreach (var neighbor in item.GetAdjacentVectors())
                {
                    if (tilemap.IsEmpty(neighbor))
                    {
                        var cost = costSoFar[item] + 1;
                        var direction = neighbor - item;

                        // Penalty
                        if (previousDirection[item] != direction)
                        {
                            if (!canChangeDirection)
                            {
                                continue;
                            }

                            cost++;
                        }

                        if (maxLength.HasValue && maxLength.Value < cost)
                        {
                            continue;
                        }

                        if (goalPoints.Contains(neighbor))
                        {
                            var line = toDoorMapping[neighbor];

                            if (direction != line.GetDirectionVector().RotateAroundCenter(90))
                            {
                                cost++;
                            }
                        }

                        float priority = cost + GetHeuristics(neighbor, goalPoints, direction);

                        if (!cameFrom.ContainsKey(neighbor) ||
                            (queue.Contains(neighbor) && queue.GetPriority(neighbor) > priority))
                        {
                            costSoFar[neighbor] = cost;
                            cameFrom[neighbor] = item;
                            previousDirection[neighbor] = direction;

                            if (queue.Contains(neighbor))
                            {
                                queue.UpdatePriority(neighbor, priority);
                            }
                            else
                            {
                                queue.EnqueueWithoutDuplicates(neighbor, priority);
                            }
                        }
                    }
                }

                // TODO: which constant should we use?
                if (cameFrom.Count > 1000)
                {
                    break;
                }
            }

            return (null, costSoFar);
        }

        
        private float GetHeuristics(Vector2Int point, List<Vector2Int> goals, Vector2Int previousDirection)
        {
            var nextStep = point + previousDirection;
            return goals.Min(x => Vector2Int.ManhattanDistance(nextStep, x)) - 1;
        }

        private List<Vector2Int> GetPath(Vector2Int goal, Dictionary<Vector2Int?, Vector2Int?> backEdges)
        {
            var path = new List<Vector2Int>();
            var current = goal;

            while (true)
            {
                path.Add(current);

                var previous = backEdges[current];

                if (previous == null)
                {
                    break;
                }
                else
                {
                    current = previous.Value;
                }
            }

            path.Reverse();

            return path;
        }
    }
}