using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
    class CrcTest
    {
        private static int arraySize = 1000;
        private static int iterations = 10000;
        private static byte[] array = new byte[arraySize];
        private static PerformanceMonitor crc32PM = new PerformanceMonitor("crc32");
        private static PerformanceMonitor crc32intPM = new PerformanceMonitor("crc32int");

        public static void Test()
        {
            FillArray();
            Crc32Test();
            Crc32IntTest();
        }

        private static void FillArray()
        {
            for (int i = 0; i < arraySize; i++)
            {
                array[i] = (byte)(i % 256);
            }
        }

        private static void Crc32Test()
        {
            crc32PM.Start();
            for (int i = 0; i < iterations; i++)
            {
                uint result = Crc32.Compute(array);
            }
            crc32PM.Stop();
        }

        private static void Crc32IntTest()
        {
            crc32intPM.Start();
            for (int i = 0; i < iterations; i++)
            {
                uint result = Crc32Int.Compute(array);
            }
            crc32intPM.Stop();
        }


    }
}
