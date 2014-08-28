using System;
using System.Drawing;
using Core.Logic.Types;

namespace Core.Logic.Filtros
{
    [Serializable]
    public class GaussianBlur
    {
        private readonly BlurType blurType;
        private readonly int radius = 25;
        private int[] kernel;
        private int kernelSum;
        private int[,] multable;

        public GaussianBlur()
        {
            PreCalculateSomeStuff();
        }

        public GaussianBlur(int radius, BlurType blurType = BlurType.Both)
        {
            if (radius < 0)
            {
                throw new Exception("GaussianBlur.radius nao pode ser negativo.");
            }
            this.radius = radius;
            this.blurType = blurType;
            PreCalculateSomeStuff();
        }

        private void PreCalculateSomeStuff()
        {
            var sz = radius * 2 + 1;
            kernel = new int[sz];
            multable = new int[sz, 256];
            for (var i = 1; i <= radius; i++)
            {
                var szi = radius - i;
                var szj = radius + i;
                kernel[szj] = kernel[szi] = (szi + 1) * (szi + 1);
                kernelSum += (kernel[szj] + kernel[szi]);
                for (var j = 0; j < 256; j++)
                {
                    multable[szj, j] = multable[szi, j] = kernel[szj] * j;
                }
            }
            kernel[radius] = (radius + 1) * (radius + 1);
            kernelSum += kernel[radius];
            for (var j = 0; j < 256; j++)
            {
                multable[radius, j] = kernel[radius] * j;
            }
        }

        public Bitmap ProcessImage(Image inputImage)
        {
            var origin = new Bitmap(inputImage);
            var blurred = new Bitmap(inputImage.Width, inputImage.Height);

            using (var src = new RawBitmap(origin))
            {
                using (var dest = new RawBitmap(blurred))
                {
                    var pixelCount = src.Width * src.Height;
                    //Stopwatch sw = new Stopwatch();
                    //sw.Start();
                    var b = new int[pixelCount];
                    var g = new int[pixelCount];
                    var r = new int[pixelCount];

                    var b2 = new int[pixelCount];
                    var g2 = new int[pixelCount];
                    var r2 = new int[pixelCount];
                    //sw.Stop();
                    //t1 = sw.ElapsedMilliseconds;

                    var offset = src.GetOffset();
                    var index = 0;
                    unsafe
                    {
                        //sw.Reset();
                        //sw.Start();

                        var ptr = src.Begin;
                        for (var i = 0; i < src.Height; i++)
                        {
                            for (var j = 0; j < src.Width; j++)
                            {
                                b[index] = *ptr;
                                ptr++;
                                g[index] = *ptr;
                                ptr++;
                                r[index] = *ptr;
                                ptr++;

                                ++index;
                            }
                            ptr += offset;
                        }

                        //sw.Stop();
                        //t2 = sw.ElapsedMilliseconds;

                        int bsum;
                        int gsum;
                        int rsum;
                        int read;
                        var start = 0;
                        index = 0;

                        //sw.Reset();
                        //sw.Start();

                        if (blurType != BlurType.VerticalOnly)
                        {
                            for (var i = 0; i < src.Height; i++)
                            {
                                for (var j = 0; j < src.Width; j++)
                                {
                                    bsum = gsum = rsum = 0;
                                    read = index - radius;

                                    for (var z = 0; z < kernel.Length; z++)
                                    {
                                        //if (read >= start && read < start + src.Width)
                                        //{
                                        //    bsum += _multable[z, b[read]];
                                        //    gsum += _multable[z, g[read]];
                                        //    rsum += _multable[z, r[read]];
                                        //    sum += _kernel[z];
                                        //}

                                        if (read < start)
                                        {
                                            bsum += multable[z, b[start]];
                                            gsum += multable[z, g[start]];
                                            rsum += multable[z, r[start]];
                                        }
                                        else if (read > start + src.Width - 1)
                                        {
                                            var idx = start + src.Width - 1;
                                            bsum += multable[z, b[idx]];
                                            gsum += multable[z, g[idx]];
                                            rsum += multable[z, r[idx]];
                                        }
                                        else
                                        {
                                            bsum += multable[z, b[read]];
                                            gsum += multable[z, g[read]];
                                            rsum += multable[z, r[read]];
                                        }
                                        ++read;
                                    }

                                    //b2[index] = (bsum / sum);
                                    //g2[index] = (gsum / sum);
                                    //r2[index] = (rsum / sum);

                                    b2[index] = (bsum / kernelSum);
                                    g2[index] = (gsum / kernelSum);
                                    r2[index] = (rsum / kernelSum);

                                    if (blurType == BlurType.HorizontalOnly)
                                    {
                                        //byte* pcell = dest[j, i];
                                        //*pcell = (byte)(bsum / sum);
                                        //pcell++;
                                        //*pcell = (byte)(gsum / sum);
                                        //pcell++;
                                        //*pcell = (byte)(rsum / sum);
                                        //pcell++;

                                        var pcell = dest[j, i];
                                        *pcell = (byte)(bsum / kernelSum);
                                        pcell++;
                                        *pcell = (byte)(gsum / kernelSum);
                                        pcell++;
                                        *pcell = (byte)(rsum / kernelSum);
                                    }

                                    ++index;
                                }
                                start += src.Width;
                            }
                        }
                        if (blurType == BlurType.HorizontalOnly)
                        {
                            return blurred;
                        }

                        //sw.Stop();
                        //t3 = sw.ElapsedMilliseconds;

                        //sw.Reset();
                        //sw.Start();

                        for (var i = 0; i < src.Height; i++)
                        {
                            var y = i - radius;
                            start = y * src.Width;
                            for (var j = 0; j < src.Width; j++)
                            {
                                bsum = gsum = rsum = 0;
                                read = start + j;
                                var tempy = y;
                                for (var z = 0; z < kernel.Length; z++)
                                {
                                    //if (tempy >= 0 && tempy < src.Height)
                                    //{
                                    //    if (_blurType == BlurType.VerticalOnly)
                                    //    {
                                    //        bsum += _multable[z, b[read]];
                                    //        gsum += _multable[z, g[read]];
                                    //        rsum += _multable[z, r[read]];
                                    //    }
                                    //    else
                                    //    {
                                    //        bsum += _multable[z, b2[read]];
                                    //        gsum += _multable[z, g2[read]];
                                    //        rsum += _multable[z, r2[read]];
                                    //    }
                                    //    sum += _kernel[z];
                                    //}

                                    if (blurType == BlurType.VerticalOnly)
                                    {
                                        if (tempy < 0)
                                        {
                                            bsum += multable[z, b[j]];
                                            gsum += multable[z, g[j]];
                                            rsum += multable[z, r[j]];
                                        }
                                        else if (tempy > src.Height - 1)
                                        {
                                            var idx = pixelCount - (src.Width - j);
                                            bsum += multable[z, b[idx]];
                                            gsum += multable[z, g[idx]];
                                            rsum += multable[z, r[idx]];
                                        }
                                        else
                                        {
                                            bsum += multable[z, b[read]];
                                            gsum += multable[z, g[read]];
                                            rsum += multable[z, r[read]];
                                        }
                                    }
                                    else
                                    {
                                        if (tempy < 0)
                                        {
                                            bsum += multable[z, b2[j]];
                                            gsum += multable[z, g2[j]];
                                            rsum += multable[z, r2[j]];
                                        }
                                        else if (tempy > src.Height - 1)
                                        {
                                            var idx = pixelCount - (src.Width - j);
                                            bsum += multable[z, b2[idx]];
                                            gsum += multable[z, g2[idx]];
                                            rsum += multable[z, r2[idx]];
                                        }
                                        else
                                        {
                                            bsum += multable[z, b2[read]];
                                            gsum += multable[z, g2[read]];
                                            rsum += multable[z, r2[read]];
                                        }
                                    }


                                    read += src.Width;
                                    ++tempy;
                                }

                                var pcell = dest[j, i];

                                //pcell[0] = (byte)(bsum / sum);
                                //pcell[1] = (byte)(gsum / sum);
                                //pcell[2] = (byte)(rsum / sum);

                                pcell[0] = (byte)(bsum / kernelSum);
                                pcell[1] = (byte)(gsum / kernelSum);
                                pcell[2] = (byte)(rsum / kernelSum);
                            }
                        }
                        //sw.Stop();
                        //t4 = sw.ElapsedMilliseconds;
                    }
                }
            }

            return blurred;
        }
    }
}