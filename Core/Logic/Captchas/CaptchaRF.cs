using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Captchas
{
    public class CaptchaRF : Captcha
    {
        public CaptchaRF(String fileName)
            : base(fileName)
        {
        }

        public CaptchaRF(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaRF(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 6; }
        }

        public override IEnumerable<ImgArray> GetCaracteres()
        {
            var separar = new SeparacaoPadrao(this);
            return
                separar.ColorFillingSegmentation2AndSeamCarving2(ImgArray, false).CortarECentralizarTodos(
                    TamanhoImagemLetra.X, TamanhoImagemLetra.Y);
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            var removerFundo = new RemocaoFundoPadrao(source);
            var img = removerFundo.UsandoKmeansEErosao();

            // Usar CropRectangle para apagar os 12 primeiros pixels da imagem para melhorar o desempenho. 
            for (var x = 0; x < img.Width; x++)
            {
                for (var y = 0; y < 13; y++)
                {
                    img.SetPixel(x, y, Color.White);
                }
            }

            img = RemoverRuidos(new ImgArray(img), 70).ToBitmap();

            return img;
        }

        /// <summary>
        ///   Pinta de branco qualquer desenho preto que seja menor ou igual ao tamanho do ruído informado
        /// </summary>
        /// <param name="imgArray"></param>
        /// <param name="tamanhoRuido"> </param>
        private ImgArray RemoverRuidos(ImgArray imgArray, int tamanhoRuido)
        {
            var x = 0;
            var y = 0;
            var idx = 0;
            var pixelsProcessados = new List<Point>();
            while (idx < imgArray.Width * imgArray.Height - 1)
            {
                var pixel = imgArray.GetPixel(x, y);
                // Procura proximo pixel preto
                while (!pixel.IsBlackPixel()
                       && idx < imgArray.Width * imgArray.Height - 1)
                {
                    idx++;
                    x = idx % imgArray.Width;
                    y = (int)Math.Floor((decimal)(idx / imgArray.Width));
                    pixel = imgArray.GetPixel(x, y);
                }
                var pointPixel = new Point(x, y);
                var fim = idx == imgArray.Width * imgArray.Height - 1;
                var sair = fim && !imgArray.GetPixel(x, y).IsBlackPixel();

                if (!sair && !pixelsProcessados.Contains(pointPixel))
                {
                    var clusterPixels = imgArray.GetCluster(pointPixel);
                    pixelsProcessados.AddRange(clusterPixels);

                    if (clusterPixels.Count() < tamanhoRuido)
                    {
                        foreach (var px in clusterPixels)
                        {
                            imgArray.SetPixel(px.X, px.Y, Color.White);
                        }
                    }
                }

                idx++;

                x = idx % imgArray.Width;
                y = (int)Math.Floor((decimal)(idx / imgArray.Width));
            }
            return imgArray;
        }
    }
}