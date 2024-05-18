using System;
using System.Linq;
using System.Collections.Generic;
namespace BMW_Matching
{
    class Program
    {

        public static int[] CountSortCharachters(char[] charachters)
        {
            int n = charachters.Length;
            int[] order = new int[n];
            int[] count = new int['Z' - 'A' + 1];
            foreach (var c in charachters)
                if(c != '$')
                    count[c - 'A']++;
            for(int i = 1; i < 'Z' - 'A' + 1; i++)
                count[i] += count[i - 1];
            order[0] = n - 1;
            for(int i = n - 2; i >= 0; i--)
            {
                var c = charachters[i];
                var index = c - 'A';
                count[index]--;
                order[count[index] + 1] = i;
            }
            return order;
        }

        public static int[] GetEquivalence(string text, int[] order)
        {
            int n = text.Length;
            int[] Equivalence = new int[n];
            Equivalence[0] = 0;
            for (int i = 1; i < n; i++)
            {
                var indexPrev = order[i - 1];
                var indexCurrent = order[i];
                if (text[indexCurrent] == text[indexPrev])
                    Equivalence[indexCurrent] = Equivalence[indexPrev];
                else
                    Equivalence[indexCurrent] = Equivalence[indexPrev] + 1;
            }
            return Equivalence;
        }

        public static int[] DoubleSort(int[] order, int[] EquivalenceClass, int L)
        {
            int n = order.Length;
            int[] newOrder = new int[n];
            int[] Count = new int[n];
            for (int i = 0; i < n; i++)
            {
                order[i] = (order[i] - L + n) % n;
                int Equivalence = EquivalenceClass[order[i]];
                Count[Equivalence]++;
            }
            for (int i = 1; i < n; i++)
                Count[i] += Count[i - 1];
            for(int i = n - 1; i >= 0; i--)
            {
                int Suffix = order[i];
                int Equivalence = EquivalenceClass[Suffix];
                Count[Equivalence]--;
                int newIndex = Count[Equivalence];
                newOrder[newIndex] = order[i];
            }
            return newOrder;
        }

        public static int[] UpdateEquivalence(int[] order, int[] OldEquivalence, int L)
        {
            int n = order.Length;
            int[] EquivalenceClass = new int[n];
            EquivalenceClass[order[0]] = 0;
            for(int i = 1; i < n; i++)
            {
                var Prev = order[i - 1];
                var PrevFirst = OldEquivalence[Prev];
                var PrevSecond = OldEquivalence[(Prev + L) % n];
                var Current = order[i];
                var CurrentFirst = OldEquivalence[Current];
                var CurrentSecond = OldEquivalence[(Current + L) % n];
                if (CurrentFirst == PrevFirst && CurrentSecond == PrevSecond)
                    EquivalenceClass[Current] = EquivalenceClass[Prev];
                else
                    EquivalenceClass[Current] = EquivalenceClass[Prev] + 1;
            }
            return EquivalenceClass;
        }

        public static int[] GetSuffixArray(string text)
        {
            int n = text.Length;
            int[] order = CountSortCharachters(text.ToCharArray());
            int[] EquivalenceClass = GetEquivalence(text, order);
            int L = 1;
            while(L < n)
            {
                order = DoubleSort(order, EquivalenceClass, L);
                EquivalenceClass = UpdateEquivalence(order, EquivalenceClass, L);
                L *= 2;
            }
            return order;
        }

        public static Tuple<int, int> FindRange(string text, string pattern, int[] SuffixArray)
        {
            int n = text.Length;
            int P = pattern.Length;
            int Left = 0;
            int Right = n;
            while (true)
            {
                int mid = (Left + Right) / 2;
                int MidSuffix = SuffixArray[mid];
                string MidValue = text.Substring(MidSuffix, Math.Min(P, n - MidSuffix));
                if (Right - Left == 1)
                {
                    if (Right == n)
                        return null;
                    int start = Right;
                    int end = n;
                    int startSuffix = SuffixArray[start];
                    if (pattern.CompareTo(text.Substring(startSuffix, Math.Min(P, n - startSuffix))) != 0)
                        return null;
                    Left = Right;
                    Right = n;
                    while (true)
                    {
                        mid = (Left + Right) / 2;
                        MidSuffix = SuffixArray[mid];
                        MidValue = text.Substring(MidSuffix, Math.Min(P, n - MidSuffix));
                        if(Right - Left == 1)
                        {
                            end = Left;
                            return new Tuple<int, int>(start, end);
                        }
                        if (pattern.CompareTo(MidValue) < 0)
                            Right = mid;
                        else
                            Left = mid;
                    }
                }
                if(pattern.CompareTo(MidValue) <= 0)
                    Right = mid;
                else
                    Left = mid;
            }
        }

        public static int[] Match(string text, string[] patterns)
        {
            int n = text.Length;
            int m = patterns.Length;
            int[] SuffixArray = GetSuffixArray(text);
            HashSet<int> result = new HashSet<int>();
            foreach(var pattern in patterns)
            {
                var range = FindRange(text, pattern, SuffixArray);
                if(range != null)
                    for (int i = range.Item1; i <= range.Item2; i++)
                        result.Add(SuffixArray[i]);
            }
            return result.ToArray();
        }


        static void Main(string[] args)
        {
            string text = Console.ReadLine();
            Console.ReadLine();
            string[] patterns = Console.ReadLine().Split();
            Match(text + '$', patterns).ToList().ForEach(x => Console.Write(x + " "));
        }
    }
}
