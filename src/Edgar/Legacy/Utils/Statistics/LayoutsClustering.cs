using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Utils.Statistics
{
    public delegate double GetDistanceDelegate<in TLayout>(TLayout layout1, TLayout layout2);

    public class LayoutsClustering<TLayout>
    {
        public List<List<TLayout>> GetClusters(List<TLayout> layouts, GetDistanceDelegate<TLayout> getDistance,
            double stopDistance)
        {
            var layoutToIntMapping = layouts.CreateIntMapping();
            var layoutDistanceMatrix = GetLayoutDistanceMatrix(layoutToIntMapping, getDistance);
            var clusters = GetInitialClusters(layoutToIntMapping);
            var clusterDistanceMatrix =
                layoutDistanceMatrix.Select(x => x.ToArray()).ToArray(); // Copy layoutDistanceMatrix array

            // Compute clusters
            while (clusters.Count > 10)
            {
                var minDistance = double.MaxValue;
                var minCluster1 = -1;
                var minCluster2 = -1;

                foreach (var cluster1 in clusters.Keys)
                {
                    foreach (var cluster2 in clusters.Keys)
                    {
                        if (cluster1 != cluster2)
                        {
                            var distance = clusterDistanceMatrix[cluster1][cluster2];

                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                minCluster1 = cluster1;
                                minCluster2 = cluster2;
                            }
                        }
                    }
                }

                if (minDistance > stopDistance)
                {
                    break;
                }

                MergeClusters(minCluster1, minCluster2, clusters, clusterDistanceMatrix, layoutDistanceMatrix);
            }

            // Prepare output
            var output = clusters
                .Values
                .OrderByDescending(x => x.Count)
                .Select(x => x.Select(y => layoutToIntMapping.GetByValue(y)).ToList())
                .ToList();

            return output;
        }

        private void MergeClusters(int cluster1, int cluster2, Dictionary<int, List<int>> clusters,
            double[][] clusterDistanceMatrix, double[][] layoutDistanceMatrix)
        {
            // Merge cluster
            clusters[cluster1].AddRange(clusters[cluster2]);

            // Remove cluster2
            clusters.Remove(cluster2);

            // Recompute distances
            foreach (var otherCluster in clusters.Keys)
            {
                if (cluster1 != otherCluster)
                {
                    var distance =
                        GetClustersDistance(clusters[cluster1], clusters[otherCluster], layoutDistanceMatrix);
                    clusterDistanceMatrix[cluster1][otherCluster] = distance;
                    clusterDistanceMatrix[otherCluster][cluster1] = distance;
                }
            }
        }

        private double GetClustersDistance(List<int> cluster1, List<int> cluster2, double[][] layoutDistanceMatrix)
        {
            var distances = new List<double>();

            foreach (var cluster1Layout in cluster1)
            {
                foreach (var cluster2Layout in cluster2)
                {
                    var distance = layoutDistanceMatrix[cluster1Layout][cluster2Layout];
                    distances.Add(distance);
                }
            }

            return distances.Max();
        }

        private Dictionary<int, List<int>> GetInitialClusters(TwoWayDictionary<TLayout, int> layouts)
        {
            var clusters = new Dictionary<int, List<int>>();

            foreach (var pair in layouts)
            {
                clusters.Add(clusters.Count, new List<int>() {pair.Value});
            }

            return clusters;
        }

        private double[][] GetLayoutDistanceMatrix(TwoWayDictionary<TLayout, int> layouts,
            GetDistanceDelegate<TLayout> getDistance)
        {
            var distanceMatrix = new double[layouts.Count][];

            for (int i = 0; i < layouts.Count; i++)
            {
                distanceMatrix[i] = new double[layouts.Count];
            }

            for (int i = 0; i < layouts.Count - 1; i++)
            {
                for (int j = i + 1; j < layouts.Count; j++)
                {
                    var distance = getDistance(layouts.GetByValue(i), layouts.GetByValue(j));
                    distanceMatrix[i][j] = distance;
                    distanceMatrix[j][i] = distance;
                }
            }

            return distanceMatrix;
        }
    }
}