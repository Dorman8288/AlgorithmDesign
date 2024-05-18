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
        static void Main(string[] args)
        {
            string text = Console.ReadLine();
            getSuffixArray(text).ToList().ForEach(x => Console.Write(x.Value + " "));
        }
    }
}
