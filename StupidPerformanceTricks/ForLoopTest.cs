using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
    class ForLoopTest
    {

        private static PerformanceMonitor ppm = new PerformanceMonitor("precalc");
        private static PerformanceMonitor dpm = new PerformanceMonitor("dynamic");

        private static MyClass myClass = new MyClass();
        private static int[] intarray = new int[10000];
        private const int iterations = 10000;

        public static void Test()
        {
            PerformTest(dpm, Dynamic);
            PerformTest(ppm, Precalc);
        }

        private static void PerformTest(PerformanceMonitor pm, Action<int> action)
        {
            pm.Start();
            for (int i = 0; i < iterations; i++)
            {
                action(i);
            }
            pm.Stop();
        }


        public static void Precalc(int value)
        {
            int arrayLength = intarray.Length - 5;
            for (int i = 0; i < arrayLength; i++)
            {
                // sum += i;
            }
        }

        public static void Dynamic(int value)
        {
            for (int i = 0; i < intarray.Length - 5; i++)
            {
                // sum += i;
            }
        }

    }

}
