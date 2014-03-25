
namespace StupidPerformanceTricks
{
	static class ArrayReferenceTest
	{
		const int repetitions = 1000000;
		private const int arraySize = 12 * 64;
		private static readonly float[][] jaggedArray = new float[2][];
		private static readonly float[] array1 = new float[arraySize];
		private static readonly float[] array2 = new float[arraySize];

		public static void Test()
		{
			jaggedArray[0] = array1;
			jaggedArray[1] = array2;
			JaggedArray();
			SeparateArrays();
		}

		static readonly PerformanceMonitor jaggedArrayPm = new PerformanceMonitor("JaggedArrays");
		private static void JaggedArray()
		{
			jaggedArrayPm.Start();
			float total = 0.0f;
			for (int x = 0; x < repetitions; x++)
			{
				for (int i = 0; i < arraySize; i++)
				{
					total += jaggedArray[0][i] + jaggedArray[1][i];
				}
			}
			jaggedArrayPm.Stop();
		}

		static readonly PerformanceMonitor separateArrayPm = new PerformanceMonitor("SeparateArrays");
		private static void SeparateArrays()
		{
			separateArrayPm.Start();
			float total = 0.0f;
			for (int x = 0; x < repetitions; x++)
			{
				for (int i = 0; i < arraySize; i++)
				{
					total += array1[i] + array2[i];
				}
			}
			separateArrayPm.Stop();
		}

	}
}
