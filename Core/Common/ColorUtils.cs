#region

using System;
using System.Drawing;
using System.Drawing.Imaging;

#endregion

namespace Core.Common
{
    public static class ColorUtils
    {
        [Obsolete("Cuidado, metodo lento")]
        public static Bitmap PreencherPixels(Bitmap source, int amount = 1)
        {
            var imgArray = new ImgArrayLite(source);


            Color right, up, down;
            var left = right = up = down = imgArray.GetPixel(0, 0);
            var preto = Color.Black;

            bool test2, sentido;
            var test1 = test2 = sentido = false;
            var count = 0;

            try
            {
                do
                {
                    for (var y = 0; y < imgArray.Height; y++)
                    {
                        for (var x = 0; x < imgArray.Width; x++)
                        {
                            var c = imgArray.GetPixel(x, y);

                            if (x > 0 && x < imgArray.Width - 1)
                            {
                                left = imgArray.GetPixel(x - 1, y);
                                right = imgArray.GetPixel(x + 1, y);
                                test1 = true;
                            }
                            if (y > 0 && y < imgArray.Height - 1)
                            {
                                up = imgArray.GetPixel(x, y + 1);
                                down = imgArray.GetPixel(x, y - 1);
                                test2 = true;
                            }

                            if (c.IsBlackPixel())
                            {
                                if (test2)
                                {
                                    if (!up.IsBlackPixel()
                                        && imgArray.GetPixel(x, y + 2).IsBlackPixel()
                                        && !imgArray.GetPixel(x, y + 4).IsBlackPixel())
                                    {
                                        imgArray.SetPixel(x, y + 1, preto);
                                    }

                                    if (y > 1 && !down.IsBlackPixel()
                                        && imgArray.GetPixel(x, y - 2).IsBlackPixel())
                                    {
                                        imgArray.SetPixel(x, y - 1, preto);
                                    }
                                }

                                if (test1)
                                {
                                    if (x > 1 && !left.IsBlackPixel() && imgArray.GetPixel(x - 2, y).IsBlackPixel())
                                        imgArray.SetPixel(x - 1, y, preto);
                                    if (!right.IsBlackPixel() && imgArray.GetPixel(x + 2, y).IsBlackPixel())
                                        imgArray.SetPixel(x + 1, y, preto);
                                }
                            }
                        }
                    }

                    for (var y = 0; y < imgArray.Height - 1; y++)
                    {
                        for (var x = 0; x < imgArray.Width - 1; x++)
                        {
                            var c = imgArray.GetPixel(x, y);

                            if (x > 0 && x < imgArray.Width - 1)
                            {
                                left = imgArray.GetPixel(x - 1, y);
                                right = imgArray.GetPixel(x + 1, y);
                                test1 = true;
                            }

                            if (y > 0 && y < imgArray.Height - 1)
                            {
                                up = imgArray.GetPixel(x, y + 1);
                                down = imgArray.GetPixel(x, y - 1);
                                test2 = true;
                            }

                            if (c.IsBlackPixel())
                            {
                                if (test2 && sentido)
                                {
                                    if (!down.IsBlackPixel() && up.IsBlackPixel())
                                    {
                                        imgArray.SetPixel(x, y - 1, preto);
                                    }

                                    sentido = false;
                                }
                                if (test1 && sentido == false)
                                {
                                    if (!left.IsBlackPixel() && right.IsBlackPixel())
                                    {
                                        imgArray.SetPixel(x - 1, y, preto);
                                    }

                                    sentido = true;
                                }
                            }
                        }
                    }
                    count++;
                } while (count < amount);
            }
            catch (Exception e)
            {
                Console.Write("Preencher pixels: " + e.Message);
            }
            return imgArray.ToBitmap();
        }

        public static int BrilhoDoPixel(Color pixel)
        {
            return (pixel.R*299/1000) + (pixel.G*587/1000) + (pixel.B*114/1000);
        }

        public static bool RGBEquals(this Color color1, Color color2)
        {
            return (color1.R == color2.R
                    && color1.G == color2.G
                    && color1.B == color2.B);
        }

        public static int CountPixelsWithColor(this Bitmap bmp, Color color)
        {
            var count = 0;
            for (var y = 0; y < bmp.Height; y++)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    if (bmp.GetPixel(x, y).RGBEquals(color))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        [Obsolete("Atenção! Este método usa Bitmap, mas é limpinho.")]
        public static void Posterize(Bitmap img, byte levels)
        {
            var bitdata = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                                              ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            const int pixelSize = 4;
            var numAreas = 256.0/levels;
            var numValues = 255.0/(levels - 1);

            unsafe
            {
                for (var y = 0; y < bitdata.Height; y++)
                {
                    var destPixels = (byte*) bitdata.Scan0 + (y*bitdata.Stride);
                    for (var x = 0; x < bitdata.Width; x++)
                    {
                        //(byte)(destPixels[x * PixelSize + 1] / (255 / levels));
                        //destPixels[x * PixelSize+2] < 85 ? (byte)85 : (destPixels[x * PixelSize+2] < 170 ? (byte)170 : (byte)255); // R 
                        var oldBlue = destPixels[x*pixelSize];
                        var blueAreaDouble = oldBlue/numAreas;
                        var blueArea = (int) Math.Round(blueAreaDouble);
                        blueArea = blueArea > blueAreaDouble ? blueArea - 1 : blueArea;
                        var newBlueDouble = numValues*blueArea;
                        var newBlue = (int) Math.Round(newBlueDouble);
                        newBlue = newBlue > newBlueDouble ? newBlue - 1 : newBlue;
                        destPixels[x*pixelSize] = (byte) newBlue;

                        var oldGreen = destPixels[x*pixelSize + 1];
                        var greenAreaDouble = oldGreen/numAreas;
                        var greenArea = (int) Math.Round(greenAreaDouble);
                        greenArea = greenArea > greenAreaDouble ? greenArea - 1 : greenArea;
                        var newgreenDouble = numValues*greenArea;
                        var newgreen = (int) Math.Round(newgreenDouble);
                        newgreen = newgreen > newgreenDouble ? newgreen - 1 : newgreen;
                        destPixels[x*pixelSize + 1] = (byte) newgreen;

                        var oldRed = destPixels[x*pixelSize + 2];
                        var redAreaDouble = oldRed/numAreas;
                        var redArea = (int) Math.Round(redAreaDouble);
                        redArea = redArea > redAreaDouble ? redArea - 1 : redArea;
                        var newredDouble = numValues*redArea;
                        var newred = (int) Math.Round(newredDouble);
                        newred = newred > newredDouble ? newred - 1 : newred;
                        destPixels[x*pixelSize + 2] = (byte) newred;

                        //destPixels[x * PixelSize] = (byte)(destPixels[x * PixelSize] / (255 / levels)); // B 
                        //destPixels[x * PixelSize + 1] = (byte)(destPixels[x * PixelSize + 1] / (255 / levels)); // G
                        //destPixels[x * PixelSize + 2] = (byte)(destPixels[x * PixelSize + 2] / (255 / levels)); // R
                        //destPixels[x * PixelSize + 3] = contrast_lookup[destPixels[x * PixelSize + 3]]; //A 
                    }
                }
                img.UnlockBits(bitdata);
            }
        }

        [Obsolete("Atenção! Este método usa Bitmap, mas é limpinho.")]
        public static Bitmap Posterize2(Bitmap img, byte levels)
        {
            var arHistogram = new long[256];

            for (var i = 0; i < arHistogram.Length; i++)
            {
                arHistogram[i] = 0;
            }

            long presentLevels = 0;

            var bitdata = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                                              ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            const int pixelSize = 4;

            unsafe
            {
                for (var y = 0; y < bitdata.Height; y++)
                {
                    var destPixels = (byte*) bitdata.Scan0 + (y*bitdata.Stride);
                    for (var x = 0; x < bitdata.Width; x++)
                    {
                        int red = destPixels[x*pixelSize + 2];
                        int green = destPixels[x*pixelSize + 1];
                        int blue = destPixels[x*pixelSize];

                        if (!(red >= 0 && red <= 255 && green >= 0 && green <= 255 && blue >= 0 && blue <= 255))
                        {
                            return img;
                        }

                        arHistogram[red]++;
                        arHistogram[green]++;
                        arHistogram[blue]++;
                    }
                }
            }

            img.UnlockBits(bitdata);

            foreach (var histo in arHistogram)
            {
                if (histo != 0)
                {
                    presentLevels++;
                }
            }

            if (!(presentLevels <= 256 && presentLevels >= 0))
            {
                return img;
            }

            if (levels >= presentLevels)
            {
                //If we actually have less than we're trying to remove
                return img; //We're done
            }

            var numAreas = (short) (256.0/levels);
            var numValues = (short) (255.0/(levels - 1));

            bitdata = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                                   ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unsafe
            {
                for (var y = 0; y < bitdata.Height; y++)
                {
                    var destPixels = (byte*) bitdata.Scan0 + (y*bitdata.Stride);
                    for (var x = 0; x < bitdata.Width; x++)
                    {
                        int red = destPixels[x*pixelSize + 2];
                        var currentArea = (short) Math.Floor((double) (red/numAreas));
                        double newValue = numValues*currentArea;

                        //Will take care of wild colors
                        if (newValue > 253) newValue = 255;
                        else if (newValue < 2) newValue = 0;

                        //Assign values
                        red = (int) newValue;

                        int green = destPixels[x*pixelSize + 1];
                        currentArea = (short) Math.Floor((double) (green/numAreas));
                        newValue = numValues*currentArea;
                        if (newValue > 253) newValue = 255;
                        else if (newValue < 2) newValue = 0;
                        green = (int) newValue;

                        int blue = destPixels[x*pixelSize];
                        currentArea = (short) Math.Floor((double) (blue/numAreas));
                        newValue = numValues*currentArea;
                        if (newValue > 253) newValue = 255;
                        else if (newValue < 2) newValue = 0;
                        blue = (int) newValue;

                        destPixels[x*pixelSize] = (byte) blue; //Color blend was finished earlier
                        destPixels[x*pixelSize + 1] = (byte) green;
                        destPixels[x*pixelSize + 2] = (byte) red;
                    }
                }
            }

            img.UnlockBits(bitdata);
            return img;
        }
    }
}