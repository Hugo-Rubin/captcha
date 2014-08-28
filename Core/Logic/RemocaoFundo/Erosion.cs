using System.Drawing;
using Core.Logic.Types;

namespace Core.Logic.RemocaoFundo
{
    public class Erosion
    {
        private readonly ImgArray b = new ImgArray(3, 3);
        private readonly ImgArray d;

        public Erosion(ImgArray img)
        {
            var a = PadImgArray(img, new Point(1, 1), 1);
            d = new ImgArray(img.Width, img.Height);
            d.ZeroFill();

            for (var y = 0; y <= a.Height - b.Height; y++)
            {
                for (var x = 0; x <= a.Width - b.Width; x++)
                {
                    var janela = a.GetSegment(new Rectangle(x, y, b.Width, b.Height));
                    if (janela.IsEqual(b))
                    {
                        d.SetPixel(x, y, Color.White);
                    }
                }
            }
        }

        public ImgArray Result
        {
            get { return d; }
        }

        public ImgArray PadImgArray(ImgArray img, Point padSize, int padValue)
        {
            var paddedImg = new ImgArray(img.Width + 2 * padSize.X, img.Height + 2 * padSize.Y);

            for (var y = padSize.Y; y <= img.Height; y++)
            {
                for (var x = padSize.X; x <= img.Width; x++)
                {
                    paddedImg.SetPixel(x, y, img.GetPixel(x - padSize.X, y - padSize.Y));
                }
            }

            for (var x1 = 0; x1 < padSize.Y; x1++)
            {
                var x2 = (paddedImg.Width - 1) - x1;
                for (var y = 0; y < paddedImg.Height; y++)
                {
                    paddedImg.SetPixel(x1, y, Color.FromArgb(padValue, padValue, padValue));
                    paddedImg.SetPixel(x2, y, Color.FromArgb(padValue, padValue, padValue));
                }
            }

            for (var y1 = 0; y1 < padSize.X; y1++)
            {
                var y2 = (paddedImg.Height - 1) - y1;
                for (var x = 0; x < paddedImg.Width; x++)
                {
                    paddedImg.SetPixel(x, y1, Color.FromArgb(padValue, padValue, padValue));
                    paddedImg.SetPixel(x, y2, Color.FromArgb(padValue, padValue, padValue));
                }
            }

            return paddedImg;
        }
    }
}