using System;

namespace StupidPerformanceTricks
{
	class BufferPoolVsNew
	{
		private const int arraySize = 160;
		private const int iterations = 10000000;

		private static readonly BufferPool<short[]> bufferPool;
		private static readonly short[] memberArray = new short[arraySize];

		private static readonly PerformanceMonitor warmup = new PerformanceMonitor("Warmup");
		private static readonly PerformanceMonitor newTest = new PerformanceMonitor("New");
		private static readonly PerformanceMonitor bpTest = new PerformanceMonitor("BufferPool");
		private static readonly PerformanceMonitor memberTest = new PerformanceMonitor("Member");

		static BufferPoolVsNew()
		{
			bufferPool = new BufferPool<short[]>(() => new short[arraySize]);
		}

		public static void Test()
		{
			PerformTest(warmup, Warmup);
			PerformTest(newTest, NewTest);
			PerformTest(bpTest, BufferPoolTest);
			PerformTest(memberTest, MemberTest);
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

		private static void Warmup(int i)
		{
			return;
		}

		private static void NewTest(int i)
		{
			var buffer = new int[arraySize];
			int a = buffer[0];
			buffer[1] = a;
		}

		private static void BufferPoolTest(int i)
		{
			var buffer = bufferPool.GetNext();
			short a = buffer[0];
			buffer[1] = a;
			bufferPool.Recycle(buffer);
		}

		private static void MemberTest(int i)
		{
			short a = memberArray[0];
			memberArray[1] = a;
		}

	}
}
