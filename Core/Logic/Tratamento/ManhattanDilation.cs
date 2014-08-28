using System;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Tratamento
{
    public class ManhattanDilation : IDilation
    {
        #region Dilation Members

        public ImgArray Apply(ImgArray img, int k)
        {
            var image = Manhattan(img.ToIntArray());
            for (var i = 0; i < image.GetLength(0); i++)
            {
                for (var j = 0; j < image.GetLength(1); j++)
                {
                    image[i, j] = ((image[i, j] <= k) ? 1 : 0);
                }
            }
            return image.ToImgArray();
        }

        #endregion

        private int[,] Manhattan(int[,] image)
        {
            // traverse from top left to bottom right
            for (var i = 0; i < image.GetLength(0); i++)
            {
                for (var j = 0; j < image.GetLength(1); j++)
                {
                    if (image[i, j] == 1)
                    {
                        // first pass and pixel was on, it gets a zero
                        image[i, j] = 0;
                    }
                    else
                    {
                        // pixel was off
                        // It is at most the sum of the lengths of the array
                        // away from a pixel that is on
                        image[i, j] = image.GetLength(0) + image.GetLength(1);
                        // or one more than the pixel to the north
                        if (i > 0) image[i, j] = Math.Min(image[i, j], image[i - 1, j] + 1);
                        // or one more than the pixel to the west
                        if (j > 0) image[i, j] = Math.Min(image[i, j], image[i, j - 1] + 1);
                    }
                }
            }
            // traverse from bottom right to top left
            for (var i = image.GetLength(0) - 1; i >= 0; i--)
            {
                for (var j = image.GetLength(1) - 1; j >= 0; j--)
                {
                    // either what we had on the first pass
                    // or one more than the pixel to the south
                    if (i + 1 < image.GetLength(0)) image[i, j] = Math.Min(image[i, j], image[i + 1, j] + 1);
                    // or one more than the pixel to the east
                    if (j + 1 < image.GetLength(1)) image[i, j] = Math.Min(image[i, j], image[i, j + 1] + 1);
                }
            }
            return image;
        }
    }
}