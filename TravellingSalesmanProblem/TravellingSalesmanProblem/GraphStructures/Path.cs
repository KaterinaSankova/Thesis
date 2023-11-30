using System;
using System.Reflection;
using System.Text;

namespace TravellingSalesmanProblem.GraphStructures
{
    public class Path
    {
        internal List<Node> path;
        internal int _currentIndex;
        internal List<Edge>? _edges;
        internal double _length;

        public double Length => _length;
        public int Count { get; }
        public Direction Direction { get; set; }
        public Node CurrentNode { get => path[CurrentIndex]; }
        public List<Edge> Edges
        {
            get
            {
                if (this._edges != null)
                    return this._edges;

                if (Count == 0)
                    return new List<Edge>();
                if (Count == 1)
                    return new List<Edge>() { new(path[0], path[0]) };

                List<Edge> edges = new();
                var startingNode = this.CurrentNode;
                var node = Next();
                edges.Add(new Edge(startingNode, node));
                while (node != startingNode)
                {
                    edges.Add(new Edge(node, PeekNext()));
                    node = Next();
                }

                this._edges = edges;

                return edges;
            }
        }
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
            if ((index != -1) && (index < Count))
                CurrentIndex = index;
            else
                throw new ArgumentException($"Node {node} is not in path.");
        }

        public void SetCurrentIndex(int index)
        {
            if ((index != -1) && (index < Count))
                CurrentIndex = index;
            else
                throw new ArgumentException($"Index is out of range.");
        }


        public void SetDirection(Node fromNode, Node toNode)
        {
            int fromNodeIndex = path.IndexOf(fromNode);
            if (fromNodeIndex == -1)
                throw new ArgumentException($"Node {fromNode} is not in path.");
            if (PeekAfter(fromNodeIndex, (int)Direction) == toNode)
                Direction = Direction.Forward;
            else if (PeekBefore(fromNodeIndex, (int)Direction) == toNode)
                Direction = Direction.Backwards;
            else
                throw new ArgumentException($"Node {toNode} must be in path right before or after node {fromNode}");
        }

        public void SetStartingPoint(Node fromNode, Node toNode, Direction direction = Direction.Forward)
        {
            int fromNodeIndex = path.IndexOf(fromNode);
            if (fromNodeIndex == -1)
                throw new ArgumentException($"Node {fromNode} is not in path.");
            if (PeekAfter(fromNodeIndex, (int)Direction) == toNode)
                Direction = (Direction)((int)Direction.Forward * (int)direction);
            else if (PeekBefore(fromNodeIndex, (int)Direction) == toNode)
                Direction = (Direction)((int)Direction.Backwards * (int)direction);
            else
                throw new ArgumentException($"Node {toNode} must be in path right before or after node {fromNode}");

            _currentIndex = fromNodeIndex;
        }

        internal Path(List<Node> path, int count, double length, int currentIndex, Direction direction, List<Edge> edges)
        {
            this.path = path;
            this.Count = count;
            this._length = length;
            this.CurrentIndex = currentIndex;
            this.Direction = direction;
            this._edges = edges;
        }

        public Path(List<Node> path)
        {
            this.path = path;
            this.Count = path.Count;
            this.CurrentIndex = 0;
            this.Direction = Direction.Forward;
            _length = 0;
            if (Count > 0)
            {
                for (int i = 0; i < Count - 1; i++)
                    _length += path[i].Distance(path[i + 1]);
                _length += path[0].Distance(path[Count - 1]);
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

        public Node PeekNext() => path[(CurrentIndex + (int)Direction + Count) % Count];

        public Node PeekPrev() => path[(CurrentIndex - (int)Direction + Count) % Count];

        public Node PeekNext(int index) => path[(index + (int)Direction + Count) % Count];

        public Node PeekPrev(int index) => path[(index - (int)Direction + Count) % Count];

        public Node PeekNext(Node node) => PeekNext(IndexOf(node));

        public Node PeekPrev(Node node) => PeekPrev(IndexOf(node));

        public Node PeekBefore(int index, int step) => path[(index - ((int)Direction * step) + Count) % Count];

        public Node PeekAfter(int index, int step) => path[(index + ((int)Direction * step) + Count) % Count];

        public Node PeekBefore(int step) => PeekBefore(CurrentIndex, step);

        public Node PeekAfter(int step) => PeekAfter(CurrentIndex, step);

        public Path ToPath() => new Path(path, Count, Length, CurrentIndex, Direction, _edges);

        public bool Contains(Node node) => path.Contains(node);

        public int IndexOf(Node node) => path.IndexOf(node);

        public IEnumerable<Node> Where(Func<Node, bool> predicate) => path.Where(predicate);

        public List<Node> ToList()
        {
            var pathNodes = path.ToList();
            pathNodes.Add(pathNodes[0]);
            return pathNodes;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            this.CurrentIndex = 0;
            for (int i = 0; i < path.Count; i++)
            {
                hash ^= path[i].GetHashCode();
                this.Next();
            }
            return hash;
        }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                return false;

            Path otherPath = (Path)obj;
            if (this.Count != otherPath.Count)
                return false;

            int currentIndex = CurrentIndex, otherCurrentIndex = otherPath.CurrentIndex;
            Direction direction = this.Direction;

            this.CurrentIndex = 0;
            otherPath.CurrentIndex = otherPath.IndexOf(path[0]);
            if (this.PeekNext() != otherPath.PeekNext() && this.PeekNext() != otherPath.PeekPrev())
                this.Direction = (Direction)(-1 * (int)this.Direction);

            for (int i = 0; i < path.Count; i++)
            {
                if (this.Next() != otherPath.Next())
                    return false;
            }

            CurrentIndex = currentIndex;
            otherPath.CurrentIndex = otherCurrentIndex;
            this.Direction = direction;

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