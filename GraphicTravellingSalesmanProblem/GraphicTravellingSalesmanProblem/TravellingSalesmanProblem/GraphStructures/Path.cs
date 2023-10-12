using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesmanProblem.Enums;

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
            if (index != -1) { CurrentIndex = index; }
            else throw new ArgumentException($"Node {node} is not in path.");
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

        private Path(List<Node> path, int count, double length, int currentIndex, Direction direction)
        {
            this.path = path;
            this.Count = count;
            this.Length = length;
            this.CurrentIndex = currentIndex;
            this.Direction = direction;
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

        public Node CurrentNode() => path[CurrentIndex];

        public Node PeekNext() => path[(CurrentIndex + (int)Direction + Count) % Count];

        public Node PeekPrev() => path[(CurrentIndex - (int)Direction + Count) % Count];

        public Node PeekNext(Node node) => path[(path.IndexOf(node) + (int)Direction + Count) % Count];

        public Node PeekPrev(Node node) => path[(path.IndexOf(node) - (int)Direction + Count) % Count]; //not in path

        public void ReconnectEdges(Node node1, Node node2, Node node3, Node node4) //connected properly
        {
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

            Length += new Edge(node1, node2).Length() + new Edge(node3, node4).Length() - new Edge(node2, node3).Length() - new Edge(node4, node1).Length();
            this.edges = null;

            if (newPath.Count != 10)
            {
                //Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXX");
            }

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

        public Path ToPath() => new Path(path, Count, Length, CurrentIndex, Direction);

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
    }
}
