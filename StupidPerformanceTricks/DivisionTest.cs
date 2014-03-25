using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
	public static class DivisionTest
	{
		private static readonly PerformanceMonitor intShiftTest = new PerformanceMonitor("IntShiftTest");
		private static readonly PerformanceMonitor intDivideTest = new PerformanceMonitor("IntDivideTest");
		private static readonly PerformanceMonitor doubleDivideTest = new PerformanceMonitor("DoubleDivideTest");
		private static readonly PerformanceMonitor floatDivideTest = new PerformanceMonitor("FloatDivideTest");

		private const int iterations = 100000000;
		private static int iResult;
		private static double dResult;
		private static float fResult;

		public static void Test()
		{
			TestHelper.PerformTest(10, intShiftTest, IntShiftTest);
			TestHelper.PerformTest(10, intDivideTest, IntDivideTest);
			TestHelper.PerformTest(10, doubleDivideTest, DoubleDivideTest);
			TestHelper.PerformTest(10, floatDivideTest, FloatDivideTest);
			Console.WriteLine("{0}, {1}, {2}", iResult, dResult, fResult);
		}

		private static void IntShiftTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				iResult = 5000 >> 8;
			}
		}

		private static void IntDivideTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				iResult = 5000 / 256;
			}
		}

		private static void DoubleDivideTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				dResult = 5000.0 / 256.0;
			}
		}

		private static void FloatDivideTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				fResult = 5000.0f / 256.0f;
			}
		}


	}
}
