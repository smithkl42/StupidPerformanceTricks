using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
    class ArraySegmentTest
    {
        private static int arraySize = 1024;
        private static int iterations = 1000000;
        private static int testIterations = 100;

        private static PerformanceMonitor warmup = new PerformanceMonitor("Warmup");
        private static PerformanceMonitor arrayOffsetTest = new PerformanceMonitor("ArrayOffset");
        private static PerformanceMonitor arrayOffsetPrecalcTest = new PerformanceMonitor("ArrayOffsetPrecalc");
        private static PerformanceMonitor arraySegmentTest = new PerformanceMonitor("ArraySegment");

        private static byte[] array = new byte[arraySize];

        static ArraySegmentTest()
        {

        }

        public static void Test()
        {
            PerformTest(warmup, Warmup);
            PerformTest(arrayOffsetTest, OffsetTest);
            PerformTest(arrayOffsetPrecalcTest, OffsetPrecalcTest);
            PerformTest(arraySegmentTest, SegmentTest);
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

        private static void Warmup(int i)
        {
            return;
        }

        private static void OffsetTest(int i)
        {
            int offset = 500;
            int sum = 0;
            for (int j = 0; j < testIterations; j++)
            {
                sum += array[offset + j];
            }
        }

        private static void OffsetPrecalcTest(int i)
        {
            int offset = 500;
            int sum = 0;
            for (int j = offset; j < offset + testIterations; j++)
            {
                sum += array[j];
            }
        }

        private static void SegmentTest(int i)
        {
            var segment = new ArraySegment<byte>(array, 500, array.Length - 500);
            int sum = 0;
            for (int j = 0; j < testIterations; j++)
            {
                sum += segment.Array[segment.Offset + j];
            }
        }
    }
}
