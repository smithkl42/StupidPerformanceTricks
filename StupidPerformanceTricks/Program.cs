using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StupidPerformanceTricks
{
	class Program
	{

		static void Main(string[] args)
		{
			for (int i = 0; i < 5; i++)
			{
				// QueueTest.Test();
				// SetByteTest.Test();
				// CrcTest.Test();
				// MemoryStreamReadTest.Test();
				// SizeOfTest.Test();
				// FloatingPointTest.Test();
				// ArrayAccessorsTest.Test();
				// MembersVsLocalsTest.Test();
				// ForLoopTest.Test();
				// ArrayCloneTest.Test();
				// ArrayCopyTest.Test();
				// BlockCopyVsRaw.Test();
				// ObjectPoolVsNew.Test();
				// FftTest.Test();
				BufferPoolVsNew.Test();
				// DictionaryLookupTest.Test();
				// ArraySegmentTest.Test();
				// CacheVsAssemblyCreateInstance.Test();
				// TicksTest.Test();
				// LengthVsByteLength.Test();
				// RoundingTest.Test();
				// ArrayClearTest.Test();
				// ArrayReferenceTest.Test();
				// Scratch.TestModulo();
				// Scratch.TestByteSubtraction();
				// StaticVsInstance.Test();
				// FieldVsProperty.Test();
				// LockOverhead.Test();
				// ThreadPoolVsThreads.Test();
				// DivisionTest.Test();
				// GetColorDistanceComparisons.Test();
				// StackVsQueue.Test();
				// SquareRoots.Test();
				// JaggedArrayVs2DArray.Test();
				Console.WriteLine();
			}
			Console.ReadLine();
		}

	}
}
