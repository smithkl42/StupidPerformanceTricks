using System;

namespace StupidPerformanceTricks
{
	class ArrayClearTest
	{
		const int repetitions = 10000000;
		private const int arraySize = 12 * 24;
		public static float[] sourceArray = new float[arraySize];
		static readonly float[] emptyArray = new float[arraySize];

		public static void Test()
		{
			ArrayClear();
			ArrayNew();
			BufferBlockCopy();
			ForLoopClear();
		}

		static PerformanceMonitor arrayClear = new PerformanceMonitor("Array.Clear");
		private static void ArrayClear()
		{
			arrayClear.Start();
			for (int x = 0; x < repetitions; x++)
			{
				Array.Clear(sourceArray, 0, sourceArray.Length);
			}
			arrayClear.Stop();
		}

		static PerformanceMonitor arrayNew = new PerformanceMonitor("Array.New");
		private static void ArrayNew()
		{
			arrayNew.Start();
			for (int x = 0; x < repetitions; x++)
			{
				sourceArray = new float[arraySize];
			}
			arrayNew.Stop();
		}

		static PerformanceMonitor bufferBlockCopy = new PerformanceMonitor("Buffer.BlockCopy");
		private static void BufferBlockCopy()
		{
			bufferBlockCopy.Start();
			for (int x = 0; x < repetitions; x++)
			{
				Buffer.BlockCopy(emptyArray, 0, sourceArray, 0, sourceArray.Length * sizeof(int));
			}
			bufferBlockCopy.Stop();
		}


		static PerformanceMonitor forLoopClear = new PerformanceMonitor("forLoop");
		private static void ForLoopClear()
		{
			forLoopClear.Start();
			for (int x = 0; x < repetitions; x++)
			{
				for (int i = 0; i < sourceArray.Length; i++)
				{
					sourceArray[i] = 0;
				}
			}
			forLoopClear.Stop();
		}

	}
}
