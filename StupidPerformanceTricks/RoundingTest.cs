using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
	class RoundingTest
	{
		const int repetitions = 100000000;

		public static void Test()
		{
			MathRound();
			ImplicitConversion();
		}

		static PerformanceMonitor implicitConversion = new PerformanceMonitor("ImplicitConversion");
		private static void ImplicitConversion()
		{
			implicitConversion.Start();
			long sum = 0;
			for (int x = 0; x < repetitions; x++)
			{
				sum += (int)100.1d;
			}
			implicitConversion.Stop();
		}

		static PerformanceMonitor mathRound = new PerformanceMonitor("MathRound");
		private static void MathRound()
		{
			mathRound.Start();
			long sum = 0;
			for (int x = 0; x < repetitions; x++)
			{
				sum += (int) Math.Round(100.1d);
			}
			mathRound.Stop();
		}
	}
}
