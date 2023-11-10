using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanProblem.Diagnostics
{
    public class MethodInfo
    {
        private CallInfo methodCallInfo = new CallInfo();

        public string Name;
        public int TotalNumberOfCalls { get => methodCallInfo.NumberOfCalls; }
        public TimeSpan TotalTimeSpend { get => methodCallInfo.TimeSpend; }
        public TimeSpan AverageTimeSpend { get => methodCallInfo.AverageTimeSpend; }

        public MethodInfo(string Name) => this.Name = Name;

        public void AddRecord(TimeSpan time) => methodCallInfo.AddRecord(time);

        public void Print()
        {
            Console.WriteLine(Name.ToUpper());
            methodCallInfo.Print();
        }
    }
}
