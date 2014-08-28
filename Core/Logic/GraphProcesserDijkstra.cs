using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Core.Logic
{
    public class GraphProcesserDijkstra<T, TE>
        where T : new()
        where TE : IEdge<T>
    {
        private List<T> basis;
        private Dictionary<T, int> distances;
        private Dictionary<T, int> oldDistances;
        private Dictionary<T, T> previous;
        private Dictionary<T, T> oldPrevious;

        public List<TE> Edges;
        public List<T> Nodes;

        public GraphProcesserDijkstra(IEdgeListGraph<T, TE> graph)
        {
            Edges = new List<TE>(graph.Edges.ToArray());
            Nodes = new List<T>(graph.Vertices.ToArray());
        }

        private void Init(int weight)
        {
            basis = new List<T>();
            distances = new Dictionary<T, int>();
            previous = new Dictionary<T, T>();
            oldDistances = new Dictionary<T, int>();
            oldPrevious = new Dictionary<T, T>();

            foreach (var n in Nodes)
            {
                previous.Add(n, n);
                basis.Add(n);
                distances.Add(n, weight);
                oldDistances.Add(n, weight);
            }
        }

        public Dictionary<T, int> GetShortestPath(T start)
        {
            Init(int.MaxValue);
            distances[start] = 0;

            while (basis.Count > 0)
            {
                var currentPixel = GetNodeWithSmallestDistance();
                if (currentPixel.Equals(new T()))
                {
                    basis.Clear();
                }
                else
                {
                    foreach (var currentNeighbor in GetNeighbors(currentPixel))
                    {
                        var newDistance = distances[currentPixel] + GetDistanceBetween(currentPixel, currentNeighbor);
                        if (newDistance < distances[currentNeighbor])
                        {
                            distances[currentNeighbor] = newDistance;
                            previous[currentNeighbor] = currentPixel;
                        }
                    }
                    basis.Remove(currentPixel);
                }
            }
            return distances;
        }

        //TODO: Otimizar metodo para nao percorrer o mesmo caminho quando houver linhas retas
        public Dictionary<T, int> GetLongestPath(T start)
        {
            Init(int.MaxValue);
            distances[start] = 0;

            while (basis.Count > 0)
            {
                var currentPixel = GetNodeWithSmallestDistance();
                if (currentPixel.Equals(new T()))
                {
                    basis.Clear();
                }
                else
                {
                    foreach (var currentNeighbor in GetNeighbors(currentPixel))
                    {
                        var newDistance = distances[currentPixel] + GetDistanceBetween(currentPixel, currentNeighbor);
                        if (distances[currentNeighbor] == int.MaxValue)
                        {
                            distances[currentNeighbor] = newDistance;
                            previous[currentNeighbor] = currentPixel;
                        }
                        else
                        {
                            if (newDistance > distances[currentNeighbor])
                            {
                                if (!previous[currentPixel].Equals(currentNeighbor))
                                {
                                    oldDistances[currentNeighbor] = distances[currentNeighbor];
                                    oldPrevious[currentNeighbor] = previous[currentNeighbor];

                                    distances[currentNeighbor] = newDistance;
                                    previous[currentNeighbor] = currentPixel;
                                }
                                else
                                {
                                    if (oldDistances[currentPixel] != int.MaxValue)
                                    {
                                        newDistance = oldDistances[currentPixel] +
                                                      GetDistanceBetween(currentPixel, currentNeighbor);
                                        if (newDistance > distances[currentNeighbor] &&
                                            !previous[currentPixel].Equals(currentNeighbor))
                                        {
                                            distances[currentNeighbor] = newDistance;
                                            previous[currentNeighbor] = currentPixel;
                                            basis.Insert(0, currentNeighbor);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    basis.Remove(currentPixel);
                }
            }
            return distances;
        }

        public List<T> GetPathTo(T destinationPixel)
        {
            var path = new List<T>();
            path.Insert(0, destinationPixel);
            while (!previous[destinationPixel].Equals(destinationPixel))
            {
                destinationPixel = previous[destinationPixel];
                path.Insert(0, destinationPixel);
            }
            return path;
        }

        private T GetNodeWithSmallestDistance()
        {
            var distance = int.MaxValue;
            var smallest = new T();

            foreach (var n in basis.Where(n => distances[n] < distance))
            {
                distance = distances[n];
                smallest = n;
            }
            return smallest;
        }

        private IEnumerable<T> GetNeighbors(T n)
        {
            return (from e in Edges where e.Source.Equals(n) && basis.Contains(n) select e.Target).ToList();
        }

        private int GetDistanceBetween(T o, T d)
        {
            return Edges.Any(e => e.Source.Equals(o) && e.Target.Equals(d)) ? 1 : 0;
        }
    }
}