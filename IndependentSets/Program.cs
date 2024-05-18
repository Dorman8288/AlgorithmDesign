using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunParty
{
    public class IndependentSet
    {
        public static void Main()
        {
            int n = int.Parse(Console.ReadLine());
            var Funs = Console.ReadLine().Split().Select(x => int.Parse(x)).ToArray();
            int[] Dists = new int[n];
            List<long>[] adj = new List<long>[n];
            for (int i = 0; i < n; i++)
            {
                adj[i] = new List<long>();
                Dists[i] = -1;
            }

            for (int i = 0; i < n - 1; i++)
            {
                var toks = Console.ReadLine().Split().Select(x => int.Parse(x)).ToArray();
                adj[toks[0] - 1].Add(toks[1] - 1);
                adj[toks[1] - 1].Add(toks[0] - 1);
            }
            //var Tree = BFS( adj , 1 , n ) ; 
            int MaxFun = FunParty(adj, 0, -1, Funs, Dists);
            Console.WriteLine(MaxFun);
        }


        private static int FunParty(List<long>[] tree, long vertex, long parent, int[] funs, int[] dists)
        {
            if (dists[vertex] == -1)
            {
                if (tree[vertex].Count == 0 && vertex != 0)
                    dists[vertex] = funs[vertex];
                else
                {
                    int m1 = funs[vertex];
                    foreach (var child in tree[vertex])
                    {
                        if (child != parent)
                        {
                            foreach (var grandChild in tree[child])
                            {
                                if (grandChild != vertex)
                                    m1 += FunParty(tree, grandChild, child, funs, dists);
                            }
                        }
                    }

                    int m2 = 0;
                    foreach (var ch in tree[vertex])
                    {
                        if (ch != parent)
                            m2 += FunParty(tree, ch, vertex, funs, dists);
                    }
                    dists[vertex] = Math.Max(m1, m2);
                }
            }
            return dists[vertex];
        }


        public static List<long>[] BFS(List<long>[] adj, long startnode, long NodeCount)
        {
            bool[] visited = new bool[NodeCount + 1];
            long[] pre = new long[NodeCount + 1];
            List<long>[] Tree = new List<long>[NodeCount + 1];
            for (int i = 0; i <= NodeCount; i++)
            {
                visited[i] = false;
                Tree[i] = new List<long>();
                pre[i] = 0;
            }

            Queue<long> queue = new Queue<long>();
            queue.Enqueue(startnode);
            visited[startnode] = true;
            while (queue.Count > 0)
            {
                var n = queue.Dequeue();
                foreach (var nn in adj[n])
                {
                    if (visited[nn] == false)
                    {
                        queue.Enqueue(nn);
                        visited[nn] = true;
                        pre[nn] = n;
                        Tree[n].Add(nn);
                    }
                }
            }
            return Tree;
        }

    }
}