using System;
using System.Linq;
using System.Collections.Generic;

namespace Clustering
{
    class Point
    {
        public int name;
        public int x;
        public int y;
        public Point(int name, int x, int y)
        {
            this.name = name;
            this.x = x;
            this.y = y;
        }
    }
    class Edge
    {
        public Point v;
        public Point u;
        public double weight;
        public Edge(Point v, Point u)
        {
            this.v = v;
            this.u = u;
            weight = GetDistance(v, u);
        }
        public static double GetDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
        }
    }
    class DisjointSet
    {
        List<int> parents = new List<int>();
        List<int> rank = new List<int>();
        public void MakeSet()
        {
            parents.Add(parents.Count);
            rank.Add(1);
        }
        public int Find(int index)
        {
            if (parents[index] != index)
                parents[index] = Find(parents[index]);
            return parents[index];
        }
        public void Union(int i, int j)
        {
            int Iroot = Find(i);
            int Jroot = Find(j);
            if (Iroot == Jroot)
                return;
            if (rank[Iroot] == rank[Jroot])
            {
                parents[Iroot] = Jroot;
                rank[Jroot]++;
            }
            if (rank[Iroot] < rank[Jroot])
            {
                parents[Iroot] = Jroot;
            }
            else
            {
                parents[Jroot] = Iroot;
            }
        }
        public void PrintParents()
        {
            parents.ToList().ForEach(x => Console.Write($"{x} "));
            Console.WriteLine();
        }
    }
    class Program
    {
        static int Kruskal(Point[] points, Edge[] Edges, double AllowedDistance)
        {
            int n = points.Length;
            int m = Edges.Length;
            DisjointSet set = new DisjointSet();
            for (int i = 0; i < n; i++)
                set.MakeSet();
            foreach(var edge in Edges)
            {
                var Vroot = set.Find(edge.v.name);
                var Uroot = set.Find(edge.u.name);
                if(Vroot != Uroot && edge.weight < AllowedDistance)
                    set.Union(Vroot, Uroot);
            }

            HashSet<int> GroupRoots = new HashSet<int>();
            for (int i = 0; i < n; i++)
                GroupRoots.Add(set.Find(points[i].name));
            return GroupRoots.Count;
        }

        static double Clustering(Point[] points, int K)
        {
            int n = points.Length;
            List<Edge> EdgesList = new List<Edge>();
            for (int i = 0; i < n - 1; i++)
                for (int j = 1; j < n; j++)
                    EdgesList.Add(new Edge(points[i], points[j]));
            Edge[] Edges = EdgesList.ToArray();
            Array.Sort(Edges, (x, y) => x.weight.CompareTo(y.weight));

            double left = 0;
            double right = 3000;
            double MaximumDistance;
            while (true)
            {
                double mid = (left + right) / 2;
                if (right - left < Math.Pow(10, -8))
                {
                    MaximumDistance = (left + right) / 2;
                    break;
                }
                int CurrentK = Kruskal(points, Edges, mid);
                if(CurrentK < K)
                    right = mid;
                else
                    left = mid;
            }
            return MaximumDistance;
        }
        static void Main(string[] args)
        {
            int n = int.Parse(Console.ReadLine());
            Point[] points = new Point[n];
            for(int i = 0; i < n; i++)
            {
                var buffer = Console.ReadLine().Split();
                var x = int.Parse(buffer[0]);
                var y = int.Parse(buffer[1]);
                points[i] = new Point(i, x, y);
            }
            int K = int.Parse(Console.ReadLine());
            Console.WriteLine(Clustering(points, K));
        }
    }
}
