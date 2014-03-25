using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
	class TicksTest
	{
		private static int iterations = 10;
		private static int testIterations = 100000;

		private static PerformanceMonitor rawTicksTest = new PerformanceMonitor("RawTicks");
		private static PerformanceMonitor timeSpanTest = new PerformanceMonitor("TimespanTest");
		private static PerformanceMonitor ticksIncrementTest = new PerformanceMonitor("TicksIncrementTest");

		public static void Test()
		{
			PerformTest(rawTicksTest, RawTicksTest);
			PerformTest(timeSpanTest, TimespanTest);
			PerformTest(ticksIncrementTest, TicksIncrementTest);
		}

		private static void PerformTest(PerformanceMonitor pm, Action<int> action)
		{
			pm.Start();
			for (int i = 0; i < iterations; i++)
			{
				action(i);
			}
			pm.Stop();
		}

		private static void RawTicksTest(int i)
		{
			var startTime = DateTime.Now;
			long ticks = 0;
			for (int j = 0; j < testIterations; j++)
			{
				ticks += DateTime.Now.Ticks - startTime.Ticks;
			}
		}

		private static void TimespanTest(int i)
		{
			var startTime = DateTime.Now;
			long ticks = 0;
			for (int j = 0; j < testIterations; j++)
			{
				ticks += (DateTime.Now - startTime).Ticks;
			}
		}

		private static void TicksIncrementTest(int i)
		{
			long ticks = 0;
			for (int j = 0; j < testIterations; j++)
			{
				ticks += 20*TimeSpan.TicksPerMillisecond;
			}
		}

	}
}
