using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
	class LengthVsByteLength
	{
		const int repetitions = 10000000;
		static float[] sourceArray = new float[64];

		public static void Test()
		{
			Length();
			LengthTimesSizeOf();
			ByteLength();
		}

		static PerformanceMonitor byteLength = new PerformanceMonitor("ByteLength");
		private static void ByteLength()
		{
			byteLength.Start();
			long sum = 0;
			for (int x = 0; x < repetitions; x++)
			{
				sum += Buffer.ByteLength(sourceArray);
			}
			byteLength.Stop();
		}

		static PerformanceMonitor length = new PerformanceMonitor("Length");
		private static void Length()
		{
			length.Start();
			long sum = 0;
			for (int x = 0; x < repetitions; x++)
			{
				sum += sourceArray.Length;
			}
			length.Stop();
		}

		static PerformanceMonitor lengthTimesSizeOf = new PerformanceMonitor("LengthTimesSizeOf");
		private static void LengthTimesSizeOf()
		{
			lengthTimesSizeOf.Start();
			long sum = 0;
			for (int x = 0; x < repetitions; x++)
			{
				sum += sourceArray.Length * sizeof(float);
			}
			lengthTimesSizeOf.Stop();
		}
	}
}
