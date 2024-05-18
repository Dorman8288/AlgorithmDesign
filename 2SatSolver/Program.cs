using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System;
using System.Linq;
using System.Collections.Generic;

namespace _2SatSolver
{
    internal class Program
    {
        public static List<int>[] GetReverseGraph(List<int>[] Graph)
        {
            int n = Graph.Length;
            List<int>[] RevereseGraph = new List<int>[n];
            for (int i = 0; i < n; i++)
                RevereseGraph[i] = new List<int>();
            for (int u = 0; u < n; u++)
                foreach (var v in Graph[u])
                    RevereseGraph[v].Add(u);
            return RevereseGraph;
        }
        public static void DFSForSinks(int x, List<int>[] Graph, List<int> sinks, bool[] mark)
        {
            mark[x] = true;
            for (int i = 0; i < Graph[x].Count; i++)
            {
                var v = Graph[x][i];
                if (!mark[v])
                    DFSForSinks(v, Graph, sinks, mark);
            }
            sinks.Add(x);
        }
        public static List<int> GetSinks(List<int>[] Graph)
        {
            int n = Graph.Length;
            List<int> sinks = new List<int>();
            bool[] mark = new bool[n];
            for (int i = 0; i < n; i++)
                if (!mark[i])
                    DFSForSinks(i, Graph, sinks, mark);
            sinks.Reverse();
            return sinks;
        }
        public static void DFSForSCCs(int x, List<int>[] Graph, HashSet<int> SCC, bool[] mark)
        {
            mark[x] = true;
            SCC.Add(x);
            for (int i = 0; i < Graph[x].Count; i++)
            {
                var v = Graph[x][i];
                if (!mark[v])
                    DFSForSCCs(v, Graph, SCC, mark);
            }
        }
        public static List<HashSet<int>> GetAllSCCsOfGraph(List<int>[] Graph)
        {
            int n = Graph.Length;
            var ReverseGraph = GetReverseGraph(Graph);
            var Sinks = GetSinks(ReverseGraph);
            List<HashSet<int>> SCCs = new List<HashSet<int>>();
            bool[] mark = new bool[n];
            foreach (var v in Sinks)
            {
                if (!mark[v])
                {
                    var SCC = new HashSet<int>();
                    SCCs.Add(SCC);
                    DFSForSCCs(v, Graph, SCC, mark);
                }
            }
            return SCCs;
        }
        public static int SatToGraph(int Sat, int V)
        {
            return Sat > 0 ? Sat + V - 1 : Sat + V;
        }
        public static int GraphToSat(int Graph, int V)
        {
            return Graph >= V ? Graph - V + 1 : Graph - V;
        }
        public static List<int>[] GetImpelicationGraph(Tuple<int, int>[] Clauses, int V)
        {
            int n = 2 * V;
            List<int>[] ImplicationGraph = new List<int>[n];
            for (int i = 0; i < n; i++)
                ImplicationGraph[i] = new List<int>();
            foreach (var clause in Clauses)
            {
                var Sat1 = clause.Item1;
                var Sat2 = clause.Item2;
                ImplicationGraph[SatToGraph(-Sat1, V)].Add(SatToGraph(Sat2, V));
                ImplicationGraph[SatToGraph(-Sat2, V)].Add(SatToGraph(Sat1, V));
            }
            return ImplicationGraph;
        }

        public static List<int> GetCorrespondingAssignment(int[] assignment)
        {
            List<int> result = new List<int>();
            for (int i = 1; i < assignment.Length; i++)
            {
                result.Add(i * assignment[i]);
            }
            return result;
        }

        public static List<int> Solve2Sat(Tuple<int, int>[] Clauses, int V)
        {
            var ImplicationGraph = GetImpelicationGraph(Clauses, V);
            var SCCs = GetAllSCCsOfGraph(ImplicationGraph);
            int[] assignment = new int[V + 1];
            bool UNSATISFYBLE = false;
            foreach (var SCC in SCCs)
            {
                foreach (var u in SCC)
                {
                    var Sat = GraphToSat(u, V);
                    if (SCC.Contains(SatToGraph(-Sat, V)))
                    {
                        UNSATISFYBLE = true;
                        break;
                    }
                    if (assignment[Math.Abs(Sat)] == 0)
                        if (Sat > 0)
                            assignment[Sat] = 1;
                        else
                            assignment[-Sat] = -1;
                }
                if (UNSATISFYBLE)
                    break;
            }
            if (UNSATISFYBLE)
                return null;
            else
                return GetCorrespondingAssignment(assignment);
        }

        public static string Three_Recoloring(string OriginalColors, List<Tuple<int, int>> Edges, int n)
        {
            List<Tuple<int, int>> Clauses = new List<Tuple<int, int>>();
            int V = 3 * n;
            for(int i = 1; i <= n; i++)
            {
                int Red = (i - 1) * 3 + 1;
                int Green = (i - 1) * 3 + 2;
                int Blue = (i - 1) * 3 + 3;
                if (OriginalColors[i - 1] == 'R')
                {
                    Clauses.Add(new Tuple<int, int>(Green, Blue));
                    Clauses.Add(new Tuple<int, int>(-Blue, -Green));
                    Clauses.Add(new Tuple<int, int>(-Red, -Red));
                }
                if (OriginalColors[i - 1] == 'G')
                {
                    Clauses.Add(new Tuple<int, int>(Red, Blue));
                    Clauses.Add(new Tuple<int, int>(-Blue, -Red));
                    Clauses.Add(new Tuple<int, int>(-Green, -Green));
                }
                if (OriginalColors[i - 1] == 'B')
                {
                    Clauses.Add(new Tuple<int, int>(Green, Red));
                    Clauses.Add(new Tuple<int, int>(-Blue, -Red));
                    Clauses.Add(new Tuple<int, int>(-Blue, -Blue));
                }
            }
            foreach(var Edge in Edges)
            {
                int v = Edge.Item1;
                int u = Edge.Item2;
                Clauses.Add(new Tuple<int, int>(-((v - 1) * 3 + 1), -((u - 1) * 3 + 1)));
                Clauses.Add(new Tuple<int, int>(-((v - 1) * 3 + 2), -((u - 1) * 3 + 2)));
                Clauses.Add(new Tuple<int, int>(-((v - 1) * 3 + 3), -((u - 1) * 3 + 3)));
            }
            var Assignment = Solve2Sat(Clauses.ToArray(), V);
            string ans = "Impossible";
            if (Assignment != null)
            {
                ans = "";
                for(int i = 0; i < Assignment.Count; i++)
                    if (Assignment[i] > 0)
                        switch (Assignment[i] % 3)
                        {
                            case 1:
                                ans += 'R';
                                break;
                            case 2:
                                ans += 'G';
                                break;
                            case 0:
                                ans += 'B';
                                break;
                        }
            }
            return ans;
        }

        static void Main(string[] args)
        {
            //var buffer = Console.ReadLine().Split();
            //int V = int.Parse(buffer[0]);
            //int C = int.Parse(buffer[1]);
            //Tuple<int, int>[] Clauses = new Tuple<int, int>[C];
            //for (int i = 0; i < C; i++)
            //{
            //    buffer = Console.ReadLine().Split();
            //    int sat1 = int.Parse(buffer[0]);
            //    int sat2 = int.Parse(buffer[1]);
            //    Clauses[i] = new Tuple<int, int>(sat1, sat2);
            //}
            //var assignments = Solve2Sat(Clauses, V);
            //if (assignments == null)
            //    Console.WriteLine("UNSATISFIABLE");
            //else
            //{
            //    Console.WriteLine("SATISFIABLE");
            //    assignments.ForEach(x => Console.Write(x + " "));
            //}
            var buffer = Console.ReadLine().Split();
            int n = int.Parse(buffer[0]);
            int m = int.Parse(buffer[1]);
            string OC = Console.ReadLine();
            List<Tuple<int, int>> Edges = new List<Tuple<int, int>>();
            for(int i = 0; i < m; i++)
            {
                buffer = Console.ReadLine().Split();
                int v = int.Parse(buffer[0]);
                int u = int.Parse(buffer[1]);
                Edges.Add(new Tuple<int, int>(v, u));
            }
            Console.WriteLine(Three_Recoloring(OC, Edges, n));
        }
    }
}