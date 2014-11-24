using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;
using Core.Logic.Utils;
using Core.Logic.Filtros;
using System.Drawing.Imaging;
using Core.Common.Extensions;

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
            int[] cuttingPointsX = { 40, 70, 100, 130, 160 };
            int qtdLetras = cuttingPointsX.Length + 1;
            ImgArray[] chars = new ImgArray[qtdLetras];
            ImgArray clusterAtual = new ImgArray(this.ImgArray.Width, this.ImgArray.Height);

            chars.Populate<ImgArray>(new ImgArray(45, 30));
                        
            for (int idx = 0; idx < qtdLetras; idx++)
            {
                for (int y = 0; y < this.ImgArray.Height; y++)
                {
                    int posInicialX = idx != 0 ? cuttingPointsX[idx - 1] : 0; // caso seja o primeiro cluster, parte da posição 0
                    int posFinalX = idx != qtdLetras - 1 ? cuttingPointsX[idx] : this.ImgArray.Width; // caso seja o último cluster, vai até o final da imagem
                    
                    for (int x = posInicialX; x < posFinalX; x++)
                    {
                        clusterAtual.SetPixel(x - posInicialX, y, ImgArray.GetPixel(x, y));
                    }
                }

                chars[idx] = clusterAtual.CortarECentralizar(45, 30);
                clusterAtual.Clear();
            }

            return chars;
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            ImgArray result = new ImgArray(source.Width, source.Height);

            // A imagem volta do servidor com o fundo preto e as letras em tons de roxo. O código abaixo deixa o fundo branco e todo o resto preto.
            unsafe
            {
                var bmd = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly,
                                              source.PixelFormat);
                var pixelSize = BitmapExtension.GetPixelSize(source.PixelFormat);

                for (var y = 0; y < bmd.Height; y++)
                {
                    var row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                    for (var x = 0; x < bmd.Width; x++)
                    {
                        var idx = x * bmd.Height + y;
                        var pixel = row[(int)(x * pixelSize)];
                        var color = Convert.ToByte(pixel == 0); // Aqui a cor do pixel é alterada
                        result[idx] = color;
                    }
                }

                source.UnlockBits(bmd);
            }

            return result.ToBitmap();
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