using Alanta.Client.Media;

namespace StupidPerformanceTricks
{
	class ObjectPoolVsNew
	{
		private static int arraySize = 1024;
		private static int iterations = 5000000;

		private static ObjectPool<byte[][]> objectPool;

		private static PerformanceMonitor warmup = new PerformanceMonitor("Warmup");
		private static PerformanceMonitor newTest = new PerformanceMonitor("New");
		private static PerformanceMonitor opTest = new PerformanceMonitor("ObjectPool");

		static ObjectPoolVsNew()
		{
			objectPool = new ObjectPool<byte[][]>(() => 
			{
				var arr = new byte[8][];
				for (int i = 0; i < 8; i++)
				{
					arr[i] = new byte[8];
				}
				return arr; 
			});
		}

		public static void Test()
		{
			TestHelper.PerformTest(iterations, warmup, Warmup);
			TestHelper.PerformTest(iterations, newTest, NewTest);
			TestHelper.PerformTest(iterations, opTest, ObjectPoolTest);
		}

		private static void Warmup(int i)
		{
			return;
		}

		private static void NewTest(int i)
		{
			var arr = new byte[8][];
			for (int j = 0; j < 8; j++)
			{
				arr[j] = new byte[8];
			}
			arr[0][0] = 1;
		}

		private static void ObjectPoolTest(int i)
		{
			byte[][] arr = objectPool.GetNext();
			arr[0][0] = 1;
			objectPool.Recycle(arr);
		}

	}
}
