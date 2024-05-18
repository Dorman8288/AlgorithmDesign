using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BW_Transform
{
    class Program
    {
        static string[] GetAllRotations(string text)
        {
            int n = text.Length;
            string[] result = new string[n];
            result[0] = text;
            for(int i = 1; i < n; i++)
                result[i] = text.Substring(i) + text.Substring(0, i);
            return result;
        }

        static string BMW_Transform(string text)
        {
            var rotations = GetAllRotations(text);
            Array.Sort(rotations);
            string result = "";
            foreach(var rotation in rotations)
                result += rotation[rotation.Length - 1];
            return result;
        }
        static char[] countSort(char[] arr)
        {
            Dictionary<char, int> quantity = new Dictionary<char, int>();
            quantity.Add('$', 0);
            for (char c = 'A'; c <= 'Z'; c++)
                quantity.Add(c, 0);
            foreach(var charachter in arr)
                quantity[charachter]++;
            List<char> ans = new List<char>();
            ans.Add('$');
            for(char c = 'A'; c <= 'Z'; c++)
                for(int i = 0; i < quantity[c]; i++)
                    ans.Add(c);
            return ans.ToArray();
        }

        static int[] GetConnections(char[] first, char[] last)
        {
            int n = first.Length;
            int[] connection = new int[n];
            Dictionary<Tuple<char, int>, int> indexes = new Dictionary<Tuple<char, int>, int>();
            Dictionary<char, int> currentQuantity = new Dictionary<char, int>();
            for(int i = 0; i < n; i++)
            {
                char charachter = last[i];
                if (!currentQuantity.ContainsKey(charachter)) currentQuantity.Add(charachter, 0);
                currentQuantity[charachter]++;
                indexes.Add(new Tuple<char, int>(charachter, currentQuantity[charachter]), i);
            }
            currentQuantity.Clear();
            for(int i = 0; i < n; i++)
            {
                char charachter = first[i];
                if (!currentQuantity.ContainsKey(charachter)) currentQuantity.Add(charachter, 0);
                currentQuantity[charachter]++;
                var quantity = currentQuantity[charachter];
                connection[i] = indexes[new Tuple<char, int>(charachter, quantity)];
            }
            return connection;
        }

        static string reverse_BMW(string text)
        {
            char[] lastcolumn = text.ToCharArray();
            char[] firstColumn = countSort(lastcolumn);
            int[] firstToLast = GetConnections(firstColumn, lastcolumn);
            StringBuilder builder = new StringBuilder();
            int current = 0;
            while (true)
            {
                int index = firstToLast[current];
                builder.Append(firstColumn[index]);
                if (firstColumn[index] == '$') break;
                current = index;
            }
            return builder.ToString();
        }
        static SortedList<string, int> getSuffixArray(string text)
        {
            int n = text.Length;
            SortedList<string, int> list = new SortedList<string, int>();
            StringBuilder builder = new StringBuilder();
            for(int i = n - 1; i >= 0; i--)
            {
                builder.Insert(0, text[i]);
                list.Add(builder.ToString(), i);
            }
            return list;
        }

        static Dictionary<char, int>[] GetCountArray(string s)
        {
            int n = s.Length + 1;
            Dictionary<char, int>[] CountArray = new Dictionary<char, int>[n];
            CountArray[0] = new Dictionary<char, int>();
            foreach (var character in s)
                if(!CountArray[0].ContainsKey(character))
                    CountArray[0].Add(character, 0);
            for(int i = 1; i < n; i++)
            {
                CountArray[i] = new Dictionary<char, int>();
                var CountArrayPrev = CountArray[i - 1];
                var CountArrayCurr = CountArray[i];
                foreach (var item in CountArrayPrev)
                    CountArrayCurr.Add(item.Key, item.Value);
                char charachter = s[i - 1];
                CountArrayCurr[charachter]++;
            }
            return CountArray;
        }
        static Dictionary<char, int> GetStartingPosition(string s)
        {
            int n = s.Length;
            var sorted = countSort(s.ToCharArray());
            Dictionary<char, int> StartingPosition = new Dictionary<char, int>();
            for(int i = 0; i < n; i++)
            {
                var charachter = sorted[i];
                if (!StartingPosition.ContainsKey(charachter))
                    StartingPosition.Add(charachter, i);
            }
            return StartingPosition;
        }
        static int Match(string BMWTransform, string pattern, Dictionary<char, int> StartingPositions, Dictionary<char, int>[] CountArray)
        {
            int n = BMWTransform.Length;
            int top = 0;
            int bottom = n - 1;
            int PatternIndex = pattern.Length - 1;
            while(top <= bottom)
            {
                if(PatternIndex >= 0)
                {
                    char character = pattern[PatternIndex];
                    if (!StartingPositions.ContainsKey(character))
                        return 0;
                    top = StartingPositions[character] + CountArray[top][character];
                    bottom = StartingPositions[character] + CountArray[bottom + 1][character] - 1;
                    PatternIndex--;
                }
                else
                    return bottom - top + 1;
            }
            return 0;
        }
        static void Main(string[] args)
        {
            string BMW = Console.ReadLine();
            var CountArray = GetCountArray(BMW);
            var StartingPositions = GetStartingPosition(BMW);
            int t = int.Parse(Console.ReadLine());
            var queries = Console.ReadLine().Split();
            List<int> result = new List<int>();
            for (int i = 0; i < t; i++)
                result.Add(Match(BMW, queries[i], StartingPositions, CountArray));
            result.ForEach(x => Console.Write(x + " "));
        }
    }
}
