
namespace StupidPerformanceTricks
{
	class JaggedArrayVs2DArray
	{
		private static PerformanceMonitor jaggedArrayTest = new PerformanceMonitor("JaggedArrayTest");
		private static PerformanceMonitor twoDArrayTest = new PerformanceMonitor("2DArrayTest");
		private static PerformanceMonitor oneDArrayTest = new PerformanceMonitor("1DArrayTest");

		private const int iterations = 100;
		private const int size1 = 1000;
		private const int size2 = 1000;
		public static int result;

		private static int[][] jaggedArray;
		private static int[,] twoDArray;
		private static int[] oneDArray;

		public static void Test()
		{
			jaggedArray = JaggedArrayHelper.Create2DJaggedArray<int>(size1, size2);
			twoDArray = new int[size1, size2];
			oneDArray = new int[size1 * size2];
			TestHelper.PerformTest(10, jaggedArrayTest, JaggedArrayTest);
			TestHelper.PerformTest(10, twoDArrayTest, TwoDArrayTest);
			TestHelper.PerformTest(10, oneDArrayTest, OneDArrayTest);
		}

		private static void JaggedArrayTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				for (int x = 0; x < size1; x++)
				{
					for (int y = 0; y < size2; y++)
					{
						result = jaggedArray[x][y];
					}
				}
			}
		}

		private static void TwoDArrayTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				for (int x = 0; x < size1; x++)
				{
					for (int y = 0; y < size2; y++)
					{
						result = twoDArray[x, y];
					}
				}
			}
		}

		private static void OneDArrayTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				for (int x = 0; x < size1; x++)
				{
					for (int y = 0; y < size2; y++)
					{
						result = oneDArray[x * size2 + y];
					}
				}
			}
		}
	}
}
