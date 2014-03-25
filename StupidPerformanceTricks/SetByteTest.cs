using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
    class SetByteTest
    {
        private static int arraySize = 10000;
        private static int iterations = 1000;
        private static byte[] array = new byte[arraySize];
        private static PerformanceMonitor arrayPM = new PerformanceMonitor("Array");
        private static PerformanceMonitor bufferPM = new PerformanceMonitor("Buffer.SetByte");

        public static void Test()
        {
            ArrayTest();
            BufferSetByteTest();
        }

        private static void ArrayTest()
        {
            arrayPM.Start();
            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < arraySize; j++)
                {
                    array[j] = 0xFF;
                }
            }
            arrayPM.Stop();
        }

        private static void BufferSetByteTest()
        {
            bufferPM.Start();
            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < arraySize; j++)
                {
                    Buffer.SetByte(array, j, 0xFF);
                }
            }
            bufferPM.Stop();
        }
    }
}
