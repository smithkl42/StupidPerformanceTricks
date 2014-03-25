using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
    class QueueTest
    {
        const int repetitions = 10000;
        const int queueSize = 1200;

        public static void Test()
        {
            NormalQueue();
            FixedQueue();
        }

        static PerformanceMonitor queuePerformance = new PerformanceMonitor("Queue");
        private static void NormalQueue()
        {
            queuePerformance.Start();
            Queue<object> queue = new Queue<object>(queueSize);
            object obj = new object();
            for (int i = 0; i < repetitions; i++)
            {
                for (int j = 0; j < queueSize; j++)
                {
                    queue.Enqueue(obj);
                }
                for (int j = 0; j < queueSize; j++)
                {
                    queue.Dequeue();
                }
            }
            queuePerformance.Stop();
        }

        static PerformanceMonitor fixedQueuePerformance = new PerformanceMonitor("FixedQueue");
        private static void FixedQueue()
        {
            fixedQueuePerformance.Start();
            Queue<object> queue = new Queue<object>();
            object obj = new object();
            for (int i = 0; i < repetitions; i++)
            {
                for (int j = 0; j < queueSize; j++)
                {
                    queue.Enqueue(obj);
                }
                for (int j = 0; j < queueSize; j++)
                {
                    queue.Dequeue();
                }
            }
            fixedQueuePerformance.Stop();
        }
    }
}
