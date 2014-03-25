using System;

namespace StupidPerformanceTricks
{
	public static class Scratch
	{
		public static void TestByteToShortConversions()
		{
			for (int i = 0; i < byte.MaxValue; i++)
			{
				for (int j = 0; j < byte.MaxValue; j++)
				{
					var b1 = (byte)i;
					var b2 = (byte)j;
					var s1 = (short)(b1 << 8 + b2);
					var s2 = (short)(b1 << 8 | b2);
					if (s1 != s2)
					{
						Console.WriteLine("They don't match: i={0}, j={1}, s1={2}, s2={3}", i, j, s1, s2);
					}
				}
			}
		}

		public static void TestModulo()
		{
			int i = 16000;
			int j = 32000;
			int k = 24000;
			int l = 48000;
			Console.WriteLine("{0} % {1} = {2}", i, j, i % j);
			Console.WriteLine("{0} % {1} = {2}", i, k, i % k);
			Console.WriteLine("{0} % {1} = {2}", i, l, i % l);
			Console.WriteLine("{0} % {1} = {2}", j, i, j % i);
			Console.WriteLine("{0} % {1} = {2}", k, i, k % i);
			Console.WriteLine("{0} % {1} = {2}", l, i, l % i);
		}

		private const int byteoffset = 127;
		public static void TestByteSubtraction()
		{

			var d = EncodeDelta(129, 0);
			var r = ReconstructOriginal(129, d);

			var foregroundColor = Console.ForegroundColor;
			int correct = 0;
			int wrong = 0;
			for (int oldbyte = byte.MinValue; oldbyte <= byte.MaxValue; oldbyte++)
			{
				for (int newbyte = byte.MinValue; newbyte <= byte.MaxValue; newbyte++)
				{
					// Store the int as an sbyte and transmit.
					// var sbDiff = StoreAsSbyte(iDiff);
					var delta = EncodeDelta((byte)oldbyte, (byte)newbyte);

					var reconstructedNewByte = ReconstructOriginal((byte)oldbyte, delta);

					// Calculate the error
					var error = Math.Abs(newbyte - reconstructedNewByte);

					if (error > 1)
					{
						wrong++;
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("oldbyte:{0}; newbyte:{1}; delta:{2}; reconstructedOldByte:{3}; error:{4}", oldbyte, newbyte, delta, reconstructedNewByte, error);
						Console.ForegroundColor = foregroundColor;
					}
					else
					{
						correct++;
					}
				}
			}
			Console.WriteLine("Correct: {0}, Wrong: {1}", correct, wrong);
		}

		//public static byte EncodeDelta(byte oldByte, byte newByte)
		//{
		//    // (1) Subtract the new byte from the old byte, e.g., 10 - 250 = -240; or 129 - 0 = 129;
		//    // (2) Add 127 to it so that the numbers from step two (which can be expected to cluster around -10 to 10) will cluster around 127 rather than 0-10 and 245-255,
		//    //     e.g., -240 + 127 = -113; or 129 + 127 = 256;
		//    // (2) Divide the result in two so that we can encode it in a byte with only minor loss of fidelity, e.g., -113 >> 1 = -57; or 256 >> 1 = 128;
		//    // (4) Convert the int to a byte and return it, e.g., (byte)-57 = 199; or (byte)128 = 128;
		//    //if (newByte == 255) newByte = 254; // A value of 255 here can cause a wrap-around during the convert-back process.
		//    //if (newByte == 0) newByte = 1;
		//    var delta = oldByte - newByte;
		//    var offsetdelta = delta + byteoffset;
		//    var bitshifted = offsetdelta >> 1;
		//    return (byte)bitshifted;
		//}

		//public static byte ReconstructOriginal(byte oldByte, byte encoded)
		//{
		//    // Reverse the steps above, namely:
		//    // (1) Convert the byte to an sbyte, e.g., (sbyte)199 = -57, or (sbyte)128 = 
		//    // (2) Double the result, e.g., -57 << 1 = -114.
		//    // (3) Subtract 127 from the result, e.g., -114 - 127 = -241.
		//    // (4) Subtract the result from the old byte, e.g., 10 - (-241) = 250 (the original "new byte" above).
		//    var encodedsbyte = (sbyte)encoded;
		//    var bitshifted = encodedsbyte << 1;
		//    var offsetdelta = bitshifted - byteoffset;
		//    var original = oldByte - offsetdelta;
		//    if (original > byte.MaxValue) return byte.MaxValue;
		//    if (original < byte.MinValue) return byte.MinValue;
		//    return (byte)original;
		//}


		public static byte EncodeDelta(byte oldByte, byte newByte)
		{
			// (1) Subtract the new byte from the old byte, e.g., 10 - 250 = -240;
			// (2) Add 127 to it so that the numbers from step two (which can be expected to cluster around -10 to 10) will cluster around 127 rather than 0-10 and 245-255,
			//     e.g., -240 + 127 = -113
			// (2) Divide the result in two so that we can encode it in a byte with only minor loss of fidelity, e.g., -113 >> 1 = -57.
			// (4) Convert the int to a byte and return it, e.g., (byte)-57 = 199.
			if (newByte == 255) newByte = 254;
			var delta = oldByte - newByte;
			var offsetdelta = delta + byteoffset;
			var bitshifted = offsetdelta >> 1;
			return (byte)bitshifted;
		}

		public static byte ReconstructOriginal(byte oldByte, byte encoded)
		{
			// Reverse the steps above, namely:
			// (1) Convert the byte to a signed integer, e.g., 128=128, 199=-57, 192=-64, 63=63. 
			//		The way the math works, we only want to convert it to an sbyte if it's > 191, e.g., sbyte.MaxValue + (sbyte.MaxValue >> 1)
			// (2) Double the result, e.g., -57 << 1 = -114.
			// (3) Subtract 127 from the result, e.g., -114 - 127 = -241.
			// (4) Subtract the result from the old byte, e.g., 10 - (-241) = 250 (the original "new byte" above).
			// (5) Clip the resulting value (rather than overflow).
			int encodedsbyte = encoded > 191 ? (int)(sbyte) encoded : encoded;
			var bitshifted = encodedsbyte << 1;
			var offsetdelta = bitshifted - byteoffset;
			var original = oldByte - offsetdelta;
			if (original < byte.MinValue)
			{
				return byte.MinValue;
			}
			if (original > byte.MaxValue)
			{
				return byte.MaxValue;
			}
			return (byte)original;
		}
	}
}
