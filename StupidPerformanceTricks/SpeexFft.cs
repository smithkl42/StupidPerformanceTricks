﻿#define USE_SMALLFT
using System;
using System.Diagnostics;

/* Copyright (C) 2005-2006 Jean-Marc Valin 
   File: fftwrap.c

   Wrapper for various FFTs 

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:
   
   - Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
   
   - Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
   
   - Neither the name of the Xiph.org Foundation nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.
   
   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE FOUNDATION OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

namespace Alanta.Client.Media.Dsp
{
    public class drft_lookup
    {
        public drft_lookup(int size)
        {
            this.n = size;
            this.trigcache = new float[3 * n]; // (float*)speex_alloc(3*n*sizeof(*l->trigcache));
            this.splitcache = new int[32]; // (int*)speex_alloc(32*sizeof(*l->splitcache));
            SpeexFft.fdrffti(n, trigcache, splitcache);
        }

        public int n;
        public float[] trigcache;
        public int[] splitcache;
    }

    public class SpeexFft
    {
        const int MAX_FFT_SIZE = 2048;

        public static drft_lookup spx_fft_init(int size)
        {
            drft_lookup table = new drft_lookup(size);
            return table;
        }

        public static void spx_fft_destroy(drft_lookup table)
        {
            spx_drft_clear(table);
            // speex_free(table);
        }

        public static void spx_fft(drft_lookup table, float[] inBuffer, int inoffset, float[] outBuffer, int outoffset)
        {
            if (inBuffer == outBuffer)
            {
                int i;
                float scale = 1.0f / table.n;
                Debug.WriteLine("FFT should not be done in-place");
                for (i = 0; i < table.n; i++)
                    outBuffer[outoffset + i] = scale * inBuffer[inoffset + i];
            }
            else
            {
                int i;
                float scale = 1.0f / table.n;
                for (i = 0; i < table.n; i++)
                    outBuffer[outoffset + i] = scale * inBuffer[inoffset + i];
            }
            spx_drft_forward(table, outBuffer, outoffset);
        }

        public static void spx_ifft(drft_lookup table, float[] inBuffer, int inoffset, float[] outBuffer, int outoffset)
        {
            if (inBuffer == outBuffer)
            {
                Debug.WriteLine("FFT should not be done in-place");
            }
            else
            {
                int i;
                for (i = 0; i < table.n; i++)
                    outBuffer[outoffset + i] = inBuffer[inoffset + i];
            }
            spx_drft_backward(table, outBuffer, outoffset);
        }

        static int[] ntryh = new int[] { 4, 2, 3, 5 };
        static float tpi = 6.28318530717958648f;

        static void drfti1(int n, float[] wa, int waoffset, int[] ifac)
        {

            float arg, argh, argld, fi;
            int ntry = 0, i, j = -1;
            int k1, l1, l2, ib;
            int ld, ii, ip, nq, nr;
            int iz;
            int ido, ipm, nfm1;
            int nl = n;
            int nf = 0;

        L101:
            j++;
            if (j < 4)
                ntry = ntryh[j];
            else
                ntry += 2;

        L104:
            nq = nl / ntry;
            nr = nl - ntry * nq;
            if (nr != 0) goto L101;

            nf++;
            ifac[nf + 1] = ntry;
            nl = nq;
            if (ntry != 2) goto L107;
            if (nf == 1) goto L107;

            for (i = 1; i < nf; i++)
            {
                ib = nf - i + 1;
                ifac[ib + 1] = ifac[ib];
            }
            ifac[2] = 2;

        L107:
            if (nl != 1) goto L104;
            ifac[0] = n;
            ifac[1] = nf;
            argh = tpi / n;
            iz = 0;
            nfm1 = nf - 1;
            l1 = 1;

            if (nfm1 == 0) return;

            for (k1 = 0; k1 < nfm1; k1++)
            {
                ip = ifac[k1 + 2];
                ld = 0;
                l2 = l1 * ip;
                ido = n / l2;
                ipm = ip - 1;

                for (j = 0; j < ipm; j++)
                {
                    ld += l1;
                    i = iz;
                    argld = (float)ld * argh;
                    fi = 0.0f;
                    for (ii = 2; ii < ido; ii += 2)
                    {
                        fi += 1.0f;
                        arg = fi * argld;
                        wa[waoffset + i++] = (float)Math.Cos(arg);
                        wa[waoffset + i++] = (float)Math.Sin(arg);
                    }
                    iz += ido;
                }
                l1 = l2;
            }
        }

        public static void fdrffti(int n, float[] wsave, int[] ifac)
        {
            if (n == 1) return;
            drfti1(n, wsave, n, ifac);
        }

        static void dradf2(int ido, int l1,
            float[] cc, int ccoffset,
            float[] ch, int choffset,
            float[] wa1, int wa1offset)
        {
            int i, k;
            float ti2, tr2;
            int t0, t1, t2, t3, t4, t5, t6;

            t1 = 0;
            t0 = (t2 = l1 * ido);
            t3 = ido << 1;
            for (k = 0; k < l1; k++)
            {
                ch[choffset + (t1 << 1)] = cc[ccoffset + t1] + cc[ccoffset + t2];
                ch[choffset + ((t1 << 1) + t3 - 1)] = cc[ccoffset + t1] - cc[ccoffset + t2];
                t1 += ido;
                t2 += ido;
            }

            if (ido < 2) return;
            if (ido == 2) goto L105;

            t1 = 0;
            t2 = t0;
            for (k = 0; k < l1; k++)
            {
                t3 = t2;
                t4 = (t1 << 1) + (ido << 1);
                t5 = t1;
                t6 = t1 + t1;
                for (i = 2; i < ido; i += 2)
                {
                    t3 += 2;
                    t4 -= 2;
                    t5 += 2;
                    t6 += 2;
                    tr2 = wa1[wa1offset + i - 2] * cc[ccoffset + t3 - 1] + wa1[wa1offset + i - 1] * cc[ccoffset + t3];
                    ti2 = wa1[wa1offset + i - 2] * cc[ccoffset + t3] - wa1[wa1offset + i - 1] * cc[ccoffset + t3 - 1];
                    ch[choffset + t6] = cc[ccoffset + t5] + ti2;
                    ch[choffset + t4] = ti2 - cc[ccoffset + t5];
                    ch[choffset + t6 - 1] = cc[ccoffset + t5 - 1] + tr2;
                    ch[choffset + t4 - 1] = cc[ccoffset + t5 - 1] - tr2;
                }
                t1 += ido;
                t2 += ido;
            }

            if (ido % 2 == 1) return;

        L105:
            t3 = (t2 = (t1 = ido) - 1);
            t2 += t0;
            for (k = 0; k < l1; k++)
            {
                ch[choffset + t1] = -cc[ccoffset + t2];
                ch[choffset + t1 - 1] = cc[ccoffset + t3];
                t1 += ido << 1;
                t2 += ido;
                t3 += ido;
            }
        }

        static float hsqt2 = .70710678118654752f;
        static void dradf4(int ido, int l1,
            float[] cc, int ccoffset,
            float[] ch, int choffset,
            float[] wa1, int wa1offset,
            float[] wa2, int wa2offset,
            float[] wa3, int wa3offset)
        {
            int i, k, t0, t1, t2, t3, t4, t5, t6;
            float ci2, ci3, ci4, cr2, cr3, cr4, ti1, ti2, ti3, ti4, tr1, tr2, tr3, tr4;
            t0 = l1 * ido;

            t1 = t0;
            t4 = t1 << 1;
            t2 = t1 + (t1 << 1);
            t3 = 0;

            for (k = 0; k < l1; k++)
            {
                tr1 = cc[ccoffset + t1] + cc[ccoffset + t2];
                tr2 = cc[ccoffset + t3] + cc[ccoffset + t4];

                ch[choffset + (t5 = t3 << 2)] = tr1 + tr2;
                ch[choffset + ((ido << 2) + t5 - 1)] = tr2 - tr1;
                ch[choffset + ((t5 += (ido << 1)) - 1)] = cc[ccoffset + t3] - cc[ccoffset + t4];
                ch[choffset + t5] = cc[ccoffset + t2] - cc[ccoffset + t1];

                t1 += ido;
                t2 += ido;
                t3 += ido;
                t4 += ido;
            }

            if (ido < 2) return;
            if (ido == 2) goto L105;


            t1 = 0;
            for (k = 0; k < l1; k++)
            {
                t2 = t1;
                t4 = t1 << 2;
                t5 = (t6 = ido << 1) + t4;
                for (i = 2; i < ido; i += 2)
                {
                    t3 = (t2 += 2);
                    t4 += 2;
                    t5 -= 2;

                    t3 += t0;
                    cr2 = wa1[wa1offset + i - 2] * cc[ccoffset + t3 - 1] + wa1[wa1offset + i - 1] * cc[ccoffset + t3];
                    ci2 = wa1[wa1offset + i - 2] * cc[ccoffset + t3] - wa1[wa1offset + i - 1] * cc[ccoffset + t3 - 1];
                    t3 += t0;
                    cr3 = wa2[wa2offset + i - 2] * cc[ccoffset + t3 - 1] + wa2[wa2offset + i - 1] * cc[ccoffset + t3];
                    ci3 = wa2[wa2offset + i - 2] * cc[ccoffset + t3] - wa2[wa2offset + i - 1] * cc[ccoffset + t3 - 1];
                    t3 += t0;
                    cr4 = wa3[wa3offset + i - 2] * cc[ccoffset + t3 - 1] + wa3[wa3offset + i - 1] * cc[ccoffset + t3];
                    ci4 = wa3[wa3offset + i - 2] * cc[ccoffset + t3] - wa3[wa3offset + i - 1] * cc[ccoffset + t3 - 1];

                    tr1 = cr2 + cr4;
                    tr4 = cr4 - cr2;
                    ti1 = ci2 + ci4;
                    ti4 = ci2 - ci4;

                    ti2 = cc[ccoffset + t2] + ci3;
                    ti3 = cc[ccoffset + t2] - ci3;
                    tr2 = cc[ccoffset + t2 - 1] + cr3;
                    tr3 = cc[ccoffset + t2 - 1] - cr3;

                    ch[choffset + t4 - 1] = tr1 + tr2;
                    ch[choffset + t4] = ti1 + ti2;

                    ch[choffset + t5 - 1] = tr3 - ti4;
                    ch[choffset + t5] = tr4 - ti3;

                    ch[choffset + t4 + t6 - 1] = ti4 + tr3;
                    ch[choffset + t4 + t6] = tr4 + ti3;

                    ch[choffset + t5 + t6 - 1] = tr2 - tr1;
                    ch[choffset + t5 + t6] = ti1 - ti2;
                }
                t1 += ido;
            }
            if ((ido & 1) != 0) return;

        L105:

            t2 = (t1 = t0 + ido - 1) + (t0 << 1);
            t3 = ido << 2;
            t4 = ido;
            t5 = ido << 1;
            t6 = ido;

            for (k = 0; k < l1; k++)
            {
                ti1 = -hsqt2 * (cc[ccoffset + t1] + cc[ccoffset + t2]);
                tr1 = hsqt2 * (cc[ccoffset + t1] - cc[ccoffset + t2]);

                ch[choffset + t4 - 1] = tr1 + cc[ccoffset + t6 - 1];
                ch[choffset + t4 + t5 - 1] = cc[ccoffset + t6 - 1] - tr1;

                ch[choffset + t4] = ti1 - cc[ccoffset + t1 + t0];
                ch[choffset + t4 + t5] = ti1 + cc[ccoffset + t1 + t0];

                t1 += ido;
                t2 += ido;
                t4 += t3;
                t6 += ido;
            }
        }

        static void dradfg(int ido, int ip, int l1, int idl1,
            float[] cc, int ccoffset,
            float[] c1, int c1offset,
            float[] c2, int c2offset,
            float[] ch, int choffset,
            float[] ch2, int ch2offset,
            float[] wa, int waoffset)
        {

            int idij, ipph, i, j, k, l, ic, ik;
            int iz;
            int t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10;
            float dc2, ai1, ai2, ar1, ar2, ds2;
            int nbd;
            float dcp, arg, dsp, ar1h, ar2h;
            int idp2, ipp2;

            arg = tpi / (float)ip;
            dcp = (float)Math.Cos(arg);
            dsp = (float)Math.Sin(arg);
            ipph = (ip + 1) >> 1;
            ipp2 = ip;
            idp2 = ido;
            nbd = (ido - 1) >> 1;
            t0 = l1 * ido;
            t10 = ip * ido;

            if (ido == 1) goto L119;
            for (ik = 0; ik < idl1; ik++) ch2[ch2offset + ik] = c2[c2offset + ik];

            t1 = 0;
            for (j = 1; j < ip; j++)
            {
                t1 += t0;
                t2 = t1;
                for (k = 0; k < l1; k++)
                {
                    ch[choffset + t2] = c1[c1offset + t2];
                    t2 += ido;
                }
            }

            iz = -ido;
            t1 = 0;
            if (nbd > l1)
            {
                for (j = 1; j < ip; j++)
                {
                    t1 += t0;
                    iz += ido;
                    t2 = -ido + t1;
                    for (k = 0; k < l1; k++)
                    {
                        idij = iz - 1;
                        t2 += ido;
                        t3 = t2;
                        for (i = 2; i < ido; i += 2)
                        {
                            idij += 2;
                            t3 += 2;
                            ch[choffset + t3 - 1] = wa[waoffset + idij - 1] * c1[c1offset + t3 - 1] + wa[waoffset + idij] * c1[c1offset + t3];
                            ch[choffset + t3] = wa[waoffset + idij - 1] * c1[c1offset + t3] - wa[waoffset + idij] * c1[c1offset + t3 - 1];
                        }
                    }
                }
            }
            else
            {

                for (j = 1; j < ip; j++)
                {
                    iz += ido;
                    idij = iz - 1;
                    t1 += t0;
                    t2 = t1;
                    for (i = 2; i < ido; i += 2)
                    {
                        idij += 2;
                        t2 += 2;
                        t3 = t2;
                        for (k = 0; k < l1; k++)
                        {
                            ch[choffset + t3 - 1] = wa[waoffset + idij - 1] * c1[c1offset + t3 - 1] + wa[waoffset + idij] * c1[c1offset + t3];
                            ch[choffset + t3] = wa[waoffset + idij - 1] * c1[c1offset + t3] - wa[waoffset + idij] * c1[c1offset + t3 - 1];
                            t3 += ido;
                        }
                    }
                }
            }

            t1 = 0;
            t2 = ipp2 * t0;
            if (nbd < l1)
            {
                for (j = 1; j < ipph; j++)
                {
                    t1 += t0;
                    t2 -= t0;
                    t3 = t1;
                    t4 = t2;
                    for (i = 2; i < ido; i += 2)
                    {
                        t3 += 2;
                        t4 += 2;
                        t5 = t3 - ido;
                        t6 = t4 - ido;
                        for (k = 0; k < l1; k++)
                        {
                            t5 += ido;
                            t6 += ido;
                            c1[c1offset + t5 - 1] = ch[choffset + t5 - 1] + ch[choffset + t6 - 1];
                            c1[c1offset + t6 - 1] = ch[choffset + t5] - ch[choffset + t6];
                            c1[c1offset + t5] = ch[choffset + t5] + ch[choffset + t6];
                            c1[c1offset + t6] = ch[choffset + t6 - 1] - ch[choffset + t5 - 1];
                        }
                    }
                }
            }
            else
            {
                for (j = 1; j < ipph; j++)
                {
                    t1 += t0;
                    t2 -= t0;
                    t3 = t1;
                    t4 = t2;
                    for (k = 0; k < l1; k++)
                    {
                        t5 = t3;
                        t6 = t4;
                        for (i = 2; i < ido; i += 2)
                        {
                            t5 += 2;
                            t6 += 2;
                            c1[c1offset + t5 - 1] = ch[choffset + t5 - 1] + ch[choffset + t6 - 1];
                            c1[c1offset + t6 - 1] = ch[choffset + t5] - ch[choffset + t6];
                            c1[c1offset + t5] = ch[choffset + t5] + ch[choffset + t6];
                            c1[c1offset + t6] = ch[choffset + t6 - 1] - ch[choffset + t5 - 1];
                        }
                        t3 += ido;
                        t4 += ido;
                    }
                }
            }

        L119:
            for (ik = 0; ik < idl1; ik++) c2[c2offset + ik] = ch2[ch2offset + ik];

            t1 = 0;
            t2 = ipp2 * idl1;
            for (j = 1; j < ipph; j++)
            {
                t1 += t0;
                t2 -= t0;
                t3 = t1 - ido;
                t4 = t2 - ido;
                for (k = 0; k < l1; k++)
                {
                    t3 += ido;
                    t4 += ido;
                    c1[c1offset + t3] = ch[choffset + t3] + ch[choffset + t4];
                    c1[c1offset + t4] = ch[choffset + t4] - ch[choffset + t3];
                }
            }

            ar1 = 1.0f;
            ai1 = 0.0f;
            t1 = 0;
            t2 = ipp2 * idl1;
            t3 = (ip - 1) * idl1;
            for (l = 1; l < ipph; l++)
            {
                t1 += idl1;
                t2 -= idl1;
                ar1h = dcp * ar1 - dsp * ai1;
                ai1 = dcp * ai1 + dsp * ar1;
                ar1 = ar1h;
                t4 = t1;
                t5 = t2;
                t6 = t3;
                t7 = idl1;

                for (ik = 0; ik < idl1; ik++)
                {
                    ch2[ch2offset + t4++] = c2[c2offset + ik] + ar1 * c2[c2offset + t7++];
                    ch2[ch2offset + t5++] = ai1 * c2[c2offset + t6++];
                }

                dc2 = ar1;
                ds2 = ai1;
                ar2 = ar1;
                ai2 = ai1;

                t4 = idl1;
                t5 = (ipp2 - 1) * idl1;
                for (j = 2; j < ipph; j++)
                {
                    t4 += idl1;
                    t5 -= idl1;

                    ar2h = dc2 * ar2 - ds2 * ai2;
                    ai2 = dc2 * ai2 + ds2 * ar2;
                    ar2 = ar2h;

                    t6 = t1;
                    t7 = t2;
                    t8 = t4;
                    t9 = t5;
                    for (ik = 0; ik < idl1; ik++)
                    {
                        ch2[ch2offset + t6++] += ar2 * c2[c2offset + t8++];
                        ch2[ch2offset + t7++] += ai2 * c2[c2offset + t9++];
                    }
                }
            }

            t1 = 0;
            for (j = 1; j < ipph; j++)
            {
                t1 += idl1;
                t2 = t1;
                for (ik = 0; ik < idl1; ik++) ch2[ch2offset + ik] += c2[c2offset + t2++];
            }

            if (ido < l1) goto L132;

            t1 = 0;
            t2 = 0;
            for (k = 0; k < l1; k++)
            {
                t3 = t1;
                t4 = t2;
                for (i = 0; i < ido; i++) cc[ccoffset + t4++] = ch[choffset + t3++];
                t1 += ido;
                t2 += t10;
            }

            goto L135;

        L132:
            for (i = 0; i < ido; i++)
            {
                t1 = i;
                t2 = i;
                for (k = 0; k < l1; k++)
                {
                    cc[ccoffset + t2] = ch[choffset + t1];
                    t1 += ido;
                    t2 += t10;
                }
            }

        L135:
            t1 = 0;
            t2 = ido << 1;
            t3 = 0;
            t4 = ipp2 * t0;
            for (j = 1; j < ipph; j++)
            {

                t1 += t2;
                t3 += t0;
                t4 -= t0;

                t5 = t1;
                t6 = t3;
                t7 = t4;

                for (k = 0; k < l1; k++)
                {
                    cc[ccoffset + t5 - 1] = ch[choffset + t6];
                    cc[ccoffset + t5] = ch[choffset + t7];
                    t5 += t10;
                    t6 += ido;
                    t7 += ido;
                }
            }

            if (ido == 1) return;
            if (nbd < l1) goto L141;

            t1 = -ido;
            t3 = 0;
            t4 = 0;
            t5 = ipp2 * t0;
            for (j = 1; j < ipph; j++)
            {
                t1 += t2;
                t3 += t2;
                t4 += t0;
                t5 -= t0;
                t6 = t1;
                t7 = t3;
                t8 = t4;
                t9 = t5;
                for (k = 0; k < l1; k++)
                {
                    for (i = 2; i < ido; i += 2)
                    {
                        ic = idp2 - i;
                        cc[ccoffset + i + t7 - 1] = ch[choffset + i + t8 - 1] + ch[choffset + i + t9 - 1];
                        cc[ccoffset + ic + t6 - 1] = ch[choffset + i + t8 - 1] - ch[choffset + i + t9 - 1];
                        cc[ccoffset + i + t7] = ch[choffset + i + t8] + ch[choffset + i + t9];
                        cc[ccoffset + ic + t6] = ch[choffset + i + t9] - ch[choffset + i + t8];
                    }
                    t6 += t10;
                    t7 += t10;
                    t8 += ido;
                    t9 += ido;
                }
            }
            return;

        L141:

            t1 = -ido;
            t3 = 0;
            t4 = 0;
            t5 = ipp2 * t0;
            for (j = 1; j < ipph; j++)
            {
                t1 += t2;
                t3 += t2;
                t4 += t0;
                t5 -= t0;
                for (i = 2; i < ido; i += 2)
                {
                    t6 = idp2 + t1 - i;
                    t7 = i + t3;
                    t8 = i + t4;
                    t9 = i + t5;
                    for (k = 0; k < l1; k++)
                    {
                        cc[ccoffset + t7 - 1] = ch[choffset + t8 - 1] + ch[choffset + t9 - 1];
                        cc[ccoffset + t6 - 1] = ch[choffset + t8 - 1] - ch[choffset + t9 - 1];
                        cc[ccoffset + t7] = ch[choffset + t8] + ch[choffset + t9];
                        cc[ccoffset + t6] = ch[choffset + t9] - ch[choffset + t8];
                        t6 += t10;
                        t7 += t10;
                        t8 += ido;
                        t9 += ido;
                    }
                }
            }
        }

        static void drftf1(int n,
            float[] c, int coffset,
            float[] ch, int choffset,
            float[] wa, int waoffset,
            int[] ifac)
        {
            int i, k1, l1, l2;
            int na, kh, nf;
            int ip, iw, ido, idl1, ix2, ix3;

            nf = ifac[1];
            na = 1;
            l2 = n;
            iw = n;

            for (k1 = 0; k1 < nf; k1++)
            {
                kh = nf - k1;
                ip = ifac[kh + 1];
                l1 = l2 / ip;
                ido = n / l2;
                idl1 = ido * l1;
                iw -= (ip - 1) * ido;
                na = 1 - na;

                if (ip != 4) goto L102;

                ix2 = iw + ido;
                ix3 = ix2 + ido;
                if (na != 0)
                    dradf4(ido, l1, ch, choffset, c, coffset, wa, waoffset + iw - 1, wa, waoffset + ix2 - 1, wa, waoffset + ix3 - 1);
                else
                    dradf4(ido, l1, c, coffset, ch, choffset, wa, waoffset + iw - 1, wa, waoffset + ix2 - 1, wa, waoffset + ix3 - 1);
                goto L110;

            L102:
                if (ip != 2) goto L104;
                if (na != 0) goto L103;

                dradf2(ido, l1, c, coffset, ch, choffset, wa, waoffset + iw - 1);
                goto L110;

            L103:
                dradf2(ido, l1, ch, choffset, c, coffset, wa, waoffset + iw - 1);
                goto L110;

            L104:
                if (ido == 1) na = 1 - na;
                if (na != 0) goto L109;

                dradfg(ido, ip, l1, idl1, c, coffset, c, coffset, c, coffset, ch, choffset, ch, choffset, wa, waoffset + iw - 1);
                na = 1;
                goto L110;

            L109:
                dradfg(ido, ip, l1, idl1, ch, choffset, ch, choffset, ch, choffset, c, coffset, c, coffset, wa, waoffset + iw - 1);
                na = 0;

            L110:
                l2 = l1;
            }

            if (na == 1) return;

            for (i = 0; i < n; i++) c[coffset + i] = ch[choffset + i];
        }

        static void dradb2(int ido, int l1,
            float[] cc, int ccoffset,
            float[] ch, int choffset,
            float[] wa1, int wa1offset)
        {
            int i, k, t0, t1, t2, t3, t4, t5, t6;
            float ti2, tr2;

            t0 = l1 * ido;

            t1 = 0;
            t2 = 0;
            t3 = (ido << 1) - 1;
            for (k = 0; k < l1; k++)
            {
                ch[choffset + t1] = cc[ccoffset + t2] + cc[ccoffset + t3 + t2];
                ch[choffset + t1 + t0] = cc[ccoffset + t2] - cc[ccoffset + t3 + t2];
                t2 = (t1 += ido) << 1;
            }

            if (ido < 2) return;
            if (ido == 2) goto L105;

            t1 = 0;
            t2 = 0;
            for (k = 0; k < l1; k++)
            {
                t3 = t1;
                t5 = (t4 = t2) + (ido << 1);
                t6 = t0 + t1;
                for (i = 2; i < ido; i += 2)
                {
                    t3 += 2;
                    t4 += 2;
                    t5 -= 2;
                    t6 += 2;
                    ch[choffset + t3 - 1] = cc[ccoffset + t4 - 1] + cc[ccoffset + t5 - 1];
                    tr2 = cc[ccoffset + t4 - 1] - cc[ccoffset + t5 - 1];
                    ch[choffset + t3] = cc[ccoffset + t4] - cc[ccoffset + t5];
                    ti2 = cc[ccoffset + t4] + cc[ccoffset + t5];
                    ch[choffset + t6 - 1] = wa1[wa1offset + i - 2] * tr2 - wa1[wa1offset + i - 1] * ti2;
                    ch[choffset + t6] = wa1[wa1offset + i - 2] * ti2 + wa1[wa1offset + i - 1] * tr2;
                }
                t2 = (t1 += ido) << 1;
            }

            if (ido % 2 == 1) return;

        L105:
            t1 = ido - 1;
            t2 = ido - 1;
            for (k = 0; k < l1; k++)
            {
                ch[choffset + t1] = cc[ccoffset + t2] + cc[ccoffset + t2];
                ch[choffset + t1 + t0] = -(cc[ccoffset + t2 + 1] + cc[ccoffset + t2 + 1]);
                t1 += ido;
                t2 += ido << 1;
            }
        }

        static float taur = -.5f;
        static float taui = .8660254037844386f;

        static void dradb3(int ido, int l1,
            float[] cc, int ccoffset,
            float[] ch, int choffset,
            float[] wa1, int wa1offset,
            float[] wa2, int wa2offset)
        {
            int i, k, t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10;
            float ci2, ci3, di2, di3, cr2, cr3, dr2, dr3, ti2, tr2;
            t0 = l1 * ido;

            t1 = 0;
            t2 = t0 << 1;
            t3 = ido << 1;
            t4 = ido + (ido << 1);
            t5 = 0;
            for (k = 0; k < l1; k++)
            {
                tr2 = cc[ccoffset + t3 - 1] + cc[ccoffset + t3 - 1];
                cr2 = cc[ccoffset + t5] + (taur * tr2);
                ch[choffset + t1] = cc[ccoffset + t5] + tr2;
                ci3 = taui * (cc[ccoffset + t3] + cc[ccoffset + t3]);
                ch[choffset + t1 + t0] = cr2 - ci3;
                ch[choffset + t1 + t2] = cr2 + ci3;
                t1 += ido;
                t3 += t4;
                t5 += t4;
            }

            if (ido == 1) return;

            t1 = 0;
            t3 = ido << 1;
            for (k = 0; k < l1; k++)
            {
                t7 = t1 + (t1 << 1);
                t6 = (t5 = t7 + t3);
                t8 = t1;
                t10 = (t9 = t1 + t0) + t0;

                for (i = 2; i < ido; i += 2)
                {
                    t5 += 2;
                    t6 -= 2;
                    t7 += 2;
                    t8 += 2;
                    t9 += 2;
                    t10 += 2;
                    tr2 = cc[ccoffset + t5 - 1] + cc[ccoffset + t6 - 1];
                    cr2 = cc[ccoffset + t7 - 1] + (taur * tr2);
                    ch[choffset + t8 - 1] = cc[ccoffset + t7 - 1] + tr2;
                    ti2 = cc[ccoffset + t5] - cc[ccoffset + t6];
                    ci2 = cc[ccoffset + t7] + (taur * ti2);
                    ch[choffset + t8] = cc[ccoffset + t7] + ti2;
                    cr3 = taui * (cc[ccoffset + t5 - 1] - cc[ccoffset + t6 - 1]);
                    ci3 = taui * (cc[ccoffset + t5] + cc[ccoffset + t6]);
                    dr2 = cr2 - ci3;
                    dr3 = cr2 + ci3;
                    di2 = ci2 + cr3;
                    di3 = ci2 - cr3;
                    ch[choffset + t9 - 1] = wa1[wa1offset + i - 2] * dr2 - wa1[wa1offset + i - 1] * di2;
                    ch[choffset + t9] = wa1[wa1offset + i - 2] * di2 + wa1[wa1offset + i - 1] * dr2;
                    ch[choffset + t10 - 1] = wa2[wa2offset + i - 2] * dr3 - wa2[wa2offset + i - 1] * di3;
                    ch[choffset + t10] = wa2[wa2offset + i - 2] * di3 + wa2[wa2offset + i - 1] * dr3;
                }
                t1 += ido;
            }
        }

        static float sqrt2 = 1.414213562373095f;

        static void dradb4(int ido, int l1,
            float[] cc, int ccoffset,
            float[] ch, int choffset,
            float[] wa1, int wa1offset,
            float[] wa2, int wa2offset,
            float[] wa3, int wa3offset)
        {
            int i, k, t0, t1, t2, t3, t4, t5, t6, t7, t8;
            float ci2, ci3, ci4, cr2, cr3, cr4, ti1, ti2, ti3, ti4, tr1, tr2, tr3, tr4;
            t0 = l1 * ido;

            t1 = 0;
            t2 = ido << 2;
            t3 = 0;
            t6 = ido << 1;
            for (k = 0; k < l1; k++)
            {
                t4 = t3 + t6;
                t5 = t1;
                tr3 = cc[ccoffset + t4 - 1] + cc[ccoffset + t4 - 1];
                tr4 = cc[ccoffset + t4] + cc[ccoffset + t4];
                tr1 = cc[ccoffset + t3] - cc[ccoffset + (t4 += t6) - 1];
                tr2 = cc[ccoffset + t3] + cc[ccoffset + t4 - 1];
                ch[choffset + t5] = tr2 + tr3;
                ch[choffset + (t5 += t0)] = tr1 - tr4;
                ch[choffset + (t5 += t0)] = tr2 - tr3;
                ch[choffset + (t5 += t0)] = tr1 + tr4;
                t1 += ido;
                t3 += t2;
            }

            if (ido < 2) return;
            if (ido == 2) goto L105;

            t1 = 0;
            for (k = 0; k < l1; k++)
            {
                t5 = (t4 = (t3 = (t2 = t1 << 2) + t6)) + t6;
                t7 = t1;
                for (i = 2; i < ido; i += 2)
                {
                    t2 += 2;
                    t3 += 2;
                    t4 -= 2;
                    t5 -= 2;
                    t7 += 2;
                    ti1 = cc[ccoffset + t2] + cc[ccoffset + t5];
                    ti2 = cc[ccoffset + t2] - cc[ccoffset + t5];
                    ti3 = cc[ccoffset + t3] - cc[ccoffset + t4];
                    tr4 = cc[ccoffset + t3] + cc[ccoffset + t4];
                    tr1 = cc[ccoffset + t2 - 1] - cc[ccoffset + t5 - 1];
                    tr2 = cc[ccoffset + t2 - 1] + cc[ccoffset + t5 - 1];
                    ti4 = cc[ccoffset + t3 - 1] - cc[ccoffset + t4 - 1];
                    tr3 = cc[ccoffset + t3 - 1] + cc[ccoffset + t4 - 1];
                    ch[choffset + t7 - 1] = tr2 + tr3;
                    cr3 = tr2 - tr3;
                    ch[choffset + t7] = ti2 + ti3;
                    ci3 = ti2 - ti3;
                    cr2 = tr1 - tr4;
                    cr4 = tr1 + tr4;
                    ci2 = ti1 + ti4;
                    ci4 = ti1 - ti4;

                    ch[choffset + (t8 = t7 + t0) - 1] = wa1[wa1offset + i - 2] * cr2 - wa1[wa1offset + i - 1] * ci2;
                    ch[choffset + t8] = wa1[wa1offset + i - 2] * ci2 + wa1[wa1offset + i - 1] * cr2;
                    ch[choffset + (t8 += t0) - 1] = wa2[wa2offset + i - 2] * cr3 - wa2[wa2offset + i - 1] * ci3;
                    ch[choffset + t8] = wa2[wa2offset + i - 2] * ci3 + wa2[wa2offset + i - 1] * cr3;
                    ch[choffset + (t8 += t0) - 1] = wa3[wa3offset + i - 2] * cr4 - wa3[wa3offset + i - 1] * ci4;
                    ch[choffset + t8] = wa3[wa3offset + i - 2] * ci4 + wa3[wa3offset + i - 1] * cr4;
                }
                t1 += ido;
            }

            if (ido % 2 == 1) return;

        L105:

            t1 = ido;
            t2 = ido << 2;
            t3 = ido - 1;
            t4 = ido + (ido << 1);
            for (k = 0; k < l1; k++)
            {
                t5 = t3;
                ti1 = cc[ccoffset + t1] + cc[ccoffset + t4];
                ti2 = cc[ccoffset + t4] - cc[ccoffset + t1];
                tr1 = cc[ccoffset + t1 - 1] - cc[ccoffset + t4 - 1];
                tr2 = cc[ccoffset + t1 - 1] + cc[ccoffset + t4 - 1];
                ch[choffset + t5] = tr2 + tr2;
                ch[choffset + (t5 += t0)] = sqrt2 * (tr1 - ti1);
                ch[choffset + (t5 += t0)] = ti2 + ti2;
                ch[choffset + (t5 += t0)] = -sqrt2 * (tr1 + ti1);

                t3 += ido;
                t1 += t2;
                t4 += t2;
            }
        }

        static void dradbg(int ido, int ip, int l1, int idl1,
            float[] cc, int ccoffset,
            float[] c1, int c1offset,
            float[] c2, int c2offset,
            float[] ch, int choffset,
            float[] ch2, int ch2offset,
            float[] wa, int waoffset)
        {
            int idij, ipph, i, j, k, l, ik, t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10,
                t11, t12;
            int iz;
            float dc2, ai1, ai2, ar1, ar2, ds2;
            int nbd;
            float dcp, arg, dsp, ar1h, ar2h;
            int ipp2;

            t10 = ip * ido;
            t0 = l1 * ido;
            arg = tpi / (float)ip;
            dcp = (float)Math.Cos(arg);
            dsp = (float)Math.Sin(arg);
            nbd = (ido - 1) >> 1;
            ipp2 = ip;
            ipph = (ip + 1) >> 1;
            if (ido < l1) goto L103;

            t1 = 0;
            t2 = 0;
            for (k = 0; k < l1; k++)
            {
                t3 = t1;
                t4 = t2;
                for (i = 0; i < ido; i++)
                {
                    ch[choffset + t3] = cc[ccoffset + t4];
                    t3++;
                    t4++;
                }
                t1 += ido;
                t2 += t10;
            }
            goto L106;

        L103:
            t1 = 0;
            for (i = 0; i < ido; i++)
            {
                t2 = t1;
                t3 = t1;
                for (k = 0; k < l1; k++)
                {
                    ch[choffset + t2] = cc[ccoffset + t3];
                    t2 += ido;
                    t3 += t10;
                }
                t1++;
            }

        L106:
            t1 = 0;
            t2 = ipp2 * t0;
            t7 = (t5 = ido << 1);
            for (j = 1; j < ipph; j++)
            {
                t1 += t0;
                t2 -= t0;
                t3 = t1;
                t4 = t2;
                t6 = t5;
                for (k = 0; k < l1; k++)
                {
                    ch[choffset + t3] = cc[ccoffset + t6 - 1] + cc[ccoffset + t6 - 1];
                    ch[choffset + t4] = cc[ccoffset + t6] + cc[ccoffset + t6];
                    t3 += ido;
                    t4 += ido;
                    t6 += t10;
                }
                t5 += t7;
            }

            if (ido == 1) goto L116;
            if (nbd < l1) goto L112;

            t1 = 0;
            t2 = ipp2 * t0;
            t7 = 0;
            for (j = 1; j < ipph; j++)
            {
                t1 += t0;
                t2 -= t0;
                t3 = t1;
                t4 = t2;

                t7 += (ido << 1);
                t8 = t7;
                for (k = 0; k < l1; k++)
                {
                    t5 = t3;
                    t6 = t4;
                    t9 = t8;
                    t11 = t8;
                    for (i = 2; i < ido; i += 2)
                    {
                        t5 += 2;
                        t6 += 2;
                        t9 += 2;
                        t11 -= 2;
                        ch[choffset + t5 - 1] = cc[ccoffset + t9 - 1] + cc[ccoffset + t11 - 1];
                        ch[choffset + t6 - 1] = cc[ccoffset + t9 - 1] - cc[ccoffset + t11 - 1];
                        ch[choffset + t5] = cc[ccoffset + t9] - cc[ccoffset + t11];
                        ch[choffset + t6] = cc[ccoffset + t9] + cc[ccoffset + t11];
                    }
                    t3 += ido;
                    t4 += ido;
                    t8 += t10;
                }
            }
            goto L116;

        L112:
            t1 = 0;
            t2 = ipp2 * t0;
            t7 = 0;
            for (j = 1; j < ipph; j++)
            {
                t1 += t0;
                t2 -= t0;
                t3 = t1;
                t4 = t2;
                t7 += (ido << 1);
                t8 = t7;
                t9 = t7;
                for (i = 2; i < ido; i += 2)
                {
                    t3 += 2;
                    t4 += 2;
                    t8 += 2;
                    t9 -= 2;
                    t5 = t3;
                    t6 = t4;
                    t11 = t8;
                    t12 = t9;
                    for (k = 0; k < l1; k++)
                    {
                        ch[choffset + t5 - 1] = cc[ccoffset + t11 - 1] + cc[ccoffset + t12 - 1];
                        ch[choffset + t6 - 1] = cc[ccoffset + t11 - 1] - cc[ccoffset + t12 - 1];
                        ch[choffset + t5] = cc[ccoffset + t11] - cc[ccoffset + t12];
                        ch[choffset + t6] = cc[ccoffset + t11] + cc[ccoffset + t12];
                        t5 += ido;
                        t6 += ido;
                        t11 += t10;
                        t12 += t10;
                    }
                }
            }

        L116:
            ar1 = 1.0f;
            ai1 = 0.0f;
            t1 = 0;
            t9 = (t2 = ipp2 * idl1);
            t3 = (ip - 1) * idl1;
            for (l = 1; l < ipph; l++)
            {
                t1 += idl1;
                t2 -= idl1;

                ar1h = dcp * ar1 - dsp * ai1;
                ai1 = dcp * ai1 + dsp * ar1;
                ar1 = ar1h;
                t4 = t1;
                t5 = t2;
                t6 = 0;
                t7 = idl1;
                t8 = t3;
                for (ik = 0; ik < idl1; ik++)
                {
                    c2[c2offset + t4++] = ch2[ch2offset + t6++] + ar1 * ch2[ch2offset + t7++];
                    c2[c2offset + t5++] = ai1 * ch2[ch2offset + t8++];
                }
                dc2 = ar1;
                ds2 = ai1;
                ar2 = ar1;
                ai2 = ai1;

                t6 = idl1;
                t7 = t9 - idl1;
                for (j = 2; j < ipph; j++)
                {
                    t6 += idl1;
                    t7 -= idl1;
                    ar2h = dc2 * ar2 - ds2 * ai2;
                    ai2 = dc2 * ai2 + ds2 * ar2;
                    ar2 = ar2h;
                    t4 = t1;
                    t5 = t2;
                    t11 = t6;
                    t12 = t7;
                    for (ik = 0; ik < idl1; ik++)
                    {
                        c2[c2offset + t4++] += ar2 * ch2[ch2offset + t11++];
                        c2[c2offset + t5++] += ai2 * ch2[ch2offset + t12++];
                    }
                }
            }

            t1 = 0;
            for (j = 1; j < ipph; j++)
            {
                t1 += idl1;
                t2 = t1;
                for (ik = 0; ik < idl1; ik++) ch2[ch2offset + ik] += ch2[ch2offset + t2++];
            }

            t1 = 0;
            t2 = ipp2 * t0;
            for (j = 1; j < ipph; j++)
            {
                t1 += t0;
                t2 -= t0;
                t3 = t1;
                t4 = t2;
                for (k = 0; k < l1; k++)
                {
                    ch[choffset + t3] = c1[c1offset + t3] - c1[c1offset + t4];
                    ch[choffset + t4] = c1[c1offset + t3] + c1[c1offset + t4];
                    t3 += ido;
                    t4 += ido;
                }
            }

            if (ido == 1) goto L132;
            if (nbd < l1) goto L128;

            t1 = 0;
            t2 = ipp2 * t0;
            for (j = 1; j < ipph; j++)
            {
                t1 += t0;
                t2 -= t0;
                t3 = t1;
                t4 = t2;
                for (k = 0; k < l1; k++)
                {
                    t5 = t3;
                    t6 = t4;
                    for (i = 2; i < ido; i += 2)
                    {
                        t5 += 2;
                        t6 += 2;
                        ch[choffset + t5 - 1] = c1[c1offset + t5 - 1] - c1[c1offset + t6];
                        ch[choffset + t6 - 1] = c1[c1offset + t5 - 1] + c1[c1offset + t6];
                        ch[choffset + t5] = c1[c1offset + t5] + c1[c1offset + t6 - 1];
                        ch[choffset + t6] = c1[c1offset + t5] - c1[c1offset + t6 - 1];
                    }
                    t3 += ido;
                    t4 += ido;
                }
            }
            goto L132;

        L128:
            t1 = 0;
            t2 = ipp2 * t0;
            for (j = 1; j < ipph; j++)
            {
                t1 += t0;
                t2 -= t0;
                t3 = t1;
                t4 = t2;
                for (i = 2; i < ido; i += 2)
                {
                    t3 += 2;
                    t4 += 2;
                    t5 = t3;
                    t6 = t4;
                    for (k = 0; k < l1; k++)
                    {
                        ch[choffset + t5 - 1] = c1[c1offset + t5 - 1] - c1[c1offset + t6];
                        ch[choffset + t6 - 1] = c1[c1offset + t5 - 1] + c1[c1offset + t6];
                        ch[choffset + t5] = c1[c1offset + t5] + c1[c1offset + t6 - 1];
                        ch[choffset + t6] = c1[c1offset + t5] - c1[c1offset + t6 - 1];
                        t5 += ido;
                        t6 += ido;
                    }
                }
            }

        L132:
            if (ido == 1) return;

            for (ik = 0; ik < idl1; ik++) c2[c2offset + ik] = ch2[ch2offset + ik];

            t1 = 0;
            for (j = 1; j < ip; j++)
            {
                t2 = (t1 += t0);
                for (k = 0; k < l1; k++)
                {
                    c1[c1offset + t2] = ch[choffset + t2];
                    t2 += ido;
                }
            }

            if (nbd > l1) goto L139;

            iz = -ido - 1;
            t1 = 0;
            for (j = 1; j < ip; j++)
            {
                iz += ido;
                t1 += t0;
                idij = iz;
                t2 = t1;
                for (i = 2; i < ido; i += 2)
                {
                    t2 += 2;
                    idij += 2;
                    t3 = t2;
                    for (k = 0; k < l1; k++)
                    {
                        c1[c1offset + t3 - 1] = wa[waoffset + idij - 1] * ch[choffset + t3 - 1] - wa[waoffset + idij] * ch[choffset + t3];
                        c1[c1offset + t3] = wa[waoffset + idij - 1] * ch[choffset + t3] + wa[waoffset + idij] * ch[choffset + t3 - 1];
                        t3 += ido;
                    }
                }
            }
            return;

        L139:
            iz = -ido - 1;
            t1 = 0;
            for (j = 1; j < ip; j++)
            {
                iz += ido;
                t1 += t0;
                t2 = t1;
                for (k = 0; k < l1; k++)
                {
                    idij = iz;
                    t3 = t2;
                    for (i = 2; i < ido; i += 2)
                    {
                        idij += 2;
                        t3 += 2;
                        c1[c1offset + t3 - 1] = wa[waoffset + idij - 1] * ch[choffset + t3 - 1] - wa[waoffset + idij] * ch[choffset + t3];
                        c1[c1offset + t3] = wa[waoffset + idij - 1] * ch[choffset + t3] + wa[waoffset + idij] * ch[choffset + t3 - 1];
                    }
                    t2 += ido;
                }
            }
        }

        static void drftb1(int n,
            float[] c, int coffset,
            float[] ch, int choffset,
            float[] wa, int waoffset,
            int[] ifac)
        {
            int i, k1, l1, l2;
            int na;
            int nf, ip, iw, ix2, ix3, ido, idl1;

            nf = ifac[1];
            na = 0;
            l1 = 1;
            iw = 1;

            for (k1 = 0; k1 < nf; k1++)
            {
                ip = ifac[k1 + 2];
                l2 = ip * l1;
                ido = n / l2;
                idl1 = ido * l1;
                if (ip != 4) goto L103;
                ix2 = iw + ido;
                ix3 = ix2 + ido;

                if (na != 0)
                    dradb4(ido, l1, ch, choffset, c, coffset, wa, waoffset + iw - 1, wa, waoffset + ix2 - 1, wa, waoffset + ix3 - 1);
                else
                    dradb4(ido, l1, c, coffset, ch, choffset, wa, waoffset + iw - 1, wa, waoffset + ix2 - 1, wa, waoffset + ix3 - 1);
                na = 1 - na;
                goto L115;

            L103:
                if (ip != 2) goto L106;

                if (na != 0)
                    dradb2(ido, l1, ch, choffset, c, coffset, wa, waoffset + iw - 1);
                else
                    dradb2(ido, l1, c, coffset, ch, choffset, wa, waoffset + iw - 1);
                na = 1 - na;
                goto L115;

            L106:
                if (ip != 3) goto L109;

                ix2 = iw + ido;
                if (na != 0)
                    dradb3(ido, l1, ch, choffset, c, coffset, wa, waoffset + iw - 1, wa, waoffset + ix2 - 1);
                else
                    dradb3(ido, l1, c, coffset, ch, choffset, wa, waoffset + iw - 1, wa, waoffset + ix2 - 1);
                na = 1 - na;
                goto L115;

            L109:
                /*    The radix five case can be translated later..... */
                /*    if(ip!=5)goto L112;

                    ix2=iw+ido;
                    ix3=ix2+ido;
                    ix4=ix3+ido;
                    if(na!=0)
                      dradb5(ido,l1,ch,c,wa+iw-1,wa+ix2-1,wa+ix3-1,wa+ix4-1);
                    else
                      dradb5(ido,l1,c,ch,wa+iw-1,wa+ix2-1,wa+ix3-1,wa+ix4-1);
                    na=1-na;
                    goto L115;

                  L112:*/
                if (na != 0)
                    dradbg(ido, ip, l1, idl1, ch, choffset, ch, choffset, ch, choffset, c, coffset, c, coffset, wa, waoffset + iw - 1);
                else
                    dradbg(ido, ip, l1, idl1, c, coffset, c, coffset, c, coffset, ch, choffset, ch, choffset, wa, waoffset + iw - 1);
                if (ido == 1) na = 1 - na;

            L115:
                l1 = l2;
                iw += (ip - 1) * ido;
            }

            if (na == 0) return;

            for (i = 0; i < n; i++) c[coffset + i] = ch[choffset + i];
        }

        static void spx_drft_forward(drft_lookup l, float[] data, int dataoffset)
        {
            if (l.n == 1) return;
            drftf1(l.n, data, dataoffset, l.trigcache, 0, l.trigcache, l.n, l.splitcache);
        }

        static void spx_drft_backward(drft_lookup l, float[] data, int dataoffset)
        {
            if (l.n == 1) return;
            drftb1(l.n, data, dataoffset, l.trigcache, 0, l.trigcache, l.n, l.splitcache);
        }

        static void spx_drft_init(drft_lookup l, int n)
        {
            l.n = n;
            l.trigcache = new float[3 * n]; //(float*)speex_alloc(3*n*sizeof(*l.trigcache));
            l.splitcache = new int[32]; // (int*)speex_alloc(32*sizeof(*l.splitcache));
            fdrffti(n, l.trigcache, l.splitcache);
        }

        private static void spx_drft_clear(drft_lookup l)
        {
            // ks 9/29/10 - Not much to do here, what with GC and all.
            //if(l != null)
            //{
            //  if(l.trigcache != null)
            //    speex_free(l.trigcache);
            //  if(l.splitcache)
            //    speex_free(l.splitcache);
            //}
        }


    }
}
