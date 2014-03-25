using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
    class ArrayIndexTest
    {
        const int repetitions = 10000;
        const int iMaxValue = short.MaxValue;
        const uint uiMaxValue = (uint)short.MaxValue;
        const ushort usMaxValue = (ushort)short.MaxValue;
        const short sMaxValue = short.MaxValue;
        static byte[] byteArray = new byte[iMaxValue];

        public static void Test()
        {
            for (int i = 0; i < 5; i++)
            {
                UnsignedIntLoop();
                SignedIntLoop();
                UnsignedShortLoop();
                ShortLoop();
                Console.WriteLine();
            }
        }

        private static void ShortLoop()
        {
            var sStartTime = DateTime.Now;
            for (int x = 0; x < repetitions; x++)
            {
                for (short s = 0; s < sMaxValue; s++)
                {
                    byteArray[s] = 1;
                }
            }
            var sElapsed = DateTime.Now - sStartTime;
            Console.WriteLine("Short Elapsed = {0}", sElapsed.TotalMilliseconds);
        }

        private static void UnsignedShortLoop()
        {
            var usStartTime = DateTime.Now;
            for (int x = 0; x < repetitions; x++)
            {
                for (ushort us = 0; us < usMaxValue; us++)
                {
                    byteArray[us] = 1;
                }
            }
            var usElapsed = DateTime.Now - usStartTime;
            Console.WriteLine("Unsigned Short Elapsed = {0}", usElapsed.TotalMilliseconds);
        }

        private static void SignedIntLoop()
        {
            var iStartTime = DateTime.Now;
            for (int x = 0; x < repetitions; x++)
            {
                for (int i = 0; i < iMaxValue; i++)
                {
                    byteArray[i] = 1;
                }
            }
            var iElapsed = DateTime.Now - iStartTime;
            Console.WriteLine("Signed Int Elapsed = {0}", iElapsed.TotalMilliseconds);
        }

        private static void UnsignedIntLoop()
        {
            var uiStartTime = DateTime.Now;
            for (int x = 0; x < repetitions; x++)
            {
                for (uint ui = 0; ui < uiMaxValue; ui++)
                {
                    byteArray[ui] = 1;
                }
            }
            var uiElapsed = DateTime.Now - uiStartTime;
            Console.WriteLine("Unsigned Int Elapsed = {0}", uiElapsed.TotalMilliseconds);
        }


    }
}
