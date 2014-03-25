using System.Threading;

namespace StupidPerformanceTricks
{
	class ThreadPoolVsThreads
	{
		private static readonly PerformanceMonitor threadPoolTest = new PerformanceMonitor("ThreadPoolTest");
		private static readonly PerformanceMonitor threadTest = new PerformanceMonitor("ThreadTest");
		private static readonly PerformanceMonitor threadTestWaitAll = new PerformanceMonitor("ThreadTestWaitAll");

		private const int iterations = 100;
		private const int threads = 10;
		private static long somevalue;

		public static void Test()
		{
			TestHelper.PerformTest(10, threadPoolTest, ThreadPoolTest);
			TestHelper.PerformTest(10, threadTest, ThreadTest);
			TestHelper.PerformTest(10, threadTestWaitAll, ThreadTestWaitAll);
		}

		private static void ThreadPoolTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				var resetEvents = new ManualResetEvent[threads];
				for (int j = 0; j < threads; j++)
				{
					var re = new ManualResetEvent(false);
					resetEvents[j] = re;
					ThreadPool.QueueUserWorkItem(o =>
					{
						somevalue++;
						re.Set();
					});
				}
				WaitHandle.WaitAll(resetEvents);
			}
		}

		private static void ThreadTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				var threadArray = new Thread[threads];
				for (int j = 0; j < threads; j++)
				{
					var thread = new Thread(o => somevalue++);
					threadArray[j] = thread;
					thread.Start();
				}
				for (int j = 0; j < threads; j++)
				{
					threadArray[j].Join();
				}
			}
		}

		private static void ThreadTestWaitAll(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				var resetEvents = new ManualResetEvent[threads];
				for (int j = 0; j < threads; j++)
				{
					var re = new ManualResetEvent(false);
					resetEvents[j] = re;
					var thread = new Thread(o =>
					{
						somevalue++;
						re.Set();
					});
					thread.Start();
				}
				WaitHandle.WaitAll(resetEvents);
			}
		}
	}
}
