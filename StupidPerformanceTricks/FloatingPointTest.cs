using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
    class FloatingPointTest
    {
        private static PerformanceMonitor fpm = new PerformanceMonitor("fp.multiply");
        private static PerformanceMonitor im = new PerformanceMonitor("i.multiply");
        private static PerformanceMonitor fpd = new PerformanceMonitor("fp.divide");
        private static PerformanceMonitor id = new PerformanceMonitor("i.divide");
        private static PerformanceMonitor fpa = new PerformanceMonitor("fp.add");
        private static PerformanceMonitor ia = new PerformanceMonitor("i.add");
        private static PerformanceMonitor fpc = new PerformanceMonitor("fp.compare");
        private static PerformanceMonitor ic = new PerformanceMonitor("i.compare");

        private static float f0 = 0, f1 = 10000.0f, f2 = 20000.0f;
        private static int i0 = 0, i1 = 10000, i2 = 20000;
        private static bool b = false;

        private const int iterations = 1000000;

        public static void Test()
        {
            PerformTest(fpa, FloatingPointAdditionTest);
            PerformTest(ia, IntegerAdditionTest);
            PerformTest(fpm, FloatingPointMultiplicationTest);
            PerformTest(im, IntegerMultiplicationTest);
            PerformTest(fpd, FloatingPointDivisionTest);
            PerformTest(id, IntegerDivisionTest);
            PerformTest(fpc, FloatingPointComparisonTest);
            PerformTest(ic, IntegerComparisonTest);
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

        public static void FloatingPointMultiplicationTest(int i)
        {
            f0 = f1 * f2;
            f0 = f1 * f2;
            f0 = f1 * f2;
            f0 = f1 * f2;
        }

        public static void IntegerMultiplicationTest(int i)
        {
            i0 = i1 * i2;
            i0 = i1 * i2;
            i0 = i1 * i2;
            i0 = i1 * i2;
        }

        public static void FloatingPointDivisionTest(int i)
        {
            f0 = f1 / f2;
            f0 = f1 / f2;
            f0 = f2 / f1;
            f0 = f2 / f1;
        }

        public static void IntegerDivisionTest(int i)
        {
            i0 = i1 / i2;
            i0 = i1 / i2;
            i0 = i2 / i1;
            i0 = i2 / i1;
        }

        public static void FloatingPointAdditionTest(int i)
        {
            f0 = f1 + f2;
            f0 = f1 + f2;
            f0 = f1 + f2;
            f0 = f1 + f2;
        }

        public static void IntegerAdditionTest(int i)
        {
            i0 = i1 + i2;
            i0 = i1 + i2;
            i0 = i1 + i2;
            i0 = i1 + i2;
        }

        public static void FloatingPointComparisonTest(int i)
        {
            b = f1 > f2;
            b = f1 > f2;
            b = f1 > f2;
            b = f1 > f2;
        }

        public static void IntegerComparisonTest(int i)
        {
            b = i1 > i2;
            b = i1 > i2;
            b = i1 > i2;
            b = i1 > i2;
        }
    }
}
