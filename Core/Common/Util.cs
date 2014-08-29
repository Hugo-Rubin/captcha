using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Core.Common
{
    public static class Util
    {
        public static void Init<T>(this T[] array, T defaultVaue)
        {
            if (array == null)
                return;
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = defaultVaue;
            }
        }

        public static Bitmap MatrizParaBitmap(this int[,] matriz, int imgWidth, int imgHeight)
        {
            var bmp = new Bitmap(imgWidth, imgHeight, PixelFormat.Format24bppRgb);

            for (var y = 0; y < matriz.GetLength(1); y++)
            {
                for (var x = 0; x < matriz.GetLength(0); x++)
                {
                    Color c;
                    var px = matriz[x, y];
                    switch (px)
                    {
                        case 0:
                            c = Color.Black;
                            break;
                        case 255:
                            c = Color.White;
                            break;
                        default:
                            c = Color.FromArgb(px, px, px);
                            break;
                    }

                    bmp.SetPixel(x, y, c);
                }
            }
            return bmp;
        }

        public static Bitmap CreateBitmapFromNanoArray(this int[] imgNanoArray, int width, int height)
        {
            var result = ConvertNanoArrayToImgArray(imgNanoArray).ToArray();
            return result.CreateBitmap(width, height);
        }

        public static Bitmap CreateBitmap(this int[] imgArray, int width, int height)
        {
            var bmp = new Bitmap(width, height);

            var k = 0;
            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var c = Color.White;

                    if (imgArray[k++] == 0)
                    {
                        c = Color.Black;
                    }

                    bmp.SetPixel(x, y, c);
                }
            }

            return bmp;
        }

        public static Bitmap CreateBitmap(this byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return null;
            }
            return (Bitmap)Image.FromStream(new MemoryStream(byteArray));
        }

        private static IEnumerable<int> ConvertNanoArrayToImgArray(IEnumerable<int> nanoArray)
        {
            // Converter NanoArray em imgArray
            // Indice Par = Brancos
            // Indice impar = Pretos
            var idx = 0;
            foreach (var item in nanoArray)
            {
                if (idx % 2 == 0)
                {
                    for (var i = 0; i < item; i++)
                    {
                        yield return 1;
                    }
                }
                else
                {
                    for (var i = 0; i < item; i++)
                    {
                        yield return 0;
                    }
                }
                idx++;
            }
        }
    }
}