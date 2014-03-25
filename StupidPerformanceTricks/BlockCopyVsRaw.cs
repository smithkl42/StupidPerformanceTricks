using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StupidPerformanceTricks
{
    class BlockCopyVsRaw
    {
        private static int arraySize = 1000000;
        private static int iterations = 1000000;

        private static byte[] array = new byte[arraySize];
        private static ByteStream byteStream = new ByteStream(array);

        private static PerformanceMonitor warmup = new PerformanceMonitor("Warmup");
        private static PerformanceMonitor wbcp = new PerformanceMonitor("Write.BlockCopy");
        private static PerformanceMonitor wrp = new PerformanceMonitor("Write.Raw");
        private static PerformanceMonitor rbcp = new PerformanceMonitor("Read.BitConverter");
        private static PerformanceMonitor rrp = new PerformanceMonitor("Read.Raw");

        static BlockCopyVsRaw()
        {
        }

        public static void Test()
        {
            PerformTest(warmup, Warmup);
            PerformTest(wrp, WriteRawTest);
            PerformTest(wbcp, WriteBlockCopyTest);
            PerformTest(rrp, ReadRawTest);
            PerformTest(rbcp, ReadBitConverterTest);
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

        private static void WriteBlockCopyTest(int i)
        {
            int endOffset = byteStream.DataOffset + byteStream.DataLength;
            while (byteStream.CurrentOffset < endOffset)
            {
                byteStream.WriteInt16(short.MaxValue);
            }
        }

        private static void WriteRawTest(int i)
        {
            int endOffset = byteStream.DataOffset + byteStream.DataLength;
            while (byteStream.CurrentOffset < endOffset)
            {
                byteStream.WriteInt16Raw(short.MaxValue);
            }
        }

        private static void ReadBitConverterTest(int i)
        {
            int endOffset = byteStream.DataOffset + byteStream.DataLength;
            while (byteStream.CurrentOffset < endOffset)
            {
                short value = byteStream.ReadInt16();
            }
        }

        private static void ReadRawTest(int i)
        {
            int endOffset = byteStream.DataOffset + byteStream.DataLength;
            while (byteStream.CurrentOffset < endOffset)
            {
                short value = byteStream.ReadInt16Raw();
            }
        }

    }
}
