using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TravellingSalesmanProblem.Diagnostics
{
    public class CallInfo
    {
        int _numberOfCalls = 0;
        TimeSpan _timeSpend = new();

        public int NumberOfCalls
        {
            get => _numberOfCalls;
            private set => _numberOfCalls = value;
        }
        public TimeSpan TimeSpend
        {
            get => _timeSpend;
            private set => _timeSpend = value;
        }

        public TimeSpan AverageTimeSpend { get => TimeSpan.FromMilliseconds((int)(TimeSpend.TotalMilliseconds / NumberOfCalls)); }

        public void AddRecord(TimeSpan time)
        {
            NumberOfCalls++;
            TimeSpend = TimeSpend.Add(time);
        }

        public void Print()
        {
            Console.WriteLine($"\tNumber Of Calls: {NumberOfCalls}");
            Console.WriteLine($"\tTime Spend: {TimeSpend.Minutes}m {TimeSpend.Seconds}s {TimeSpend.Milliseconds / 10}ms");
            Console.WriteLine($"\tAverage Time Spend: {AverageTimeSpend.Minutes}m {AverageTimeSpend.Seconds}s {AverageTimeSpend.Milliseconds / 10}ms");
        }
    }
}
