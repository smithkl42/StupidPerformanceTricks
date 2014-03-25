using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
    class DictionaryLookupTest
    {

        private struct MyStruct
        {
            public MyStruct(short height, short width, short va1, short val2)
            {
                Height = height;
                Width = width;
                Val1 = va1;
                Val2 = val2;
            }

            short Height;
            short Width;
            short Val1;
            short Val2;
        }

        private static int iterations = 1000000;

        private static Dictionary<long, string> longDictionary;
        private static Dictionary<MyStruct, string> structDictionary;

        private static PerformanceMonitor longBlockCopyPM = new PerformanceMonitor("longBlockCopyDictionary");
        private static PerformanceMonitor longMathPM = new PerformanceMonitor("longMathDictionary");
        private static PerformanceMonitor structPM = new PerformanceMonitor("structDictionary");

        static DictionaryLookupTest()
        {
            long lkey1 = GetLongViaBlockCopy(100, 100, 100);
            long lkey2 = GetLongViaMath(200, 200, 200);
            longDictionary = new Dictionary<long, string>();
            longDictionary.Add(lkey1, "Key1");
            longDictionary.Add(lkey2, "Key2");

            MyStruct skey1 = new MyStruct(100, 100, 100, 100);
            MyStruct skey2 = new MyStruct(200, 200, 200, 200);
            structDictionary = new Dictionary<MyStruct, string>();
            structDictionary.Add(skey1, "Key1");
            structDictionary.Add(skey2, "Key2");
        }

        private static long GetLongViaBlockCopy(short height, short width, short quality)
        {
            // Convert the various height/width/quality values into a long (Int64) value.
            byte[] value = new byte[8];

            Buffer.BlockCopy(BitConverter.GetBytes(height), 0, value, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(width), 0, value, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(quality), 0, value, 4, 2);
            ushort threadId = 100;
            Buffer.BlockCopy(BitConverter.GetBytes(threadId), 0, value, 6, 2);
            return BitConverter.ToInt64(value, 0);
        }

        private static long GetLongViaMath(short height, short width, short bands)
        {
            // Convert the various height/width/quality values into a long (Int64) value.
            ushort threadId = 100;
            long l = height |
                (width << 16) |
                (long)((long)bands << 32) |
                (long)((long)threadId << 48);
            return l;

            //byte[] value = new byte[8];
            //value[0] = (byte)(0xff & height);
            //value[1] = (byte)(0xff << 8 & height);
            //value[2] = (byte)(0xff & width);
            //value[3] = (byte)(0xff << 8 & width);
            //value[4] = (byte)(0xff & bands);
            //value[5] = (byte)(0xff << 8 & bands);
            //value[6] = (byte)(0xff & threadId);
            //value[7] = (byte)(0xff << 8 & threadId);
            //return BitConverter.ToInt64(value, 0);
        }


        public static void Test()
        {
            PerformTest(longMathPM, TestLongViaMath);
            PerformTest(longBlockCopyPM, TestLongViaBlockCopy);
            PerformTest(structPM, TestStruct);
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

        private static void TestLongViaBlockCopy(int i)
        {
            long key = GetLongViaBlockCopy(100, 100, 100);
            string s;
            longDictionary.TryGetValue(key, out s);
        }

        private static void TestLongViaMath(int i)
        {
            long key = GetLongViaMath(100, 100, 100);
            string s;
            longDictionary.TryGetValue(key, out s);
        }

        private static void TestStruct(int i)
        {
            MyStruct key = new MyStruct(100, 100, 100, 100);
            string s;
            structDictionary.TryGetValue(key, out s);
        }

    }
}
