using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanProblem.Diagnostics
{
    public class ImprovePathMathodInfo : MethodInfo
    {
        public CallInfo Node1 = new();
        public CallInfo BrokenEdge1 = new();
        public CallInfo AddedEdge1 = new();
        public CallInfo BrokenEdge2 = new();
        public CallInfo AddedEdge2 = new();
        public CallInfo AlternativeBrokenEdge2Option1 = new();
        public CallInfo AlternativeBrokenEdge2Option2 = new();
        public CallInfo FromNode = new();
        public ImprovePathMathodInfo(string Name) : base(Name) { }
        public void Print()
        {
            Console.WriteLine("IMPROVE PATH");
            Console.WriteLine($"Node1:");
            Node1.Print();
            Console.WriteLine($"BrokenEdge1:");
            BrokenEdge1.Print();
            Console.WriteLine($"AddedEdge1:");
            AddedEdge1.Print();
            Console.WriteLine($"AddedEdge2:");
            AddedEdge1.Print();
            Console.WriteLine($"BrokenEdge2:");
            BrokenEdge2.Print();
            Console.WriteLine($"AlternativeBrokenEdge2Option1:");
            AlternativeBrokenEdge2Option1.Print();
            Console.WriteLine($"AlternativeBrokenEdge2Option2:");
            AlternativeBrokenEdge2Option2.Print();
            Console.WriteLine($"FromNode:");
            FromNode.Print();
        }
    }
}
