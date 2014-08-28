using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Core.Logic.Tratamento
{
    public class ICMMRF : ICM
    {
        #region ICM Members

        public Bitmap Apply(Bitmap img, double maxDiff, double weightDiff, int iterations, double covar)
        {
            // Maintain two buffer images.
            // In alternate iterations, one will be the
            // source image, the other the destination.
            var buffer = new Bitmap[2];
            buffer[0] = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format32bppArgb);
            buffer[1] = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
            var source = 1;
            var destination = 0;

            // This value is guaranteed to be larger than the
            // potential of any configuration of pixel values.
            var vMax = img.Width*img.Height*(Math.Pow(256, 2)/(2*covar) + 4*weightDiff*maxDiff);

            for (var i = 0; i < iterations; i++)
            {
                // Switch source and destination buffers.
                if (source == 0)
                {
                    source = 1;
                    destination = 0;
                }
                else
                {
                    source = 0;
                    destination = 1;
                }

                // Vary each pixel individually to find the
                // values that minimise the local potentials.
                for (var y = 0; y < img.Height; y++)
                {
                    for (var x = 0; x < img.Width; x++)
                    {
                        var vLocal = vMax;
                        var minVal = 0;
                        for (var val = 0; val < 255; val++)
                        {
                            // The component of the potential due to the known data.
                            var vData = Math.Pow(val - img.GetPixel(x, y).R, 2)/(2*covar);

                            // The component of the potential due to the
                            // difference between neighbouring pixel values.
                            double vDiff = 0;
                            if (y > 1)
                            {
                                vDiff = vDiff +
                                         Math.Min(Math.Pow(val - buffer[source].GetPixel(x, y - 1).R, 2), maxDiff);
                            }
                            if (y < img.Height - 1)
                            {
                                vDiff = vDiff +
                                         Math.Min(Math.Pow(val - buffer[source].GetPixel(x, y + 1).R, 2), maxDiff);
                            }
                            if (x > 1)
                            {
                                vDiff = vDiff +
                                         Math.Min(Math.Pow(val - buffer[source].GetPixel(x - 1, y).R, 2), maxDiff);
                            }
                            if (x < img.Width - 1)
                            {
                                vDiff = vDiff +
                                         Math.Min(Math.Pow(val - buffer[source].GetPixel(x + 1, y).R, 2), maxDiff);
                            }

                            var vCurrent = vData + weightDiff*vDiff;

                            if (vCurrent < vLocal)
                            {
                                minVal = val;
                                vLocal = vCurrent;
                            }
                        }

                        buffer[destination].SetPixel(x, y, Color.FromArgb(minVal, minVal, minVal));
                    }
                }
            }

            return buffer[destination];
        }

        #endregion
    }
}