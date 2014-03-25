using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
	class StaticVsInstance
	{
		private static PerformanceMonitor instanceTest = new PerformanceMonitor("InstanceTest");
		private static PerformanceMonitor readonlyInstanceTest = new PerformanceMonitor("ReadonlyInstanceTest");
		private static PerformanceMonitor staticTest = new PerformanceMonitor("StaticTest");

		private const int iterations = 10000000;

		private static MyTestClass myTestClass = new MyTestClass();
		private static readonly MyTestClass readonlyTestClass = new MyTestClass();

		public static void Test()
		{
			TestHelper.PerformTest(10, instanceTest, InstanceTest);
			TestHelper.PerformTest(10, readonlyInstanceTest, ReadonlyInstanceTest);
			TestHelper.PerformTest(10, staticTest, StaticTest);
		}

		private static void InstanceTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				myTestClass.InstanceIncrement();
			}
		}

		private static void ReadonlyInstanceTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				readonlyTestClass.InstanceIncrement();
			}
		}
		private static void StaticTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				MyTestClass.StaticIncrement();
			}
		}

	}

	public class MyTestClass
	{
		private int i;
		public void InstanceIncrement()
		{
			i++;
		}

		private static int statici;
		public static void StaticIncrement()
		{
			statici++;
		}
	}
}
