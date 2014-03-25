using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alanta.Client.Controls.AudioVisualizer;
using Alanta.Client.Media.Dsp;

namespace StupidPerformanceTricks
{
    /// <summary>
    /// Tests whether the SpeexFft implementation is faster or slower than a clean algorithm from Ernest Laurentin/Don Cross.
    /// </summary>
    class FftTest
    {
        const int repetitions = 10000;
        private const int arrayLength = 256;
        static float[] input = new float[arrayLength];
        static float[] realOut = new float[arrayLength];
        static float[] imaginaryOut = new float[arrayLength];
        private static drft_lookup spxLookup;

        static FftTest()
        {
            // Initialize the source array.
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = i;
            }

            // Initialize the Speex fft lookup table.
            spxLookup = SpeexFft.spx_fft_init(arrayLength);
        }

        public static void Test()
        {
            CrossFftTest();
            SpeexFftTest();

            SpeexFftTest(500);
            SpeexFftTest(512);
            SpeexFftTest(520);
            SpeexFftTest(4000);
            SpeexFftTest(4096);
            SpeexFftTest(4100);
        }

        static PerformanceMonitor crossFftMonitor = new PerformanceMonitor("CrossFft");
        private static void CrossFftTest()
        {
            crossFftMonitor.Start();
            for (int x = 0; x < repetitions; x++)
            {
                FourierTransform.Compute(arrayLength, input, null, realOut, imaginaryOut, false);
            }
            crossFftMonitor.Stop();
        }

        static PerformanceMonitor speexFftMonitor = new PerformanceMonitor("SpeexFft");
        private static void SpeexFftTest()
        {
            speexFftMonitor.Start();
            for (int x = 0; x < repetitions; x++)
            {
                SpeexFft.spx_fft(spxLookup, input, 0, realOut, 0);
            }
            speexFftMonitor.Stop();
        }

        private static void SpeexFftTest(int size)
        {
            drft_lookup lookup = SpeexFft.spx_fft_init(size);
            float[] input = new float[size];
            float[] realOut = new float[size];
            float[] imaginaryOut = new float[size];

            // 
            for (int i = 0; i < size; i++)
            {
                input[i] = i;
            }

            PerformanceMonitor pm = new PerformanceMonitor(size.ToString());
            pm.Start();
            for (int x = 0; x < repetitions; x++)
            {
                SpeexFft.spx_fft(lookup, input, 0, realOut, 0);
            }
            pm.Stop();
        }


    }
}
