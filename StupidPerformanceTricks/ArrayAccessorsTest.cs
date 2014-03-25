using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
    class ArrayAccessorsTest
    {
        private static PerformanceMonitor am = new PerformanceMonitor("array.multiply");
        private static PerformanceMonitor aam = new PerformanceMonitor("array.add.multiply");
        private static PerformanceMonitor ap = new PerformanceMonitor("array.power");
        private static PerformanceMonitor nm = new PerformanceMonitor("normal.multiply");
        private static PerformanceMonitor np = new PerformanceMonitor("normal.power");

        private static float[] farray = { 200.0f, 300.0f, 400.0f };

        private const int iterations = 1000000;

        public static void Test()
        {
            PerformTest(nm, NormalMultiply);
            PerformTest(am, ArrayMultiply);
            PerformTest(aam, ArrayAddMultiply);
            // PerformTest(ap, ArrayPower);
            // PerformTest(np, NormalPower);
        }

        public static void PerformTest(PerformanceMonitor pm, Action<int> action)
        {
            pm.Start();
            for (int i = 0; i < iterations; i++)
            {
                action(i);
            }
            pm.Stop();
        }

        public static void ArrayMultiply(int i)
        {
            float result = farray[2] * farray[2];
        }

        public static void ArrayAddMultiply(int i)
        {
            float result = farray[1 + 1] * farray[1 + 1];
        }

        public static void ArrayPower(int i)
        {
            float result = (float)Math.Pow(farray[2], 2.0f);
        }

        public static void NormalMultiply(int i)
        {
            float result = 400.0f * 400.0f;
        }

        public static void NormalPower(int i)
        {
            float result = (float)Math.Pow(400.0f, 2.0f);
        }


    }
}
