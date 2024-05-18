using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
namespace Coursera_Tests
{
    class Vertex
    {
        public int value;
        public List<Tuple<Vertex, long>> ForwardEdges;
        public List<Tuple<Vertex, long>> BackwardEdges;
        public Vertex(int value)
        {
            this.value = value;
            ForwardEdges = new List<Tuple<Vertex, long>>();
            BackwardEdges = new List<Tuple<Vertex, long>>();
        }
    }


    class Heap<TKey>
    {
        Dictionary<TKey, long> KeyToDistance;
        Dictionary<long, HashSet<TKey>> DistanceToKey;
        long MinimumDistance;
        long MaximumDistance;
        public int Count { get { return DistanceToKey.Count; } }
        public long this[TKey key]
        {
            get { return KeyToDistance[key]; }
            set
            {
                if (!Contains(key))
                    Add(key, value);
                else
                    Update(key, value);
            }
        }
        public Heap()
        {
            DistanceToKey = new Dictionary<long, HashSet<TKey>>();
            KeyToDistance = new Dictionary<TKey, long>();
            MinimumDistance = 0;
            MaximumDistance = long.MinValue;
        }
        public void Add(TKey key, long priority)
        {
            if(!DistanceToKey.ContainsKey(priority))
                DistanceToKey.Add(priority, new HashSet<TKey>());
            DistanceToKey[priority].Add(key);
            KeyToDistance[key] = priority;
            MaximumDistance = Math.Max(priority, MaximumDistance);
        }
        public void Update(TKey key, long priority)
        {
            Remove(key);
            Add(key, priority);
        }
        public void Remove(TKey key)
        {
            var priority = KeyToDistance[key];
            DistanceToKey[priority].Remove(key);
            if (DistanceToKey[priority].Count == 0)
                DistanceToKey.Remove(priority);
            KeyToDistance.Remove(key);
        }
        public KeyValuePair<TKey, long> ExtractMin()
        {
            for (long i = MinimumDistance; i <= MaximumDistance; i++)
                if (DistanceToKey.ContainsKey(i))
                    foreach (var item in DistanceToKey[i])
                    {
                        var ans = new KeyValuePair<TKey, long>(item, i);
                        MinimumDistance = i;
                        Remove(item);
                        return ans;
                    }
            throw new Exception("Empty");
        }
        public bool Contains(TKey Key)
        {
            return KeyToDistance.ContainsKey(Key);
        }
    }

    internal class Program
    {
        public static List<Vertex> Graph;
        public static long BidirectionalDijkstra(long start, long end)
        {
            if (start == end) return 0;
            int n = Graph.Count;
            HashSet<int> Proccessed = new HashSet<int>();
            HashSet<int> RProccessed = new HashSet<int>();
            Heap<int> PriorityQueue = new Heap<int>();
            Heap<int> RPriorityQueue = new Heap<int>();
            Dictionary<int, long> Distance = new Dictionary<int, long>();
            Dictionary<int, long> RDistance = new Dictionary<int, long>();
            Distance[(int)start] = 0;
            RDistance[(int)end] = 0;
            PriorityQueue[(int)start] = 0;
            RPriorityQueue[(int)end] = 0;
            long MinimumDistance = long.MaxValue;
            while (true)
            {
                var minimum = PriorityQueue.ExtractMin();
                var v = Graph[minimum.Key].value;
                Distance[v] = minimum.Value;
                var Edges = Graph[v].ForwardEdges;
                for (int i = 0; i < Edges.Count; i++)
                {
                    var u = Edges[i].Item1.value;
                    var weight = Edges[i].Item2;
                    if (Distance.ContainsKey(v) && Distance[v] != long.MaxValue && (!Distance.ContainsKey(u) || Distance[v] + weight < Distance[u]))
                    {
                        Distance[u] = Distance[v] + weight;
                        if (RProccessed.Contains(u) && RDistance[u] != long.MaxValue)
                        {
                            MinimumDistance = Math.Min(Distance[v] + weight + RDistance[u], MinimumDistance);
                        }
                        PriorityQueue[u] = Distance[u];
                    }
                }
                Proccessed.Add(v);
                if (PriorityQueue.Count == 0 || RProccessed.Contains(v))
                    break;

                minimum = RPriorityQueue.ExtractMin();
                v = Graph[minimum.Key].value;
                RDistance[v] = minimum.Value;
                Edges = Graph[v].BackwardEdges;
                for (int i = 0; i < Edges.Count; i++)
                {
                    var u = Edges[i].Item1.value;
                    var weight = Edges[i].Item2;
                    if (RDistance.ContainsKey(v) && RDistance[v] != long.MaxValue && (!RDistance.ContainsKey(u) || RDistance[v] + weight < RDistance[u]))
                    {
                        RDistance[u] = RDistance[v] + weight;
                        if (Proccessed.Contains(u) && Distance[u] != long.MaxValue)
                        {
                            MinimumDistance = Math.Min(RDistance[v] + weight + Distance[u], MinimumDistance);
                        }
                        RPriorityQueue[u] = RDistance[u];
                    }
                }
                RProccessed.Add(v);
                if (RPriorityQueue.Count == 0 || Proccessed.Contains(v))
                    break;
            }
            return MinimumDistance == long.MaxValue ? -1 : MinimumDistance;
        }

        public static List<long> Solve(long nodeCount, long[][] edges, long[,] queries)
        {
            Graph = new List<Vertex>();
            for (int i = 0; i < nodeCount; i++)
            {
                Graph.Add(new Vertex(i));
            }
            for (int i = 0; i < edges.Length; i++)
            {
                int u = (int)edges[i][0] - 1;
                int v = (int)edges[i][1] - 1;
                var weight = edges[i][2];
                Graph[u].ForwardEdges.Add(new Tuple<Vertex, long>(Graph[v], weight));
                Graph[v].BackwardEdges.Add(new Tuple<Vertex, long>(Graph[u], weight));
            }
            List<long> result = new List<long>();
            for (int i = 0; i < queries.Length / 2; i++)
            {
                long start = queries[i, 0] - 1;
                long end = queries[i, 1] - 1;
                result.Add(BidirectionalDijkstra(start, end));
            }
            return result;
        }
        public static void Main(string[] args)
        {
            string[] temp = Console.ReadLine().Split();
            long n = long.Parse(temp[0]);
            long m = long.Parse(temp[1]);
            long[][] edges = new long[m][];
            for (int i = 0; i < m; i++)
            {
                edges[i] = new long[3];
                temp = Console.ReadLine().Split();
                edges[i][0] = long.Parse(temp[0]);
                edges[i][1] = long.Parse(temp[1]);
                edges[i][2] = long.Parse(temp[2]);
            }
            long QueryCount = long.Parse(Console.ReadLine());
            long[,] Queries = new long[QueryCount, 2];
            for (int i = 0; i < QueryCount; i++)
            {
                temp = Console.ReadLine().Split();
                Queries[i, 0] = long.Parse(temp[0]);
                Queries[i, 1] = long.Parse(temp[1]);
            }
            Solve(n, edges, Queries).ForEach(x => Console.WriteLine(x));
        }
    }
}