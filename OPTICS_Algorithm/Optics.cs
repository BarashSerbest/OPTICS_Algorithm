using System;
using System.Collections.Generic;
using Accord.Math;
using Accord.Math.Distances;

namespace OPTICS_Algorithm
{
    public class Optics<T>
    {
        private readonly int _minPts;
        private readonly double _xi;

        public Optics(int minPts, double xi)
        {
            _minPts = minPts;
            _xi = xi;
        }

        public int[] Compute(T[][] data)
        {
            int n = data.Length;
            var visited = new bool[n];
            var coreDistances = new double[n];
            var reachabilityDistances = new double[n];
            var clusterIDs = new int[n];
            var clusters = new List<int[]>();

            for (int i = 0; i < n; i++)
            {
                if (!visited[i])
                {
                    visited[i] = true;
                    var neighbors = RangeSearch(data, data[i]);
                    if (neighbors.Length < _minPts)
                    {
                        coreDistances[i] = double.PositiveInfinity;
                    }
                    else
                    {
                        var seeds = new List<int>();
                        Update(data, i, neighbors, visited, coreDistances, seeds);
                        clusters.Add(seeds.ToArray());
                    }
                }
            }

            int clusterID = 0;
            for (int i = 0; i < clusters.Count; i++)
            {
                var seeds = clusters[i];
                foreach (int seed in seeds)
                {
                    if (reachabilityDistances[seed] <= _xi)
                    {
                        clusterIDs[seed] = clusterID;
                    }
                    else
                    {
                        clusterIDs[seed] = -1;
                    }
                }
                clusterID++;
            }

            return clusterIDs;
        }

        private int[] RangeSearch(T[][] data, T[] query)
        {
            double[] distances = new double[data.Length];
            var neighbors = new List<int>();
            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] <= _xi)
                {
                    neighbors.Add(i);
                }
            }
            return neighbors.ToArray();
        }

        private void Update(T[][] data, int p, int[] neighbors, bool[] visited, double[] coreDistances, List<int> seeds)
        {
            visited[p] = true;
            seeds.Add(p);

            var coredist = _xi;

            for (int i = 0; i < neighbors.Length; i++)
            {
                int q = neighbors[i];
                if (!visited[q])
                {
                    visited[q] = true;
                    var qNeighbors = RangeSearch(data, data[q]);
                    if (qNeighbors.Length >= _minPts)
                    {
                        Update(data, q, qNeighbors, visited, coreDistances, seeds);
                    }
                }
                if (coreDistances[q] < coredist)
                {
                    coredist = coreDistances[q];
                }
            }

            coreDistances[p] = coredist;
        }
    }
}
