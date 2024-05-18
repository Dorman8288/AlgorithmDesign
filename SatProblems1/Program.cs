using System;
using System.Collections.Generic;
using System.Text;

namespace SatProblems1
{

    class SAT
    {
        public int VariableCount;
        public List<int[]> Clauses;
        public SAT(int VariableCount)
        {
            this.VariableCount = VariableCount;
            Clauses = new List<int[]>();
        }
        public void AddClause(params int[] Clause)
        {
            Clauses.Add(Clause);
        }
        public void AddExactlyOneOf(params int[] variables)
        {
            AddClause(variables);
            for(int i = 0; i < variables.Length - 1; i++)
            {
                for(int j = i + 1; j < variables.Length; j++)
                {
                    AddClause(-variables[i], -variables[j]);
                }
            }
        }
        public void OutPut()
        {
            Console.WriteLine(Clauses.Count + " " + VariableCount);
            foreach(var clause in Clauses)
            {
                foreach (var item in clause)
                    Console.Write(item + " ");
                Console.Write("0\n");
            }
        }
    }

    class Program
    {
        public static SAT ConvertGraph1ToSAT(int n, List<Tuple<int, int>> Edges)
        {
            SAT result = new SAT(3 * n);
            for(int i = 0; i < n; i++)
            {
                int IndexInSAT = (i * 3) + 1;
                int X1 = IndexInSAT;
                int X2 = IndexInSAT + 1;
                int X3 = IndexInSAT + 2;
                result.AddClause(X1, X2, X3);
                result.AddClause(-X1, -X2);
                result.AddClause(-X1, -X3);
                result.AddClause(-X2, -X3);
                //result.AddClause(X1, X2, X3);
                //result.AddClause(X1, -X2, -X3);
                //result.AddClause(-X1, X2, -X3);
                //result.AddClause(-X1, -X2, X3);
                //result.AddClause(-X1, -X2, -X3);
            }
            foreach(var edge in Edges)
            {
                int u = (edge.Item1 * 3) + 1;
                int v = (edge.Item2 * 3) + 1;
                int U1 = u;
                int U2 = u + 1;
                int U3 = u + 2;
                int V1 = v;
                int V2 = v + 1;
                int V3 = v + 2;
                result.AddClause(-U1, -V1);
                result.AddClause(-U2, -V2);
                result.AddClause(-U3, -V3);
            }
            return result;
        }
        public static SAT ConvertGraph2ToSAT(int n, List<int>[] Graph)
        {
            SAT result = new SAT(n * n);
            Func<int, int, int> IndexConvert = (i, j) => (1 + (n * i) + j);
            for (int i = 0; i < n; i++)
            {
                int[] indexes = new int[n];
                for(int j = 0; j < n; j++)
                    indexes[j] = IndexConvert(i, j);
                result.AddExactlyOneOf(indexes);
            }
            for (int j = 0; j < n; j++)
            {
                int[] indexes = new int[n];
                for (int i = 0; i < n; i++)
                    indexes[i] = IndexConvert(i, j);
                result.AddExactlyOneOf(indexes);
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (!Graph[i].Contains(j))
                    {
                        for (int k = 0; k < n - 1; k++)
                        {
                            var index1 = IndexConvert(i, k);
                            var index2 = IndexConvert(j, k + 1);
                            result.AddClause(-index1, -index2);
                        }
                    }
                }
            }
            return result;
        }
        public static void AddInequality(int[] Inequality, SAT sat)
        { 
            int n = Inequality.Length - 1;
            int RightSide = Inequality[n];
            List<Tuple<int, int>> NonZeroCoefficients = new List<Tuple<int, int>>();
            for(int i = 0; i < n; i++)
                if (Inequality[i] != 0)
                    NonZeroCoefficients.Add(new Tuple<int, int>(i + 1, Inequality[i]));
            switch(NonZeroCoefficients.Count)
            {
                case 1:
                    {
                        var a = NonZeroCoefficients[0];
                        if (a.Item2 > RightSide)
                            sat.AddClause(-a.Item1);
                        if (0 > RightSide)
                            sat.AddClause(a.Item1);
                        break;
                    }
                case 2:
                    {
                        var a = NonZeroCoefficients[0];
                        var b = NonZeroCoefficients[1];
                        if(0 > RightSide)
                            sat.AddClause(a.Item1, b.Item1);
                        if (a.Item2 > RightSide)
                            sat.AddClause(-a.Item1, b.Item1);
                        if (b.Item2 > RightSide)
                            sat.AddClause(a.Item1, -b.Item1);
                        if (a.Item2 + b.Item2 > RightSide)
                            sat.AddClause(-a.Item1, -b.Item1);
                        break;
                    }
                case 3:
                    {
                        var a = NonZeroCoefficients[0];
                        var b = NonZeroCoefficients[1];
                        var c = NonZeroCoefficients[2];
                        if (0 > RightSide)
                            sat.AddClause(a.Item1, b.Item1, c.Item1);
                        if (a.Item2 > RightSide)
                            sat.AddClause(-a.Item1, b.Item1, c.Item1);
                        if (b.Item2 > RightSide)
                            sat.AddClause(a.Item1, -b.Item1, c.Item1);
                        if (c.Item2 > RightSide)
                            sat.AddClause(a.Item1, b.Item1, -c.Item1);
                        if (a.Item2 + b.Item2 > RightSide)
                            sat.AddClause(-a.Item1, -b.Item1, c.Item1);
                        if (a.Item2 + c.Item2 > RightSide)
                            sat.AddClause(-a.Item1, b.Item1, -c.Item1);
                        if (b.Item2 + c.Item2 > RightSide)
                            sat.AddClause(a.Item1, -b.Item1, -c.Item1);
                        if (a.Item2 + b.Item2 + c.Item2 > RightSide)
                            sat.AddClause(-a.Item1, -b.Item1, -c.Item1);
                        break;
                    }
                default:
                    break;
            }

        }
        public static SAT ConvertLPToSat(int[][] LP)
        {
            int n = LP[0].Length - 1;
            SAT sat = new SAT(n);
            foreach(var Inequality in LP)
                AddInequality(Inequality, sat);
            if (sat.Clauses.Count == 0)
                sat.AddClause(1);
            return sat;
        }
        static void Main(string[] args)
        {
            var buffer = Console.ReadLine().Split();
            int n = int.Parse(buffer[0]);
            int m = int.Parse(buffer[1]);
            var Graph = new List<int>[n];
            for (int i = 0; i < n; i++)
                Graph[i] = new List<int>();
            for(int i = 0; i < m; i++)
            {
                buffer = Console.ReadLine().Split();
                int v = int.Parse(buffer[0]) - 1;
                int u = int.Parse(buffer[1]) - 1;
                Graph[v].Add(u);
                Graph[u].Add(v);
            }
            var sat = ConvertGraph2ToSAT(n, Graph);
            sat.OutPut();
            //var buffer = Console.ReadLine().Split();
            //int n = int.Parse(buffer[0]);
            //int m = int.Parse(buffer[1]);
            //int[][] LP = new int[n][];
            //for (int i = 0; i < n; i++)
            //    LP[i] = new int[m + 1];
            //for(int i = 0; i < n; i++)
            //{
            //    buffer = Console.ReadLine().Split();
            //    for(int j = 0; j < m; j++)
            //        LP[i][j] = int.Parse(buffer[j]);
            //}
            //buffer = Console.ReadLine().Split();
            //for(int i = 0; i < n; i++)
            //    LP[i][m] = int.Parse(buffer[i]);
            //var sat = ConvertLPToSat(LP);
            //sat.OutPut();
        }
    }
}
