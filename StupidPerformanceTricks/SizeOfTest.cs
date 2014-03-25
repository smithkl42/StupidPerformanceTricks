using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StupidPerformanceTricks
{
    class SizeOfTest
    {
        private static int iterations = 10000000;
        private const int sizeOfInt = sizeof(int);
        private static PerformanceMonitor dynamicPM = new PerformanceMonitor("dynamic");
        private static PerformanceMonitor constantPM = new PerformanceMonitor("constant");

        public static void Test()
        {
            ConstantTest();
            DynamicTest();
        }

        private static void DynamicTest()
        {
            dynamicPM.Start();
            int value = 0;
            for (int i = 0; i < iterations; i++)
            {
                value += sizeof(int);
                value += sizeof(int);
                value += sizeof(int);
                value += sizeof(int);
                value += sizeof(int);
            }
            dynamicPM.Stop();
        }

        private static void ConstantTest()
        {
            constantPM.Start();
            int value = 0;
            for (int i = 0; i < iterations; i++)
            {
                value += sizeOfInt;
                value += sizeOfInt;
                value += sizeOfInt;
                value += sizeOfInt;
                value += sizeOfInt;
            }
            constantPM.Stop();
        }

    }
}
