using System.Drawing;
using Core.Logic.Types;

namespace Core.Logic.Tratamento
{
    // ReSharper disable once InconsistentNaming
    public class ICMBasic
    {
        public ImgArray Apply(ImgArray img, double alpha, double beta, int iterations, double covar = 0)
        {
            var imgY = new ImgArray[2];
            imgY[0] = img.Clone();
            imgY[1] = new ImgArray(img.Width, img.Height);

            for (var k = 0; k < iterations; k++)
            {
                for (var y = 0; y < img.Height; y++)
                {
                    for (var x = 0; x < img.Width; x++)
                    {
                        var n = new Neighbors(imgY[0], new Point(x, y), false);
                        var costBlack = alpha * (1 - Match(0, img.GetPixel(x, y).R)) + beta * NeighborhoodCost(n, 0);
                        var costWhite = alpha * (1 - Match(255, img.GetPixel(x, y).R)) + beta * NeighborhoodCost(n, 1);

                        var newColor = costBlack < costWhite ? Color.Black : Color.White;
                        imgY[1].SetPixel(x, y, newColor);
                    }
                }

                if (imgY[0].IsEqual(imgY[1]))
                {
                    break;
                }

                imgY[0] = imgY[1].Clone();
                imgY[1].Clear();
            }

            return imgY[0];
        }

        private int NeighborhoodCost(Neighbors n, int p1)
        {
            var neighborhood = n.ToStack();
            var sum = 0;

            foreach (var neighbor in neighborhood)
            {
                sum += 1 - Match(p1, neighbor);
            }

            return sum;
        }

        private int Match(int cor1, int cor2)
        {
            return cor1 == cor2 ? 1 : 0;
        }
    }
}