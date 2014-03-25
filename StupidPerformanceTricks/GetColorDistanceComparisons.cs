using System;

namespace StupidPerformanceTricks
{
	class GetColorDistanceComparisons
	{
		private static readonly PerformanceMonitor getColorTestBaseline = new PerformanceMonitor("GetColorTestBaseline");
		private static readonly PerformanceMonitor getColorTest1 = new PerformanceMonitor("GetColorTestDoubles");
		private static readonly PerformanceMonitor getColorTest2 = new PerformanceMonitor("GetColorTestInts");
		private static readonly PerformanceMonitor getColorTest3 = new PerformanceMonitor("GetColorTestApprox");
		private static readonly PerformanceMonitor getColorTest4 = new PerformanceMonitor("GetColorTestLookup");
		private static readonly PerformanceMonitor getColorTestNoSqrtFloat = new PerformanceMonitor("GetColorTestNoSqrtFloat");
		private static readonly PerformanceMonitor getColorTestNoSqrtInt = new PerformanceMonitor("GetColorTestNoSqrtInt");

		private const int iterations = 10;
		public static int iResult;
		public static double dResult;
		public static float fResult;
		public static byte bResult;

		static GetColorDistanceComparisons()
		{
			PopulateColorDistanceLookup1();
		}

		public static void Test()
		{
			TestHelper.PerformTest(1, getColorTestBaseline, GetColorTestBaseline);
			TestHelper.PerformTest(1, getColorTest1, GetColorTestDoubles);
			TestHelper.PerformTest(1, getColorTest2, GetColorTestInts);
			TestHelper.PerformTest(1, getColorTest3, GetColorTestApprox);
			TestHelper.PerformTest(1, getColorTest4, GetColorTestLookup);
			TestHelper.PerformTest(1, getColorTestNoSqrtFloat, GetColorTestNoSqrtFloat);
			TestHelper.PerformTest(1, getColorTestNoSqrtInt, GetColorTestNoSqrtInt);
		}

		private static void Loop(Action<int, int, int> action)
		{
			for (int i = 0; i < iterations; i++)
			{
				for (int a = 0; a < byte.MaxValue; a++)
				{
					for (int b = 0; b < byte.MaxValue; b++)
					{
						for (int c = 0; c < byte.MaxValue; c++)
						{
							action(a, b, c);
						}
					}
				}
			}
		}

		private static void GetColorTestBaseline(int iteration)
		{
			Loop((a, b, c) =>
			{
				fResult = 0;
			});
		}

		private static void GetColorTestDoubles(int iteration)
		{
			Loop((a, b, c) =>
			{
				fResult = GetColorDistanceDoubles((byte)a, (byte)b, (byte)c, (byte)(c), (byte)b, (byte)a);
			});
		}

		private static void GetColorTestInts(int iteration)
		{
			Loop((a, b, c) =>
			{
				dResult = GetColorDistanceInts((byte)a, (byte)b, (byte)c, (byte)(c), (byte)b, (byte)a);
			});
		}

		private static void GetColorTestApprox(int iteration)
		{
			Loop((a, b, c) =>
			{
				dResult = GetColorDistanceApproximate((byte)a, (byte)b, (byte)c, (byte)(c), (byte)b, (byte)a);
			});
		}

		private static void GetColorTestLookup(int iteration)
		{
			Loop((a, b, c) =>
			{
				bResult = GetColorDistanceLookup1((byte)a, (byte)b, (byte)c, (byte)(c), (byte)b, (byte)a);
			});
		}

		private static void GetColorTestNoSqrtFloat(int iteration)
		{
			Loop((a, b, c) =>
			{
				fResult = GetColorDistanceNoSqrtFloat((byte)a, (byte)b, (byte)c, (byte)(c), (byte)b, (byte)a);
			});
		}

		private static void GetColorTestNoSqrtInt(int iteration)
		{
			Loop((a, b, c) =>
			{
				iResult = GetColorDistanceNoSqrtInt((byte)a, (byte)b, (byte)c, (byte)(c), (byte)b, (byte)a);
			});
		}

		private static float maxDistance;
		public static float GetColorDistanceDoubles(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
		{
			double rmean = (r1 + r2) / 2.0d;
			int r = r1 - r2;
			int g = g1 - g2;
			int b = b1 - b2;
			double weightR = 2 + rmean / 256;
			const double weightG = 4.0;
			double weightB = 2 + (255 - rmean) / 256;
			var distance = (float)Math.Sqrt(weightR * r * r + weightG * g * g + weightB * b * b);
			//if (distance > maxDistance)
			//{
			//    Console.WriteLine("distance: {0}", distance);
			//    maxDistance = distance;
			//}
			return distance;
		}

		public static double GetColorDistanceInts(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
		{
			int rmean = (r1 + r2) / 2;
			int r = r1 - r2;
			int g = g1 - g2;
			int b = b1 - b2;
			int weightR = 2 + rmean / 256;
			const int weightG = 4;
			int weightB = 2 + (255 - rmean) / 256;
			return Math.Sqrt(weightR * r * r + weightG * g * g + weightB * b * b);
		}

		public static float GetColorDistanceApproximate(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
		{
			int rmean = (r1 + r2) / 2;
			int r = r1 - r2;
			int g = g1 - g2;
			int b = b1 - b2;
			int weightR = 2 + rmean / 256;
			const int weightG = 4;
			int weightB = 2 + (255 - rmean) / 256;
			return Approximate.Sqrt(weightR * r * r + weightG * g * g + weightB * b * b);
		}

		public static float GetColorDistanceNoSqrtFloat(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
		{
			int rmean = (r1 + r2) / 2;
			int r = r1 - r2;
			int g = g1 - g2;
			int b = b1 - b2;
			int weightR = 2 + rmean / 256;
			const int weightG = 4;
			int weightB = 2 + (255 - rmean) / 256;
			var distanceSquared = weightR * r * r + weightG * g * g + weightB * b * b;
			return distanceSquared;
		}

		public static int GetColorDistanceNoSqrtInt(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
		{
			int rmean = (r1 + r2) / 2;
			int r = r1 - r2;
			int g = g1 - g2;
			int b = b1 - b2;
			int weightR = 2 + rmean / 256;
			const int weightG = 4;
			int weightB = 2 + (255 - rmean) / 256;
			var distanceSquared = weightR * r * r + weightG * g * g + weightB * b * b;
			return distanceSquared;
		}

		private const int bitdepth = 4;
		private const int bitshift = 8 - bitdepth;

		#region Color Distance Lookup 1
		private static readonly int arraysize1 = (int)Math.Pow(2, bitdepth);
		private static byte[][][][][][] colorDistanceLookup1;
		private static void PopulateColorDistanceLookup1()
		{
			colorDistanceLookup1 = JaggedArrayHelper.Create6DJaggedArray<byte>(arraysize1, arraysize1, arraysize1, arraysize1, arraysize1, arraysize1);
			for (int a = 0; a < arraysize1; a++)
			{
				var r1 = (byte)(a << bitshift);
				for (int b = 0; b < arraysize1; b++)
				{
					var g1 = (byte)(b << bitshift);
					for (int c = 0; c < arraysize1; c++)
					{
						var b1 = (byte)(c << bitshift);
						for (int d = 0; d < arraysize1; d++)
						{
							var r2 = (byte)(d << bitshift);
							for (int e = 0; e < arraysize1; e++)
							{
								var g2 = (byte)(e << bitshift);
								for (int f = 0; f < arraysize1; f++)
								{
									var b2 = (byte)(f << bitshift);
									var distance = (int)GetColorDistanceDoubles(r1, g1, b1, r2, g2, b2);
									distance >>= 1;
									distance = Math.Min(byte.MaxValue, distance);
									colorDistanceLookup1[a][b][c][d][e][f] = (byte)distance;
								}
							}
						}
					}
				}
			}
		}

		public static byte GetColorDistanceLookup1(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
		{
			r1 >>= bitshift;
			g1 >>= bitshift;
			b1 >>= bitshift;
			r2 >>= bitshift;
			g2 >>= bitshift;
			b2 >>= bitshift;
			return colorDistanceLookup1[r1][g1][b1][r2][g2][b2];
		}
		#endregion

	}
}
