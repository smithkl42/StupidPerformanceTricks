using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
	class TestHelper
	{
		public static void PerformTest(int iterations, PerformanceMonitor pm, Action<int> action)
		{
			pm.Start();
			for (int i = 0; i < iterations; i++)
			{
				action(i);
			}
			pm.Stop();
			
		}
	}
}
