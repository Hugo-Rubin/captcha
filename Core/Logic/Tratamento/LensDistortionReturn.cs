using System;
using System.Drawing;
using Core.Logic.Filtros;
using Core.Logic.Types;

namespace Core.Logic.Tratamento
{
    public class LensDistortionReturn
    {
        private const double StdevR = 35; //50
        private const double StdevC = 35; //46
        private static int meanR;
        private static int meanC;

        public static ImgArray Calculate(ImgArray imgIn)
        {
            meanC = imgIn.Width / 2;
            meanR = imgIn.Height / 2;
            var imgOut = new ImgArray(imgIn.Width, imgIn.Height);

            for (var i = 0; i < imgOut.Width; i++)
            {
                for (var j = 0; j < imgOut.Height; j++)
                {
                    imgOut.SetPixel(i, j, Color.FromArgb(255, 255, 255, 255));
                }
            }

            for (var y = 0; y < imgIn.Width; y++)
                for (var x = 0; x < imgIn.Height; x++)
                {
                    // Gaussian-weighted shifting towards image[image.height/2][image.width/2]
                    var scale = Gauss.Calculate(meanR, StdevR, x) * Gauss.Calculate(meanC, StdevC, y);
                    var newX = x + (meanR - x) * scale;
                    var newY = y + (meanC - y) * scale;

                    var newYf = (int)Math.Floor(newY);
                    var newXf = (int)Math.Floor(newX);

                    var newYc = (int)Math.Ceiling(newY); // or simply newRowF+1
                    var newXc = (int)Math.Ceiling(newX); // or simply newColF+1

                    var c = imgIn.GetPixel(y, x);
                    imgOut.SetPixel(newYf, newXf, c);
                    imgOut.SetPixel(newYc, newXc, c);
                }
            return imgOut;
        }
    }
}