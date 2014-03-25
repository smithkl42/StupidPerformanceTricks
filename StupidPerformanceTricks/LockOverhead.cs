
using System;

namespace StupidPerformanceTricks
{
	class LockOverhead
	{
		private static readonly PerformanceMonitor instanceTest = new PerformanceMonitor("NoLock");
		private static readonly PerformanceMonitor readonlyInstanceTest = new PerformanceMonitor("Locked");

		private const int iterations = 10000000;

		private const int length = 16 * 4;
		private static byte[] buffer1 = new byte[length];
		private static byte[] buffer2 = new byte[length];
		private static object lockObject = new object();


		public static void Test()
		{
			TestHelper.PerformTest(10, instanceTest, NoLock);
			TestHelper.PerformTest(10, readonlyInstanceTest, Locked);
		}

		private static void NoLock(int iteration)
		{
			lock (lockObject)
			{
				for (int i = 0; i < iterations; i++)
				{
					Buffer.BlockCopy(buffer1, 0, buffer2, 0, buffer2.Length);
				}
			}
		}

		private static void Locked(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				lock (lockObject)
				{
					Buffer.BlockCopy(buffer1, 0, buffer2, 0, buffer2.Length);
				}
			}
		}
	}

}
