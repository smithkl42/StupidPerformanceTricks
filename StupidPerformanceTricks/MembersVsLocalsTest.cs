using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
    class MembersVsLocalsTest
    {

        private static PerformanceMonitor fpm = new PerformanceMonitor("field");
        private static PerformanceMonitor ppm = new PerformanceMonitor("property");
        private static PerformanceMonitor lpm = new PerformanceMonitor("local");

        private static MyClass myClass = new MyClass();
        private const int iterations = 10000000;

        public static void Test()
        {
            FieldTest();
            PropertyTest();
            LocalTest();
        }

        public static void FieldTest()
        {
            fpm.Start();
            long sum = 0;
            for (int i = 0; i < iterations; i++)
            {
                sum += myClass.IntField;
            }
            fpm.Stop();
        }

        public static void PropertyTest()
        {
            ppm.Start();
            long sum = 0;
            for (int i = 0; i < iterations; i++)
            {
                sum += myClass.IntProperty;
            }
            ppm.Stop();
        }

        public static void LocalTest()
        {
            lpm.Start();
            long sum = 0;
            int local = myClass.IntField;
            for (int i = 0; i < iterations; i++)
            {
                sum += myClass.IntField;
            }
            lpm.Stop();
        }

    }

    public class MyClass
    {
        public MyClass()
        {
            IntField = 100;
            intProperty = 100;
        }

        public int IntField;

        private int intProperty;
        public int IntProperty
        {
            get
            {
                return intProperty;
            }
        }
    }

}
