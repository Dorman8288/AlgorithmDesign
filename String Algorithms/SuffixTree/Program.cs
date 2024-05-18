using System;
using System.Collections.Generic;
using System.Linq;
namespace SuffixTree
{

    class Node
    {
        public int Depth;
        public Node Parent;
        public int index;
        public List<Tuple<Node, int, int>> Edges;
        public Node(int index = -1, int Depth = 0)
        {
            Parent = null;
            this.Depth = Depth;
            this.index = index;
            Edges = new List<Tuple<Node, int, int>>();
        }
        public Node(Node Parent, int index = -1, int Depth = 0)
        {
            this.index = index;
            this.Depth = Depth;
            this.Parent = Parent;
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
            for(int i = 0; i < this.text.Length; i++)
            {
                AddFromIndex(i);
            }
        }

        public SuffixTree(string text, int[] SuffixArray, int[] LCPArray)
        {
            int n = text.Length;
            this.text = text;
            BuildTree(SuffixArray, LCPArray);
        }
        public void BuildTree(int[] SuffixArray, int[] LCPArray)
        {
            int n = text.Length;
            Root = new Node();
            Root.Edges.Add(new Tuple<Node, int, int>(new Node(Root, n - 1, 1), n - 1, 1));
            Node current = Root;
            for (int i = 1; i < n; i++)
            {
                var suffix = SuffixArray[i];
                while (LCPArray[i - 1] < current.Depth)
                    current = current.Parent;
                var SuffixLength = text.Length - suffix;
                var SuffixCurrentIndex = suffix + current.Depth;
                if (current.Depth == LCPArray[i - 1])
                {
                    var NewEdge = new Tuple<Node, int, int>(new Node(current, suffix, SuffixLength), SuffixCurrentIndex, text.Length - SuffixCurrentIndex);
                    current.Edges.Add(NewEdge);
                    current = NewEdge.Item1;
                }
                else
                {
                    int mark = -1;
                    for(int j = 0; j < current.Edges.Count; j++)
                    {
                        var start = current.Edges[j].Item2;
                        if (text[start] == text[SuffixCurrentIndex])
                        {
                            mark = j;
                            break;
                        }
                    }
                    SplitEdge(current, mark, SuffixCurrentIndex, suffix, LCPArray[i - 1] - current.Depth);
                    current = current.Edges.Last().Item1;
                }
            }
        }
        public int[] GetLCPArray(int[] SuffixArray)
        {
            int n = SuffixArray.Length;
            int[] Positions = GetPositions(SuffixArray);
            int[] LCPArray = new int[n - 1];
            int suffix = 0;
            int LCP = 0;
            for(int i = 0; i < n; i++)
            {
                int pos = Positions[suffix];
                if(pos == n - 1)
                {
                    LCP = 0;
                    suffix = (SuffixArray[pos] + 1) % n;
                    continue;
                }
                LCPArray[pos] = CalculateLCP(SuffixArray[pos], SuffixArray[pos + 1], LCP - 1);
                suffix = (SuffixArray[pos] + 1) % n;
            }
            return LCPArray;
        }

        public int CalculateLCP(int i, int j, int equal)
        {
            int LCP = equal > 0 ? equal : 0;
            i = LCP + i;
            j = LCP + j;
            while (i != text.Length && j != text.Length)
            {
                if (text[i] == text[j])
                    LCP++;
                else
                    break;
                i++;
                j++;
            }
            return LCP;
        }

        public int[] GetPositions(int[] SuffixArray)
        {
            int n = SuffixArray.Length;
            int[] Positions = new int[n];
            for(int i = 0; i < n; i++)
                Positions[SuffixArray[i]] = i;
            return Positions;
        }

        public void SplitEdge(Node head, int EdgeNumber, int targetIndex, int finalIndex, int offset = -1)
        {
            Node v = head.Edges[EdgeNumber].Item1;
            int start = head.Edges[EdgeNumber].Item2;
            int length = head.Edges[EdgeNumber].Item3;
            head.Edges.RemoveAt(EdgeNumber);
            int splitLength = 0;
            if (offset == -1)
            {
                while (text[targetIndex + splitLength] == text[start + splitLength])
                    splitLength++;
            }
            else
                splitLength = offset;
            Node Middle = new Node(head, -1, head.Depth + splitLength);
            Node newNode = new Node(Middle, finalIndex, text.Length - finalIndex);
            v.Parent = Middle;
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
                for(int i = 0; i < currentNode.Edges.Count; i++)
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
                for(int i = 0; i < currentNode.Edges.Count; i++)
                {
                    var start = currentNode.Edges[i].Item2;
                    if(text[start] == text[currentIndex])
                    {
                        mark = i;
                        break;
                    }
                }
                if(mark == -1)
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
        public void PrintTree()
        {
            Console.WriteLine(text);
            Stack<Tuple<Node, int, int>> stack = new Stack<Tuple<Node, int, int>>();
            stack.Push(new Tuple<Node, int, int>(Root, 0, 0));
            while (stack.Count > 0)
            {
                var edge = stack.Pop();
                var node = edge.Item1;

                var NodeEdges = node.Edges;
                for(int i = NodeEdges.Count - 1; i >= 0; i--)
                    stack.Push(NodeEdges[i]);
                if(edge.Item2 != 0 | edge.Item3 != 0)
                    Console.WriteLine($"{edge.Item2} {edge.Item2 + edge.Item3}");
            }
        }
    }

    class Program
    {
        static List<string> Solve(string text)
        {
            SuffixTree tree = new SuffixTree(text);
            return tree.GetAllEdgeLabels();
        }
        static void Main(string[] args)
        {
            string s = Console.ReadLine();
            int[] suffix = Console.ReadLine().Split().Select(x => int.Parse(x)).ToArray();
            int[] LCP = Console.ReadLine().Split().Select(x => int.Parse(x)).ToArray();
            SuffixTree tree = new SuffixTree(s, suffix, LCP);
            tree.PrintTree();
        }
    }
}
