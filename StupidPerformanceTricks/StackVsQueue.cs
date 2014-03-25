using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
	class StackVsQueue
	{

		private static PerformanceMonitor stackTest = new PerformanceMonitor("StackTest");
		private static PerformanceMonitor queueTest = new PerformanceMonitor("QueueTest");

		private const int iterations = 100000;
		private const int containerSize = 100;
		public static int item;

		public static void Test()
		{
			TestHelper.PerformTest(10, stackTest, StackTest);
			TestHelper.PerformTest(10, queueTest, QueueTest);
		}

		private static void StackTest(int iteration)
		{
			var stack = new Stack<int>();
			for (int i = 0; i < iterations; i++)
			{
				for (int j = 0; j < containerSize; j++)
				{
					stack.Push(j);
				}
				for (int j = 0; j < containerSize; j++)
				{
					item = stack.Pop();
				}
			}
		}

		private static void QueueTest(int iteration)
		{
			var queue = new Queue<int>();
			for (int i = 0; i < iterations; i++)
			{
				for (int j = 0; j < containerSize; j++)
				{
					queue.Enqueue(j);
				}
				for (int j = 0; j < containerSize; j++)
				{
					item = queue.Dequeue();
				}
			}
		}
	}
}
