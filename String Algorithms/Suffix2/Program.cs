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
namespace SuffixTree
{

    class Node
    {
        public int index;
        public List<Tuple<Node, int, int>> Edges;
        public Node(int index = -1)
        {
            this.index = index;
            Edges = new List<Tuple<Node, int, int>>();
        }
    }
    class SuffixTree
    {
        string text;
        Node Root;
        public SuffixTree(string text)
        {
            this.text = text;
            Root = new Node();
            for (int i = 0; i < this.text.Length; i++)
            {
                AddFromIndex(i);
            }
        }

        public string GetShortestNonSharedSubString()
        {
            List<string> Candidates = new List<string>();
            GetCandidates(Root, "", Candidates);
            string ans = "";
            foreach(var candidate in Candidates)
            {
                if (candidate.Length < ans.Length || ans == "")
                    ans = candidate;
            }
            return ans;
        }

        public bool GetCandidates(Node x, string back, List<string> Candidates)
        {
            bool SpecialCase = true;
            for(int i = 0; i < x.Edges.Count; i++)
            {
                var u = x.Edges[i].Item1;
                var start = x.Edges[i].Item2;
                var Length = x.Edges[i].Item3;
                if(Length > text.Length / 2 && text[start] != '#')
                    Candidates.Add(back + text[start]);
                if (Length < text.Length / 2 && text[start + Length - 1] == '$')
                    SpecialCase = false;
                SpecialCase &= GetCandidates(u, back + text.Substring(start, Length), Candidates);
            }
            if (SpecialCase && x.Edges.Count != 0)
                Candidates.Add(back);
            return SpecialCase;
        }
        public void SplitEdge(Node head, int EdgeNumber, int targetIndex, int finalIndex)
        {
            Node v = head.Edges[EdgeNumber].Item1;
            int start = head.Edges[EdgeNumber].Item2;
            int length = head.Edges[EdgeNumber].Item3;
            head.Edges.RemoveAt(EdgeNumber);
            int splitLength = 0;
            while (text[targetIndex + splitLength] == text[start + splitLength])
                splitLength++;
            Node Middle = new Node();
            Node newNode = new Node(finalIndex);
            Middle.Edges.Add(new Tuple<Node, int, int>(v, start + splitLength, length - splitLength));
            Middle.Edges.Add(new Tuple<Node, int, int>(newNode, targetIndex + splitLength, text.Length - splitLength - targetIndex));
            head.Edges.Add(new Tuple<Node, int, int>(Middle, start, splitLength));
        }

        public void AddFromIndex(int index)
        {
            var currentIndex = index;
            Node currentNode = Root;
            while (true)
            {
                bool updated = false;
                for (int i = 0; i < currentNode.Edges.Count; i++)
                {
                    var v = currentNode.Edges[i].Item1;
                    var start = currentNode.Edges[i].Item2;
                    var length = currentNode.Edges[i].Item3;
                    if (text.Length - currentIndex >= length && text.Substring(start, length) == text.Substring(currentIndex, length))
                    {
                        currentIndex += length;
                        currentNode = v;
                        updated = true;
                        break;
                    }
                }
                if (updated)
                    continue;
                int mark = -1;
                for (int i = 0; i < currentNode.Edges.Count; i++)
                {
                    var start = currentNode.Edges[i].Item2;
                    if (text[start] == text[currentIndex])
                    {
                        mark = i;
                        break;
                    }
                }
                if (mark == -1)
                {
                    var newNode = new Node(index);
                    var length = text.Length - currentIndex;
                    currentNode.Edges.Add(new Tuple<Node, int, int>(newNode, currentIndex, length));
                }
                else
                {
                    SplitEdge(currentNode, mark, currentIndex, index);
                }
                break;
            }
        }
        public List<string> GetAllEdgeLabels()
        {
            List<string> result = new List<string>();
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(Root);
            while (queue.Count != 0)
            {
                var currentNode = queue.Dequeue();
                for (int i = 0; i < currentNode.Edges.Count; i++)
                {
                    var u = currentNode.Edges[i].Item1;
                    var start = currentNode.Edges[i].Item2;
                    var length = currentNode.Edges[i].Item3;
                    queue.Enqueue(u);
                    result.Add(text.Substring(start, length));
                }
            }
            return result;
        }
    }

    class Program
    {
        static string Solve(string text1, string text2)
        {
            SuffixTree tree = new SuffixTree(text1 + "#" + text2 + "$");
            return tree.GetShortestNonSharedSubString();
        }
        static void Main(string[] args)
        {
            string s1 = Console.ReadLine();
            string s2 = Console.ReadLine();
            Console.WriteLine(Solve(s1, s2));
        }
    }
}
