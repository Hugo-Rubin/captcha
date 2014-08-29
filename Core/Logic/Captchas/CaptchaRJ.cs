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

namespace Core.Logic.Captchas
{
    /// <summary>
    ///   Implementação do Captcha do Sintegra de RJ
    /// </summary>
    public class CaptchaRJ : Captcha
    {
        /// <summary>
        ///   Configura margem de tolerância na seleção das letras
        /// </summary>
        private const byte ToleranciaBrilhoLetras = 8;

        /// <summary>
        ///   Lista com as cores utilizadas apenas nas letras
        /// </summary>
        private static readonly Color[] CoresValidasParaLetras =
        {
            /* IMPORTANTE: CAPTURE APENAS UM PIXEL PARA CADA FAMILIA DE COR 
               PORQUE ESSA LISTA É USADA PARA CLASSIFICAR CLUSTER POR CORES */
            Color.FromArgb(245, 245, 250), //branco
            Color.FromArgb(10, 13, 2), //preto
            Color.FromArgb(233, 250, 31) //amarelo
        };

        private readonly List<byte> brilhosValidosParaLetras = new List<byte>();
        private readonly List<ImgArray> clustersPorCorDeLetra = new List<ImgArray>();

        public CaptchaRJ(String fileName)
            : base(fileName)
        {
        }

        public CaptchaRJ(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaRJ(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 5; }
        }

        public override IEnumerable<ImgArray> GetCaracteres()
        {
            var idxInicial = new List<int>();
            var clusters = new List<ImgArray>();

            foreach (var cfsResult in clustersPorCorDeLetra.Select(cluster => new ColorFillingSegmentation2(cluster, 8, NumeroMinimoDePixelsEmCluster, false)).Select(cfs => cfs.GetCaracteres()))
            {
                clusters.AddRange(cfsResult);
                foreach (var img in cfsResult)
                {
                    idxInicial.Add(img.GetMinX());
                }
            }

            var orderedClusters = new ImgArray[clusters.Count()];
            var i = 0;
            while (clusters.Any())
            {
                var minIdx = idxInicial.IndexOf(idxInicial.Min());
                idxInicial.RemoveAt(minIdx);
                if (clusters[minIdx].CountPixelsWithColor(Color.Black) >= NumeroMinimoDePixelsEmCluster)
                {
                    orderedClusters[i++] = clusters[minIdx].CortarECentralizar(60, 60);
                }
                clusters.RemoveAt(minIdx);
            }

            return orderedClusters;
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            while (clustersPorCorDeLetra.Count() < CoresValidasParaLetras.Count())
            {
                clustersPorCorDeLetra.Add(new ImgArray(source.Width, source.Height));
            }
            var wu = new WuColorQuantizer();
            var pq = new PalleteQuantizer(source, wu, 16);
            source = (Bitmap)pq.ApplyFilter();

            var result = new ImgArray(source.Width, source.Height);

            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var letraCorIdx = IsLetterColor(source.GetPixel(x, y));
                    if (letraCorIdx.Chave)
                    {
                        result.SetPixel(x, y, Color.Black);
                        clustersPorCorDeLetra[letraCorIdx.Valor].SetPixel(x, y, Color.Black);
                    }
                }
            }
            for (var i = 0; i < clustersPorCorDeLetra.Count; i++)
            {
                clustersPorCorDeLetra[i] = RemoverRuidos(clustersPorCorDeLetra[i], 5);
            }
            return RemoverRuidos(result, 5).ToBitmap();
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

        /// <summary>
        ///   Retorna true se for cor de letra e retorna o indice da letra encontrada
        /// </summary>
        /// <param name="color"> </param>
        /// <returns> </returns>
        private ChaveValor<bool, int> IsLetterColor(Color color)
        {
            var result = false;
            var brilho = color.BrilhoDoPixel();

            var i = brilho - ToleranciaBrilhoLetras;
            var max = brilho + ToleranciaBrilhoLetras;
            var idx = -1;
            while (!result && i < max)
            {
                if (i > byte.MinValue
                    && i < byte.MaxValue)
                {
                    idx = brilhosValidosParaLetras.FindIndex(item => item == (byte)i);
                    result = idx >= 0;
                }
                i++;
            }

            return new ChaveValor<bool, int>
                       {
                           Chave = result,
                           Valor = (int)Math.Floor((decimal)(idx / (2 * ToleranciaBrilhoLetras)))
                       };
        }

        protected override void Init()
        {
            foreach (var cor in CoresValidasParaLetras)
            {
                for (var i = -ToleranciaBrilhoLetras; i < ToleranciaBrilhoLetras; i++)
                {
                    var brilho = cor.BrilhoDoPixel() + i;
                    if (brilho > byte.MinValue && brilho < byte.MaxValue)
                    {
                        brilhosValidosParaLetras.Add((byte)(brilho));
                    }
                }
            }
        }
    }
}