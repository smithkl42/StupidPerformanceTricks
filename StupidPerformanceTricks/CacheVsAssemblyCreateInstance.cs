using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using Alanta.Client.Media;

namespace StupidPerformanceTricks
{
	class CacheVsAssemblyCreateInstance
	{
		private static int cacheSize = 100;
		private static int iterations = 50000;

		private static Dictionary<string, object> cache;
		private static List<string> keys;
		private static Assembly assembly;

		private static PerformanceMonitor warmup = new PerformanceMonitor("Warmup");
		private static PerformanceMonitor cacheTest = new PerformanceMonitor("Cache");
		private static PerformanceMonitor createInstanceTest = new PerformanceMonitor("CreateInstance");

		static CacheVsAssemblyCreateInstance()
		{
			assembly = Assembly.GetExecutingAssembly();
			cache = new Dictionary<string, object>();
			keys = new List<string>();
			for (int i = 0; i < cacheSize; i++)
			{
				string key = i.ToString();
				cache[key] = new TestClass();
				keys.Add(key);
			}
		}

		public static void Test()
		{
			PerformTest(warmup, Warmup);
			PerformTest(cacheTest, CacheTest);
			PerformTest(createInstanceTest, CreateInstanceTest);
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

		private static void CacheTest(int i)
		{
			foreach (string key in keys)
			{
				var testClass = cache[key];
			}
		}

		private static void CreateInstanceTest(int i)
		{
			foreach (string key in keys)
			{
				var testClass = assembly.CreateInstance("StupidPerformanceTricks.TestClass");
			}
		}

	}

	public class TestClass
	{
	}
}
