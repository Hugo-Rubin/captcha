using System.Drawing;
using Core.Common;
using Core.Logic.RemocaoFundo;

namespace Core.Logic.Utils
{
    public class RemocaoFundoPadrao
    {
        private Bitmap bmp;

        public RemocaoFundoPadrao(Bitmap src)
        {
            //TODO: Verificar a real necessidade da linha abaixo que estava sendo usada no primeiro captcha da RF
            //this.bmp = src.Clone(new Rectangle(0, 0, source.Width, source.Height), PixelFormat.Format32bppArgb);
            bmp = src;
        }

        public Bitmap LetrasTemBrilhoMenorQue(int brilhoMaxLetra)
        {
            for (var y = 0; y < bmp.Height; y++)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    var novoPixel = Color.White;
                    if (ColorUtils.BrilhoDoPixel(bmp.GetPixel(x, y)) < 90)
                    {
                        novoPixel = Color.Black;
                    }
                    bmp.SetPixel(x, y, novoPixel);
                }
            }
            return bmp;
        }

        public Bitmap UsandoKmeansEErosao()
        {
            var km = new KMeans(2, bmp);
            bmp = km.Apply();

            var img = bmp.CopiarPixelsPretos();

            img.InvertColors();

            var er = new Erosion(img);
            //TODO: Melhorar erosion para nao precisar Inverter cores novamente
            return er.Result.InvertImageColors().ToBitmap();
        }
    }
}