using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
	class ArrayCloneTest
	{
		const int repetitions = 100;
		const int arraysize = 4000000;
		static byte[] sourceArray = new byte[arraysize];
		static byte[] destinationArray = new byte[arraysize];

		static ArrayCloneTest()
		{
		}

		public static void Test()
		{
			ArrayCopy();
			BufferBlockCopy();
			ManualCopy();
		}

		static PerformanceMonitor arrayCopy = new PerformanceMonitor("Array.Copy");
		private static void ArrayCopy()
		{
			arrayCopy.Start();
			for (int i = 0; i < repetitions; i++)
			{
				for (int x = 0; x < sourceArray.Length; x += 4)
				{
					Array.Copy(sourceArray, x, destinationArray, x, 4);
				}
			}
			arrayCopy.Stop();
		}


		static PerformanceMonitor bufferBlockCopy = new PerformanceMonitor("Buffer.BlockCopy");
		private static void BufferBlockCopy()
		{
			bufferBlockCopy.Start();
			for (int i = 0; i < repetitions; i++)
			{
				for (int x = 0; x < sourceArray.Length; x += 4)
				{
					Buffer.BlockCopy(sourceArray, x, destinationArray, x, 4);
				}
			}
			bufferBlockCopy.Stop();
		}

		static PerformanceMonitor manualCopy = new PerformanceMonitor("Manual.Copy");
		private static void ManualCopy()
		{
			manualCopy.Start();
			for (int i = 0; i < repetitions; i++)
			{
				for (int x = 0; x < sourceArray.Length; x++)
				{
					int y = x;
					destinationArray[x++] = sourceArray[y++];
					destinationArray[x++] = sourceArray[y++];
					destinationArray[x++] = sourceArray[y++];
					destinationArray[x] = sourceArray[y];
				}
			}
			manualCopy.Stop();
		}

	}
}
