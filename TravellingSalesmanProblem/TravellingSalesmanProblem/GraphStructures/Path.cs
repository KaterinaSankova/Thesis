using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TravellingSalesmanProblem.GraphStructures
{
    public class Path
    {
        private List<Node> path;
        public int Count; //can set
        public double Length;
        private int _currentIndex;
        private List<Edge> edges;

        public int CurrentIndex {
            get => _currentIndex;
            set
            {
                if (value < 0 || value > Count - 1)
                    throw new ArgumentOutOfRangeException("Index out of range.");
                _currentIndex = value;
            }
        }

        public void SetCurrentIndex(Node node)
        {
            int index = path.IndexOf(node);
            if ((index != -1) && (index < Count)) { CurrentIndex = index; }
            else throw new ArgumentException($"Node {node} is not in path.");
        }

        public void SetCurrentIndex(int index)
        {
            if ((index != -1) && (index < Count)) { CurrentIndex = index; }
            else throw new ArgumentException($"Index is out of range.");
        }

        public Direction Direction { get; set; }

        public void SetDirection(Node fromNode, Node toNode)
        {
            int fromNodeIndex = path.IndexOf(fromNode);
            if (fromNodeIndex == -1)
                throw new ArgumentException($"Node {fromNode} is not in path.");
            if (path[(fromNodeIndex + 1 + Count) % Count] == toNode)
                Direction = Direction.Forward;
            else if (path[(fromNodeIndex - 1 + Count) % Count] == toNode)
                Direction = Direction.Backwards;
            else
                throw new ArgumentException($"Node {toNode} must be in path right before or after node {fromNode}");
        }

        public void SetStartingPoint(Node fromNode, Node toNode, Direction direction = Direction.Forward)
        {
            int fromNodeIndex = path.IndexOf(fromNode);
            if (fromNodeIndex == -1)
                throw new ArgumentException($"Node {fromNode} is not in path.");
            if (path[(fromNodeIndex + 1 + Count) % Count] == toNode)
                Direction = (Direction)((int)Direction.Forward * (int)direction);
            else if (path[(fromNodeIndex - 1 + Count) % Count] == toNode)
                Direction = (Direction)((int)Direction.Backwards * (int)direction);
            else
                throw new ArgumentException($"Node {toNode} must be in path right before or after node {fromNode}");

            _currentIndex = fromNodeIndex;
        }

        private Path(List<Node> path, int count, double length, int currentIndex, Direction direction, List<Edge> edges)
        {
            this.path = path;
            this.Count = count;
            this.Length = length;
            this.CurrentIndex = currentIndex;
            this.Direction = direction;
            this.edges = edges;
        }

        public Path(List<Node> path)
        {
            this.path = path;
            this.Count = path.Count;
            this.CurrentIndex = 0;
            this.Direction = Direction.Forward;
            Length = 0;
            if (Count > 0)
            {
                for (int i = 0; i < Count - 1; i++)
                    Length += new Edge(path[i], path[i + 1]).Length();
                Length += new Edge(path[0], path[Count - 1]).Length();
            }
        }

        public Node this[int index]
        {
            get
            {
                if (index < 0 || index > Count - 1)
                    throw new ArgumentOutOfRangeException("Index out of range.");
                return path[index];
            }
            set
            {
                if (index < 0 || index > Count - 1)
                    throw new ArgumentOutOfRangeException("Index out of range.");
                path[index] = value;
            }
        }

        public Node Next()
        {
            CurrentIndex = (CurrentIndex + (int)Direction + Count) % Count;
            return path[CurrentIndex];
        } 

        public Node Prev()
        {
            CurrentIndex = (CurrentIndex - (int)Direction + Count) % Count;
            return path[CurrentIndex];
        }

        public Node Next(int step)
        {
            CurrentIndex = (CurrentIndex + ((int)Direction * step) + Count) % Count;
            return path[CurrentIndex];
        }

        public Node Prev(int step)
        {
            CurrentIndex = (CurrentIndex - ((int)Direction * step) + Count) % Count;
            return path[CurrentIndex];
        }
        public int NextIndex(int step = 1) => (CurrentIndex + ((int) Direction * step) + Count) % Count;

        public int PrevIndex(int step = 1) => (CurrentIndex - ((int)Direction * step) + Count) % Count;

        public Node CurrentNode() => path[CurrentIndex];

        public Node PeekNext() => path[(CurrentIndex + (int)Direction + Count) % Count];

        public Node PeekPrev() => path[(CurrentIndex - (int)Direction + Count) % Count];

        public Node PeekNext(Node node) => path[(path.IndexOf(node) + (int)Direction + Count) % Count];

        public Node PeekPrev(Node node) => path[(path.IndexOf(node) - (int)Direction + Count) % Count]; //not in path

        public Node PeekNext(int index) => path[(index + (int)Direction + Count) % Count];

        public Node PeekPrev(int index) => path[(index - (int)Direction + Count) % Count]; //not in path

        public Node PeekBefore(int step) => path[(CurrentIndex - ((int)Direction * step) + Count) % Count];

        public Node PeekAfter(int step) => path[(CurrentIndex + ((int)Direction * step) + Count) % Count];

        public void ReconnectEdges(Node node1, Node node2, Node node3, Node node4) //connected properly
        {
            Console.WriteLine("[+] RECONNECTING EDGES");
            Console.WriteLine($"\tSTARTING NODE: {node1}");
            Console.WriteLine($"\tENCLOSING NODE: {node2}");
            Console.WriteLine($"\tNODE: {node3}");
            Console.WriteLine($"\tNODE: {node4}");
            List<Node> newPath = new List<Node>();

            SetDirection(node2, node1);
            SetCurrentIndex(node1);
            var node = node1;
            while (node != node3)
            {
                newPath.Add(node);
                node = Next();
            }
            newPath.Add(node3);
            newPath.Add(node2);

            SetDirection(node1, node2);
            SetCurrentIndex(node2);
            node = Next();
            while (node != node4)
            {
                newPath.Add(node);
                node = Next();
            }
            newPath.Add(node4);

            Length = Length - new Edge(node1, node2).Length() - new Edge(node3, node4).Length() + new Edge(node2, node3).Length() + new Edge(node4, node1).Length();
            this.edges = null;

            path = newPath;
        }

        public void ReconnectEdges(Node node1, Node node2, Node node3, Node node4, Node node5, Node node6) //connected properly
        {
            Console.WriteLine("[+] RECONNECTING EDGES");
            Console.WriteLine($"\tSTARTING NODE: {node1}");
            Console.WriteLine($"\tENCLOSING NODE: {node2}");
            Console.WriteLine($"\tNODE: {node3}");
            Console.WriteLine($"\tNODE: {node4}");
            Console.WriteLine($"\tNODE: {node5}");
            Console.WriteLine($"\tNODE: {node6}");
            List<Node> newPath = new List<Node>();

            SetDirection(node2, node1);
            SetCurrentIndex(node1);
            var node = node1;
            while (node != node4)
            {
                newPath.Add(node);
                node = Next();
            }
            newPath.Add(node4);

            SetDirection(node6, node5);
            SetCurrentIndex(node5);
            node = node5;
            while (node != node3 && node != node2)
            {
                newPath.Add(node);
                node = Next();
            }
            if (node == node3)
            {
                newPath.Add(node3);
                SetDirection(node1, node2);
                SetCurrentIndex(node2);
                node = node2;
                while (node != node6)
                {
                    newPath.Add(node);
                    node = Next();
                }
                newPath.Add(node6);
            }
            else
            {
                newPath.Add(node2);
                SetDirection(node4, node3);
                SetCurrentIndex(node3);
                node = node3;
                while (node != node6)
                {
                    newPath.Add(node);
                    node = Next();
                }
                newPath.Add(node6);
            }

            Length = Length - new Edge(node1, node2).Length() - new Edge(node3, node4).Length() - new Edge(node5, node6).Length() + new Edge(node2, node3).Length() + new Edge(node4, node5).Length() + new Edge(node6, node1).Length();
            this.edges = null;

            path = newPath;
        }

        public void ReconnectEdges(Node node1, Node node2, Node node3, Node node4, Node node5, Node node6, Node node7, Node node8) //connected properly
        {
            Console.WriteLine("[+] RECONNECTING EDGES");
            Console.WriteLine($"\tSTARTING NODE: {node1}");
            Console.WriteLine($"\tENCLOSING NODE: {node2}");
            Console.WriteLine($"\tNODE: {node3}");
            Console.WriteLine($"\tNODE: {node4}");
            Console.WriteLine($"\tNODE: {node5}");
            Console.WriteLine($"\tNODE: {node6}");
            Console.WriteLine($"\tNODE: {node7}");
            Console.WriteLine($"\tNODE: {node8}");
            List<Node> newPath = new List<Node>();
            SetDirection(node2, node1);
            SetCurrentIndex(node1);
            var node = node1;
            while (node != node5)
            {
                newPath.Add(node);
                node = Next();
            }
            newPath.Add(node5);
            SetDirection(node3, node4);
            SetCurrentIndex(node4);
            node = node4;
            while (node != node6)
            {
                newPath.Add(node);
                node = Next();
            }
            newPath.Add(node6);
            SetDirection(node8, node7);
            SetCurrentIndex(node7);
            node = node7;
            while (node != node3 && node != node2)
            {
                newPath.Add(node);
                node = Next();
            }
            if (node == node3)
            {
                newPath.Add(node3);
                SetDirection(node1, node2);
                SetCurrentIndex(node2);
                node = node2;
                while (node != node8)
                {
                    newPath.Add(node);
                    node = Next();
                }
                newPath.Add(node8);
            }
            else
            {
                newPath.Add(node2);
                SetDirection(node4, node3);
                SetCurrentIndex(node3);
                node = node3;
                while (node != node8)
                {
                    newPath.Add(node);
                    node = Next();
                }
                newPath.Add(node8);
            }

            Length = Length - new Edge(node1, node2).Length() - new Edge(node3, node4).Length() - new Edge(node5, node6).Length() - new Edge(node7, node8).Length() + new Edge(node2, node3).Length() + new Edge(node4, node5).Length() + new Edge(node6, node7).Length() + new Edge(node8, node1).Length();
            this.edges = null;

            path = newPath;
        }

        public void ReconnectEdges((Edge[] BrokenEdges, Edge[] AddedEdges) edgesToReconnect, double improvement)
        {
            Console.WriteLine("[+] RECONNECTING EDGES");
            Console.WriteLine($"\tBROKEN EDGE 1: {edgesToReconnect.BrokenEdges[0]}");
            Console.WriteLine($"\tBROKEN EDGE 2: {edgesToReconnect.BrokenEdges[1]}");
            Console.WriteLine($"\tBROKEN EDGE 3: {edgesToReconnect.BrokenEdges[2]}");
            Console.WriteLine($"\tBROKEN EDGE 4: {edgesToReconnect.BrokenEdges[3]}");
            Console.WriteLine($"\tADDED EDGE 1: {edgesToReconnect.AddedEdges[0]}");
            Console.WriteLine($"\tADDED EDGE 2: {edgesToReconnect.AddedEdges[1]}");
            Console.WriteLine($"\tADDED EDGE 3: {edgesToReconnect.AddedEdges[2]}");
            Console.WriteLine($"\tADDED EDGE 4: {edgesToReconnect.AddedEdges[3]}");
            Console.WriteLine($"\tIMPROVEMENT: {improvement}");
            List<Node> newPath = new List<Node>();

            SetDirection(edgesToReconnect.BrokenEdges[0].node1, edgesToReconnect.BrokenEdges[0].node2);

            SetCurrentIndex(edgesToReconnect.BrokenEdges[0].node2);
            var node = edgesToReconnect.BrokenEdges[0].node2;
            while (node != edgesToReconnect.BrokenEdges[3].node1)
            {
                newPath.Add(node);
                node = Next();
            }
            newPath.Add(edgesToReconnect.BrokenEdges[3].node1);

            SetCurrentIndex(edgesToReconnect.BrokenEdges[2].node2);
            node = edgesToReconnect.BrokenEdges[2].node2;
            while (node != edgesToReconnect.BrokenEdges[0].node1)
            {
                newPath.Add(node);
                node = Next();
            }
            newPath.Add(edgesToReconnect.BrokenEdges[0].node1);

            SetCurrentIndex(edgesToReconnect.BrokenEdges[1].node2);
            node = edgesToReconnect.BrokenEdges[1].node2;
            while (node != edgesToReconnect.BrokenEdges[2].node1)
            {
                newPath.Add(node);
                node = Next();
            }
            newPath.Add(edgesToReconnect.BrokenEdges[2].node1);

            SetCurrentIndex(edgesToReconnect.BrokenEdges[3].node2);
            node = edgesToReconnect.BrokenEdges[3].node2;
            while (node != edgesToReconnect.BrokenEdges[1].node1)
            {
                newPath.Add(node);
                node = Next();
            }
            newPath.Add(edgesToReconnect.BrokenEdges[1].node1);

            Length = Length - improvement;
            this.edges = null;
            path = newPath;
        }


        public List<Edge> GetEdges()
        {
            if(this.edges != null)
                return this.edges;

            if (Count == 0)
                return new List<Edge>();
            if (Count == 1)
                return new List<Edge>() { new Edge(path[0], path[0]) };

            var edges = new List<Edge>();
            var startingNode = CurrentNode();
            var node = Next();
            edges.Add(new Edge (startingNode, node));
            while (node != startingNode)
            {
                edges.Add(new Edge(node, PeekNext()));
                node = Next();
            }

            this.edges = edges;

            return edges;
        }

        public Path ToPath() => new Path(path, Count, Length, CurrentIndex, Direction, edges);

        public bool Contains(Node node) => path.Contains(node);

        public int IndexOf(Node node) => path.IndexOf(node);

        public IEnumerable<Node> Where(Node node, Func<Node, bool> predicate) => path.Where(predicate);

        public List<Node> ToList() => path.ToList();

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                return false;

            Path otherPath = (Path)obj;
            if (this.Count != otherPath.Count)
                return false;

            this.CurrentIndex = 0;
            otherPath.CurrentIndex = otherPath.IndexOf(path[0]);
            for (int i = 0; i < path.Count; i++)
            {
                if (this.Next() != otherPath.Next())
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            var pathString = new StringBuilder("");
            foreach (var node in path)
                pathString.Append($"{node} ");
            return pathString.ToString();
        }
    }
}
