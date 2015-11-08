using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Logic.Tratamento
{
    class CircleHough
    {
        int[] input;
        long[] output;
        float[] template = { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
        int width;
        int height;
        long[] acc;
        int accSize = 30;
        long[] results;
        int r;

        public void Init(int[] inputIn, int widthIn, int heightIn, int radius)
        {
            r = radius;
            width = widthIn;
            height = heightIn;
            input = new int[width * height];
            output = new long[width * height];
            input = inputIn;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    output[x + (width * y)] = 0xff000000;
                }
            }
        }

        public void SetLines(int lines)
        {
            accSize = lines;
        }

        // hough transform for lines (polar), returns the accumulator array
        public long[] Process()
        {
            // for polar we need accumulator of 180degress * the longest length in the image
            int rmax = (int)Math.Sqrt(width * width + height * height);
            acc = new long[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    acc[y * width + x] = 0;
                }
            }
            int x0, y0;
            double t;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    if ((input[y * width + x] & 0xff) == 255)
                    {

                        for (int theta = 0; theta < 360; theta++)
                        {
                            t = (theta * 3.14159265) / 180;
                            x0 = (int)Math.Round(x - r * Math.Cos(t));
                            y0 = (int)Math.Round(y - r * Math.Sin(t));
                            if (x0 < width && x0 > 0 && y0 < height && y0 > 0)
                            {
                                acc[x0 + (y0 * width)] += 1;
                            }
                        }
                    }
                }
            }

            // now normalise to 255 and put in format for a pixel array
            long max = 0;

            // Find max acc value
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    if (acc[x + (y * width)] > max)
                    {
                        max = acc[x + (y * width)];
                    }
                }
            }

            //System.out.println("Max :" + max);

            // Normalise all the values
            int value;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    value = (int)(((double)acc[x + (y * width)] / (double)max) * 255.0);
                    acc[x + (y * width)] = 0xff000000 | (value << 16 | value << 8 | value);
                }
            }
            FindMaxima();

            /*byte[] image = new byte[output.Length];
            for (int i = 0; i < output.Length; i++)
            {
                if (output[i] == 4278190080)
                    image[i] = 1;
            }
            Core.Logic.Types.ImgArray img = new Types.ImgArray(image, width, height);
            img.Save(@"E:\OCR\z1.png");*/

            return output;
        }
        private int[] FindMaxima()
        {
            results = new long[accSize * 3];
            int[] output = new int[width * height];


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    long value = (acc[x + (y * width)] & 0xff);

                    // if its higher than lowest value add it and then sort
                    if (value > results[(accSize - 1) * 3])
                    {

                        // add to bottom of array
                        results[(accSize - 1) * 3] = value;
                        results[(accSize - 1) * 3 + 1] = x;
                        results[(accSize - 1) * 3 + 2] = y;

                        // shift up until its in right place
                        int i = (accSize - 2) * 3;
                        while ((i >= 0) && (results[i + 3] > results[i]))
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                long temp = results[i + j];
                                results[i + j] = results[i + 3 + j];
                                results[i + 3 + j] = temp;
                            }
                            i = i - 3;
                            if (i < 0) break;
                        }
                    }
                }
            }

            double ratio = (double)(width / 2) / accSize;
            //System.Windows.Forms.MessageBox.Show("top "+accSize+" matches:");
            for (int i = accSize - 1; i >= 0; i--)
            {
                //System.out.println("value: " + results[i*3] + ", r: " + results[i*3+1] + ", theta: " + results[i*3+2]);
                drawCircle(results[i * 3], results[i * 3 + 1], results[i * 3 + 2]);
            }
            return output;
        }

        private void SetPixel(long value, long xPos, long yPos)
        {
            output[(yPos * width) + xPos] = 0xff000000 | (value << 16 | value << 8 | value);
        }

        // draw circle at x y
        private void drawCircle(long pix, long xCenter, long yCenter)
        {
            pix = 250;

            int x, y, r2;
            int radius = r;
            r2 = r * r;
            SetPixel(pix, xCenter, yCenter + radius);
            SetPixel(pix, xCenter, yCenter - radius);
            SetPixel(pix, xCenter + radius, yCenter);
            SetPixel(pix, xCenter - radius, yCenter);

            y = radius;
            x = 1;
            y = (int)(Math.Sqrt(r2 - 1) + 0.5);
            while (x < y)
            {
                SetPixel(pix, xCenter + x, yCenter + y);
                SetPixel(pix, xCenter + x, yCenter - y);
                SetPixel(pix, xCenter - x, yCenter + y);
                SetPixel(pix, xCenter - x, yCenter - y);
                SetPixel(pix, xCenter + y, yCenter + x);
                SetPixel(pix, xCenter + y, yCenter - x);
                SetPixel(pix, xCenter - y, yCenter + x);
                SetPixel(pix, xCenter - y, yCenter - x);
                x += 1;
                y = (int)(Math.Sqrt(r2 - x * x) + 0.5);
            }
            if (x == y)
            {
                SetPixel(pix, xCenter + x, yCenter + y);
                SetPixel(pix, xCenter + x, yCenter - y);
                SetPixel(pix, xCenter - x, yCenter + y);
                SetPixel(pix, xCenter - x, yCenter - y);
            }
        }

        public long[] GetAcc()
        {
            return acc;
        }


    }
}
