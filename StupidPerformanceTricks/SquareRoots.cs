using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
	class SquareRoots
	{
		private static PerformanceMonitor mathSqrtTest = new PerformanceMonitor("MathSqrtTest");
		private static PerformanceMonitor approxSqrtTest = new PerformanceMonitor("ApproxSqrtTest");
		private static PerformanceMonitor approxSqrt2Test = new PerformanceMonitor("ApproxSqrt2Test");

		private const int iterations = 10000000;
		public static float result;

		public static void Test()
		{
			TestHelper.PerformTest(10, mathSqrtTest, MathSqrtTest);
			TestHelper.PerformTest(10, approxSqrtTest, ApproxSqrtTest);
			TestHelper.PerformTest(10, approxSqrt2Test, ApproxSqrt2Test);
			Compare(0);
		}

		private static void MathSqrtTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				result = (float)Math.Sqrt(i);
			}
		}

		private static void ApproxSqrtTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				result = Approximate.Sqrt(i);
			}
		}

		private static void ApproxSqrt2Test(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				result = Approximate.Sqrt2(i);
			}
		}


		private static void Compare(int iteration)
		{
			double totalDiff1 = 0;
			double totalDiff2 = 0;
			for (int i = 0; i < iterations; i++)
			{
				var f = Math.Sqrt(i);

				// Compare the first method.
				var f1 = Approximate.Sqrt(i);
				var diff1 = Math.Abs(f - f1) / f;
				if (diff1 > .10)
				{
					Console.WriteLine("i:{0}; f:{1}; f1:{2}; diff: {3}", i, f, f1, diff1);
				}
				if (!double.IsNaN(diff1))
				{
					totalDiff1 += diff1;
				}

				// Compare the first method.
				var f2 = Approximate.Sqrt2(i);
				var diff2 = Math.Abs(f - f2) / f;
				if (diff2 > .10)
				{
					Console.WriteLine("i:{0}; f:{1}; f2:{2}; diff: {3}", i, f, f2, diff2);
				}
				if (!double.IsNaN(diff2))
				{
					totalDiff2 += diff2;
				}

			}
			var avg1 = totalDiff1 / iterations;
			Console.WriteLine("Average variation for method1: {0}", avg1);
			var avg2 = totalDiff2 / iterations;
			Console.WriteLine("Average variation for method2: {0}", avg2);
		}
	}
}
