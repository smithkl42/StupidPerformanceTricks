using System;
using System.Net;
using System.Diagnostics;

namespace StupidPerformanceTricks
{
    public class PerformanceMonitor
    {
        public PerformanceMonitor()
        {
            Name = Guid.NewGuid().ToString();
            ReportingFrequency = 1;
        }

        public PerformanceMonitor(string name)
        {
            Name = name;
            ReportingFrequency = 1;
        }

        public PerformanceMonitor(string name, int reportingFrequency)
        {
            Name = name;
            ReportingFrequency = reportingFrequency;
        }

        private DateTime startTime;
        private bool inIteration;
        public int ReportingFrequency { get; set; }
        public string Name { get; set; }
        public int Iterations { get; private set; }
        public double TotalCompletionTimeInMs { get; private set; }
        public double AverageCompletionTimeInMs
        {
            get
            {
                return TotalCompletionTimeInMs / Iterations;
            }
        }

        public void Start()
        {
            if (!inIteration)
            {
                Iterations++;
                startTime = DateTime.Now;
            }
        }

        public void Stop()
        {
            inIteration = false;
            double completionTime = (DateTime.Now - startTime).TotalMilliseconds;
            TotalCompletionTimeInMs += completionTime;
            if (Iterations % ReportingFrequency == 0)
            {
                Console.WriteLine("{0} action completed: iteration = {1}, completionTime = {2}, averageCompletionTime = {3:0.000}", Name, Iterations, completionTime, AverageCompletionTimeInMs);
            }
        }

    }
}
