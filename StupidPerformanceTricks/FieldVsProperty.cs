
namespace StupidPerformanceTricks
{
	class FieldVsProperty
	{
		private static PerformanceMonitor instanceTest = new PerformanceMonitor("FieldTest");
		private static PerformanceMonitor readonlyInstanceTest = new PerformanceMonitor("PropertyTest");

		private const int iterations = 10000000;

		private static FieldVsPropertyTestClass myTestClass = new FieldVsPropertyTestClass();

		public static void Test()
		{
			TestHelper.PerformTest(10, instanceTest, FieldTest);
			TestHelper.PerformTest(10, readonlyInstanceTest, PropertyTest);
		}

		private static void FieldTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				myTestClass.MyField = i;
				i = myTestClass.MyField;
			}
		}

		private static void PropertyTest(int iteration)
		{
			for (int i = 0; i < iterations; i++)
			{
				myTestClass.MyProperty = i;
				i = myTestClass.MyProperty;
			}
		}
	}

	public class FieldVsPropertyTestClass
	{
		public int MyField;
		public int MyProperty { get; set; }
	}
}
