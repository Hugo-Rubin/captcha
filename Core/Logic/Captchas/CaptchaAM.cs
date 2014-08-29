using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common;
using Core.Common.Extensions;
using Core.Logic.Captchas.Abstract;
using Core.Logic.ImageQuantizer.Quantizers.XiaolinWu;
using Core.Logic.Separacao;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Captchas
{
    /// <summary>
    ///   Implementação do Captcha do Sintegra de AM
    /// </summary>
    public class CaptchaAM : Captcha
    {
        public CaptchaAM(String fileName)
            : base(fileName)
        {
        }

        public CaptchaAM(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaAM(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 7; }
        }

        public override Point TamanhoImagemLetra
        {
            get { return new Point(30, 30); }
        }

        public override IEnumerable<ImgArray> GetCaracteres()
        {
            var cfs = new ColorFillingSegmentation2(ImgArray, 8, 1);
            var chars = cfs.GetCaracteres();

            // TODO: Mesclar clusters do CFS2 não está funcionando nesse CAPTCHA por algum motivo, por isso precisei implementar algo semelhante abaixo.
            for (var i = 1; i < chars.Count; i++)
            {
                if (chars[i].CountPixelsWithColor(Color.Black) <= 2)
                {
                    // Mesclar com a imagem anterior e excluir essa imagem
                    var pingo = chars[i];

                    for (var y = 0; y < pingo.Height; y++)
                    {
                        for (var x = 0; x < pingo.Width; x++)
                        {
                            if (pingo.GetPixel(x, y).IsBlackPixel())
                            {
                                chars[i - 1].SetPixel(x, y, Color.Black);
                            }
                        }
                    }

                    chars.RemoveAt(i);
                }
            }

            while (chars.Count > NumeroMinimoDeLetras)
            {
                var menorCluster = 0;
                for (var i = 1; i < chars.Count; i++)
                {
                    if (chars[i].CountPixelsWithColor(Color.Black) <
                        chars[menorCluster].CountPixelsWithColor(Color.Black))
                    {
                        menorCluster = i;
                    }
                }

                chars.RemoveAt(menorCluster);
            }

            while (chars.Count < NumeroMinimoDeLetras)
            {
                var maxWidth = chars.Max(i => i.Width);
                var maxIdx = chars.IndexOf(chars.FirstOrDefault(i => i.Width == chars.Max(m => m.Width)));

                var seamCarving = new SeamCarving2(2, maxWidth / 2);
                var clusters = seamCarving.GetClusters(new ImgArray(chars[maxIdx]));

                // Substitui Imagem Colada pelos clusters encontrados
                chars.RemoveAt(maxIdx);
                chars.InsertRange(maxIdx, clusters);
            }

            return chars.CortarECentralizarTodos(TamanhoImagemLetra.X, TamanhoImagemLetra.Y);
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            var img = new ImgArray(source.Width, source.Height);

            var quadroLetras = new Rectangle(3, 5, 115, 27);
            source = source.CropRectangle(quadroLetras);

            var wu = new WuColorQuantizer();
            var pq = new PalleteQuantizer(source, wu, 4);
            var bmp8 = (Bitmap)pq.ApplyFilter();

            for (var y = 0; y < bmp8.Height; y++)
            {
                for (var x = 0; x < bmp8.Width; x++)
                {
                    if (source.GetPixel(x, y).BrilhoDoPixel() < 160)
                    {
                        img.SetPixel(x, y, Color.Black);
                    }
                }
            }

            img = RemoverRuidos(img, 6);

            //img.SaveWithAleatory(@"C:\");
            return img.ToBitmap();
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

            while (idx < imgArray.Length - 1)
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
                var fim = idx == imgArray.Length - 1;
                var sair = fim && !imgArray.GetPixel(x, y).IsBlackPixel();

                if (!sair && !pixelsProcessados.Contains(pointPixel))
                {
                    var clusterPixels = imgArray.GetCluster(pointPixel);
                    pixelsProcessados.AddRange(clusterPixels);

                    if (clusterPixels.Count() < tamanhoRuido && !VerificarPingoDoI(imgArray, clusterPixels))
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

        /// <summary>
        ///   Verifica se o ruído é na verdade o pingo de um 'i' ou 'j'
        /// </summary>
        /// <param name="img"> Imagem com fundo removido </param>
        /// <param name="pixels"> Conjunto de pixels que formam o possível ruído </param>
        /// <returns> Verdadeiro se o ruído for um pingo e falso caso contrário. </returns>
        private bool VerificarPingoDoI(ImgArray img, IEnumerable<Point> pixels)
        {
            foreach (var pix in pixels)
            {
                int x = pix.X, y = pix.Y;
                if ((y == 10 || (y == 9 && img.GetPixel(x, 10).IsBlackPixel())) && img.GetPixel(x, 13).IsBlackPixel())
                {
                    return true;
                }
            }

            return false;
        }
    }
}