using System.Drawing;

namespace Core.Common.Extensions
{
    public static class ColorExtension
    {
        public static int Brightness(this Color pixel)
        {
            return (pixel.R * 299 / 1000) + (pixel.G * 587 / 1000) + (pixel.B * 114 / 1000);
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
    }
}
