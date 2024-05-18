using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
namespace Trie
{
    class Trie
    {
        public List<List<Tuple<int, char>>> Tree;
        public Trie()
        {
            Tree = new List<List<Tuple<int, char>>>();
            Tree.Add(new List<Tuple<int, char>>());
        }
        void AddPatternToNode(string Pattern, int node)
        {
            int index = 0;
            while(index < Pattern.Length)
            {
                Tree.Add(new List<Tuple<int, char>>());
                var newNodeIndex = Tree.Count - 1;
                var newEdgeCharachter = Pattern[index];
                Tree[node].Add(new Tuple<int, char>(newNodeIndex, newEdgeCharachter));
                node = newNodeIndex;
                index++;
            }
        }
        public void AddPattern(string pattern)
        {
            pattern += "$";
            int currentNode = 0;
            int currentPatternIndex = 0;
            while (true)
            {
                bool updated = false;
                for(int i = 0; i < Tree[currentNode].Count; i++)
                {
                    var u = Tree[currentNode][i].Item1;
                    var EdgeCharachter = Tree[currentNode][i].Item2;
                    var PatternChrachter = pattern[currentPatternIndex];
                    if (EdgeCharachter == PatternChrachter)
                    {
                        currentNode = u;
                        currentPatternIndex++;
                        updated = true;
                    }
                }
                if (updated == true) continue;
                AddPatternToNode(pattern.Substring(currentPatternIndex), currentNode);
                break;
            }
        }
        public bool IsMatch(string text, int StartingIndex)
        {
            text += '|';
            var currentNode = 0;
            var currentIndex = StartingIndex;
            while (true)
            {
                if (Tree[currentNode].Count == 0)
                    return true;
                bool Updated = false;
                for(int i = 0; i < Tree[currentNode].Count; i++)
                {
                    var EdgeCharachter = Tree[currentNode][i].Item2;
                    if (EdgeCharachter == '$')
                        return true;
                }
                for(int i = 0; i < Tree[currentNode].Count; i++)
                {
                    var NextNode = Tree[currentNode][i].Item1;
                    var EdgeCharachter = Tree[currentNode][i].Item2;
                    var TextCharachter = text[currentIndex];
                    if(TextCharachter == EdgeCharachter)
                    {
                        currentNode = NextNode;
                        currentIndex++;
                        Updated = true;
                        break;
                    }
                }
                if (Updated == false)
                    return false;
            }
        }
        public List<string> GetStringFormat()
        {
            List<string> result = new List<string>();
            for(int i = 0; i < Tree.Count; i++)
            {
                for(int j = 0; j < Tree[i].Count; j++)
                {
                    var v = i;
                    var u = Tree[i][j].Item1;
                    var charachter = Tree[i][j].Item2;
                    result.Add(v + "->" + u + ":" + charachter);
                }
            }
            return result;
        }
    }

    class Program
    {
        public static List<int> Solve(string text, string[] patterns)
        {
            Trie trie = new Trie();
            foreach (var pattern in patterns)
                trie.AddPattern(pattern);
            List<int> result = new List<int>();
            for(int i = 0; i < text.Length; i++)
                if (trie.IsMatch(text, i))
                    result.Add(i);
            return result;
        }
        static void Main(string[] args)
        {
            string text = Console.ReadLine();
            int n = int.Parse(Console.ReadLine());
            string[] patterns = new string[n];
            for (int i = 0; i < n; i++)
                patterns[i] = Console.ReadLine();
            Solve(text, patterns).ForEach(x => Console.Write(x.ToString() + " "));
        }
    }
}
