using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanProblem.GraphStructures
{
    public class KernighanLinPath : Path
    {
        public KernighanLinPath() : base() { }

        public KernighanLinPath(List<Node> path) : base(path) { }

        public KernighanLinPath(List<Node> path, int count, double length, int currentIndex, Direction direction, List<Edge> edges) : base(path, count, length, currentIndex, direction, edges) { }

        public new KernighanLinPath ToPath() => new KernighanLinPath(path, Count, Length, CurrentIndex, Direction, _edges);

        public void ReconnectEdges(Node node1, Node node2, Node node3, Node node4) //connected properly
        {
            //Console.WriteLine("[+] RECONNECTING EDGES");
            //Console.WriteLine($"\tSTARTING NODE: {node1}");
            //Console.WriteLine($"\tENCLOSING NODE: {node2}");
            //Console.WriteLine($"\tNODE: {node3}");
            //Console.WriteLine($"\tNODE: {node4}");
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

            _length += node2.Distance(node3) + node4.Distance(node1) - node1.Distance(node2) - node3.Distance(node4);
            this._edges = null;

            path = newPath;
        }

        public void ReconnectEdges(Node node1, Node node2, Node node3, Node node4, Node node5, Node node6) //connected properly
        {
            //Console.WriteLine("[+] RECONNECTING EDGES");
            //Console.WriteLine($"\tSTARTING NODE: {node1}");
            //Console.WriteLine($"\tENCLOSING NODE: {node2}");
            //Console.WriteLine($"\tNODE: {node3}");
            //Console.WriteLine($"\tNODE: {node4}");
            //Console.WriteLine($"\tNODE: {node5}");
            //Console.WriteLine($"\tNODE: {node6}");
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

            _length += node2.Distance(node3) + node4.Distance(node5) + node6.Distance(node1) - node1.Distance(node2) - node3.Distance(node4) - node5.Distance(node6);
            this._edges = null;

            path = newPath;
        }

        public void ReconnectEdges(Node node1, Node node2, Node node3, Node node4, Node node5, Node node6, Node node7, Node node8) //connected properly
        {
            //Console.WriteLine("[+] RECONNECTING EDGES");
            //Console.WriteLine($"\tSTARTING NODE: {node1}");
            //Console.WriteLine($"\tENCLOSING NODE: {node2}");
            //Console.WriteLine($"\tNODE: {node3}");
            //Console.WriteLine($"\tNODE: {node4}");
            //Console.WriteLine($"\tNODE: {node5}");
            //Console.WriteLine($"\tNODE: {node6}");
            //Console.WriteLine($"\tNODE: {node7}");
            //Console.WriteLine($"\tNODE: {node8}");
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

            _length += node2.Distance(node3) + node4.Distance(node5) + node6.Distance(node7) + node8.Distance(node1) - node1.Distance(node2) - node3.Distance(node4) - node5.Distance(node6) - node7.Distance(node8);
            this._edges = null;

            path = newPath;
        }

        public void ReconnectEdges((Edge[] BrokenEdges, Edge[] AddedEdges) edgesToReconnect, double improvement)
        {
            //Console.WriteLine("[+] RECONNECTING EDGES");
            //Console.WriteLine($"\tBROKEN EDGE 1: {edgesToReconnect.BrokenEdges[0]}");
            //Console.WriteLine($"\tBROKEN EDGE 2: {edgesToReconnect.BrokenEdges[1]}");
            //Console.WriteLine($"\tBROKEN EDGE 3: {edgesToReconnect.BrokenEdges[2]}");
            //Console.WriteLine($"\tBROKEN EDGE 4: {edgesToReconnect.BrokenEdges[3]}");
            //Console.WriteLine($"\tADDED EDGE 1: {edgesToReconnect.AddedEdges[0]}");
            //Console.WriteLine($"\tADDED EDGE 2: {edgesToReconnect.AddedEdges[1]}");
            //Console.WriteLine($"\tADDED EDGE 3: {edgesToReconnect.AddedEdges[2]}");
            //Console.WriteLine($"\tADDED EDGE 4: {edgesToReconnect.AddedEdges[3]}");
            //Console.WriteLine($"\tIMPROVEMENT: {improvement}");
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

            _length -= improvement;
            this._edges = null;
            path = newPath;
        }
    }
}
