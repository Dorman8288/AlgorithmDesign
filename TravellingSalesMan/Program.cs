using System;
using System.Collections.Generic;
using System.Linq;

namespace TravellingSalesMan
{
    class Program
    {

        public static List<long> GetAllXSubsets(long x, long n)
        {
            if(x == 0)
            {
                List<long> ans = new List<long>();
                ans.Add(0);
                return ans;
            }
            if (x > n)
                return new List<long>();
            var Subsets = GetAllXSubsets(x - 1, n - 1).Select(y => (y << 1) + 1).ToList();
            var temp = GetAllXSubsets(x, n - 1).Select(y => y << 1).ToList();
            Subsets.AddRange(temp);
            return Subsets;
        }

        public static List<long> AssemblePath(Dictionary<Tuple<long, long>, Tuple<long, long>> prev, Tuple<long, long> end, long n)
        {
            var current = end;
            List<long> path = new List<long>();
            while (true)
            {
                if (current == null)
                    break;
                path.Add(current.Item2);
                current = prev[current];
            }
            path.Reverse();
            return path;
        }
        public static Tuple<long, List<long>> TravellingSalesman(long[][] Graph)
        {
            long n = Graph.Length;
            if(n == 2)
                return new Tuple<long, List<long>>(-1, new List<long>());
            Dictionary<Tuple<long, long>, long> DP = new Dictionary<Tuple<long, long>, long>();
            Dictionary<Tuple<long, long>, Tuple<long, long>> prev = new Dictionary<Tuple<long, long>, Tuple<long, long>>();
            for(int i = 1; i < n; i++)
            {
                if (Graph[0][i] > 0)
                    DP[new Tuple<long, long>(1L << (i - 1), i - 1)] = Graph[0][i];
                    prev[new Tuple<long, long>(1L << (i - 1), i - 1)] = null;
            }
            for(int size = 2; size <= n - 1; size++)
            {
                var Subsets = GetAllXSubsets(size, n - 1);
                for(int i = 0; i < Subsets.Count; i++)
                {
                    for(int j = 0; j < n - 1; j++)
                    {
                        if(((Subsets[i] & (1L << j)) >> j) % 2 == 1)
                        {
                            long Update = long.MaxValue;
                            Tuple<long, long> parent = null;
                            long SubsetWithoutJ = Subsets[i] ^ (1L << j);
                            if (SubsetWithoutJ == 0)
                                Update = 0;
                            for (int k = 0; k < n - 1; k++)
                            {
                                var key = new Tuple<long, long>(SubsetWithoutJ, k);
                                if (((Subsets[i] & (1L << k)) >> k) % 2 == 1 && Graph[j + 1][k + 1] != 0 && DP.ContainsKey(key) && DP[key] != long.MaxValue)
                                    if(DP[key] + Graph[j + 1][k + 1] <= Update)
                                    {
                                        Update = DP[key] + Graph[j + 1][k + 1];
                                        parent = key;
                                    }
                            }
                            DP[new Tuple<long, long>(Subsets[i], j)] = Update;
                            prev[new Tuple<long, long>(Subsets[i], j)] = parent;
                        }
                    }
                }
            }
            long ans = long.MaxValue;
            Tuple<long, long> last = null;
            for(int i = 0; i < n - 1; i++)
            {
                long subset = (long)(Math.Pow(2, n - 1) - 1);
                if (Graph[i + 1][0] != 0 && DP[new Tuple<long, long>(subset, i)] != long.MaxValue)
                {
                    var candidate = DP[new Tuple<long, long>(subset, i)] + Graph[i + 1][0];
                    if(candidate < ans)
                    {
                        ans = candidate;
                        last = new Tuple<long, long>(subset, i);
                    }
                }
            }
            if (ans == long.MaxValue)
                return new Tuple<long, List<long>>(-1, new List<long>());
            List<long> path = AssemblePath(prev, last, n - 1);
            return new Tuple<long, List<long>>(ans, path);
        }

        static void Main(string[] args)
        {
            var buffer = Console.ReadLine().Split();
            long n = long.Parse(buffer[0]);
            long m = long.Parse(buffer[1]);
            long[][] Graph = new long[n][];
            for (int i = 0; i < n; i++)
                Graph[i] = new long[n];
            for(int i = 0; i < m; i++)
            {
                buffer = Console.ReadLine().Split();
                long v = long.Parse(buffer[0]) - 1;
                long u = long.Parse(buffer[1]) - 1;
                long w = long.Parse(buffer[2]);
                Graph[u][v] = w;
                Graph[v][u] = w;
            }
            var ans = TravellingSalesman(Graph);
            Console.WriteLine(ans.Item1);
            if(ans.Item1 != -1)
                Console.Write(1 + " ");
            ans.Item2.ForEach(x => Console.Write(x + 2 + " "));
        }
    }
}
