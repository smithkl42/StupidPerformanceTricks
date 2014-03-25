using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace StupidPerformanceTricks
{
    class DoubleTest
    {

        public static void Test()
        {
            var startTime = DateTime.Now;
            double dv = 0.0;
            Interlocked.CompareExchange(ref dv, dv, dv); 
            double[] d = new double[1000];
            for (int i = 0; i < 1000000; i++)
            {
                for (int j = 0; j < d.Length; j++)
                {
                    d[j] = (double)(3.0 * d[j]);
                    //double v = 3.0 * d[j];
                    //d[j] = (double)v;
                }
            }
            Console.WriteLine("Double arithmetic completed at {0} milliseconds", (DateTime.Now - startTime).TotalMilliseconds);
        }

    }
}
