using System.Runtime.InteropServices;

namespace StupidPerformanceTricks
{
	class Approximate
	{
		public static float Sqrt(float z)
		{
			if (z == 0) return 0;
			FloatIntUnion u;
			u.tmp = 0;
			u.f = z;
			u.tmp -= 1 << 23; /* Subtract 2^m. */
			u.tmp >>= 1; /* Divide by 2. */
			u.tmp += 1 << 29; /* Add ((b + 1) / 2) * 2^m. */
			return u.f;
		}

		public static float Sqrt2(float x)
		{
			if (x == 0) return 0;
			FloatIntUnion u;
			u.tmp = 0;
			float xhalf = 0.5f * x;
			u.f = x;
			u.tmp = 0x5f375a86 - (u.tmp >> 1); // gives initial guess y0
			u.f	 = u.f * (1.5f - xhalf * u.f * u.f); // Newton step, repeating increases accuracy
			return u.f * x;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct FloatIntUnion
		{
			[FieldOffset(0)]
			public float f;

			[FieldOffset(0)]
			public int tmp;
		}
	}
}
