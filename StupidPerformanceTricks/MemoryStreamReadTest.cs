using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StupidPerformanceTricks
{
    class MemoryStreamReadTest
    {
        private static int arraySize = 10000;
        private static int iterations = 10000;

        private static byte[] array = new byte[arraySize];
        private static MemoryStream stream = new MemoryStream(array);
        private static BinaryReader reader = new BinaryReader(stream);
        private static ByteStream byteStream = new ByteStream(array);
        private static ByteStreamStruct byteStreamStruct = new ByteStreamStruct(array);

        private static PerformanceMonitor streamLoopPM = new PerformanceMonitor("streamLoop");
        private static PerformanceMonitor sameStreamPM = new PerformanceMonitor("sameStream");
        private static PerformanceMonitor byteStreamPM = new PerformanceMonitor("byteStream");
        private static PerformanceMonitor sameByteStreamPM = new PerformanceMonitor("sameByteStream");
        private static PerformanceMonitor byteStreamStructPM = new PerformanceMonitor("byteStreamStruct");
        private static PerformanceMonitor sameByteStreamStructPM = new PerformanceMonitor("sameByteStreamStruct");
        private static PerformanceMonitor bitConverterPM = new PerformanceMonitor("bitconverter");

        static MemoryStreamReadTest()
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (byte)(i % 256);
            }
        }

        public static void Test()
        {
            PerformTest(streamLoopPM, StreamTest);
            PerformTest(sameStreamPM, SameStreamTest);
            PerformTest(byteStreamPM, ByteStreamTest);
            PerformTest(sameByteStreamPM, SameByteStreamTest);
            PerformTest(byteStreamStructPM, ByteStreamStructTest);
            PerformTest(sameByteStreamStructPM, SameByteStreamStructTest);
            PerformTest(bitConverterPM, BitConverterTest);
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

        private static void StreamTest(int i)
        {
            using (MemoryStream stream = new MemoryStream(array))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    for (int j = 0; j < stream.Length; j += sizeof(int))
                    {
                        int value = reader.ReadInt32();
                    }
                }
            }
        }

        private static void SameStreamTest(int i)
        {
            stream.Seek(0, SeekOrigin.Begin);
            for (int j = 0; j < stream.Length; j += sizeof(int))
            {
                int value = reader.ReadInt32();
            }
        }

        private static void ByteStreamTest(int i)
        {
            ByteStream byteStream = new ByteStream(array);
            int endOffset = byteStream.DataOffset + byteStream.DataLength;
            while (byteStream.CurrentOffset < endOffset)
            {
                int value = byteStream.ReadInt32();
            }
        }

        private static void SameByteStreamTest(int i)
        {
            int endOffset = byteStream.DataOffset + byteStream.DataLength;
            byteStream.CurrentOffset = byteStream.DataOffset;
            while (byteStream.CurrentOffset < endOffset)
            {
                int value = byteStream.ReadInt32();
            }
        }

        private static void ByteStreamStructTest(int i)
        {
            ByteStreamStruct byteStream = new ByteStreamStruct(array);
            int endOffset = byteStream.DataOffset + byteStream.DataLength;
            while (byteStream.CurrentOffset < endOffset)
            {
                int value = byteStream.ReadInt32();
            }
        }

        private static void SameByteStreamStructTest(int i)
        {
            int endOffset = byteStreamStruct.DataOffset + byteStreamStruct.DataLength;
            byteStreamStruct.CurrentOffset = byteStreamStruct.DataOffset;
            while (byteStreamStruct.CurrentOffset < endOffset)
            {
                int value = byteStreamStruct.ReadInt32();
            }
        }


        private static void BitConverterTest(int i)
        {
            for (int j = 0; j < array.Length; j += sizeof(int))
            {
                int value = BitConverter.ToInt32(array, j);
            }
        }

    }
}
