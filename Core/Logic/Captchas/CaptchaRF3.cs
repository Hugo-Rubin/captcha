using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Core.Common.Extensions;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Captchas
{
    public class CaptchaRF3 : Captcha
    {
        public CaptchaRF3(string fileName) : base(fileName)
        {
        }

        public CaptchaRF3(Bitmap bmpSource) : base(bmpSource)
        {
        }

        public CaptchaRF3(NanoArray nanoArraySource) : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 6; }
        }

        public override IEnumerable<ImgArray> GetCaracteres()
        {
            int[] cuttingPointsX = { 40, 70, 100, 130, 160 };
            var qtdLetras = cuttingPointsX.Length + 1;
            var chars = new ImgArray[qtdLetras];
            var clusterAtual = new ImgArray(ImgArray.Width, ImgArray.Height);

            chars.Populate(new ImgArray(45, 30));

            for (var idx = 0; idx < qtdLetras; idx++)
            {
                for (var y = 0; y < ImgArray.Height; y++)
                {
                    var posInicialX = idx == 0 ? 0 : cuttingPointsX[idx - 1]; // caso seja o primeiro cluster, parte da posição 0
                    var posFinalX = idx != qtdLetras - 1 ? cuttingPointsX[idx] : ImgArray.Width; // caso seja o último cluster, vai até o final da imagem

                    for (var x = posInicialX; x < posFinalX; x++)
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
            var result = new ImgArray(source.Width, source.Height);

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
    }
}
