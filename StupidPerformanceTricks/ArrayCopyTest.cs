using System;

namespace StupidPerformanceTricks
{
	class ArrayCopyTest
	{
		const int repetitions = 10000000;
		private const int arraySize = 16;
		static readonly byte[] sourceArray = new byte[arraySize];
		static byte[] destinationArray = new byte[arraySize];

		public static void Test()
		{
			ArrayClone();
			BufferBlockCopy();
			ArrayCopy();
			ForLoopCopy();
		}

		static PerformanceMonitor arrayCopy = new PerformanceMonitor("Array.Copy");
		private static void ArrayCopy()
		{
			arrayCopy.Start();
			for (int x = 0; x < repetitions; x++)
			{
				Array.Copy(sourceArray, 0, destinationArray, 0, sourceArray.Length);
			}
			arrayCopy.Stop();
		}

		static PerformanceMonitor arrayClone = new PerformanceMonitor("Array.Clone");
		private static void ArrayClone()
		{
			arrayClone.Start();
			for (int x = 0; x < repetitions; x++)
			{
				destinationArray = (byte[])sourceArray.Clone();
			}
			arrayClone.Stop();
		}

		static PerformanceMonitor bufferBlockCopy = new PerformanceMonitor("Buffer.BlockCopy");
		private static void BufferBlockCopy()
		{
			bufferBlockCopy.Start();
			for (int x = 0; x < repetitions; x++)
			{
				Buffer.BlockCopy(sourceArray, 0, destinationArray, 0, sourceArray.Length);
			}
			bufferBlockCopy.Stop();
		}

		static PerformanceMonitor forLoopcopy = new PerformanceMonitor("forLoop");
		private static void ForLoopCopy()
		{
			forLoopcopy.Start();
			for (int x = 0; x < repetitions; x++)
			{
				for (int i = 0; i < sourceArray.Length; i++)
				{
					destinationArray[i] = sourceArray[i];
				}
			}
			forLoopcopy.Stop();
		}

	}
}
