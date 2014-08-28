using System;
using System.Drawing;
using Core.Logic.Types;

namespace Core.Logic.Tratamento
{
    public class ForwardDerivative
    {
        public ImgArray Apply(ImgArray img, bool vertical = true, bool horizontal = true)
        {
            var shifted = new ImgArray(img.Width, img.Height);

            if (horizontal && vertical)
            {
                for (var y = 1; y < img.Height; y++)
                {
                    for (var x = 0; x < img.Width; x++)
                    {
                        var cor = img.GetPixel(x, y - 1);
                        shifted.SetPixel(x, y, cor);
                    }
                }

                for (var y = 0; y < img.Height; y++)
                {
                    for (var x = 0; x < img.Width; x++)
                    {
                        int corImg = img.GetPixel(x, y).R;
                        int corShifted = shifted.GetPixel(x, y).R;
                        var cor = Math.Abs(corImg - corShifted);
                        if (cor == 255)
                        {
                            img.SetPixel(x, y, Color.FromArgb(cor, cor, cor));
                        }
                    }
                }

                shifted.Clear();

                for (var y = 0; y < img.Height; y++)
                {
                    for (var x = 1; x < img.Width; x++)
                    {
                        var cor = img.GetPixel(x - 1, y);
                        shifted.SetPixel(x, y, cor);
                    }
                }
            }
            else
            {
                if (horizontal)
                {
                    for (var y = 0; y < img.Height; y++)
                    {
                        for (var x = 1; x < img.Width; x++)
                        {
                            var cor = img.GetPixel(x - 1, y);
                            shifted.SetPixel(x, y, cor);
                        }
                    }
                }
                else
                {
                    if (vertical)
                    {
                        for (var y = 1; y < img.Height; y++)
                        {
                            for (var x = 0; x < img.Width; x++)
                            {
                                var cor = img.GetPixel(x, y - 1);
                                shifted.SetPixel(x, y, cor);
                            }
                        }
                    }
                    else
                    {
                        return img;
                    }
                }
            }

            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    int corImg = img.GetPixel(x, y).R;
                    int corShifted = shifted.GetPixel(x, y).R;
                    var cor = Math.Abs(corImg - corShifted);
                    if (cor == 255)
                    {
                        img.SetPixel(x, y, Color.FromArgb(cor, cor, cor));
                    }
                }
            }

            return img;
        }
    }
}