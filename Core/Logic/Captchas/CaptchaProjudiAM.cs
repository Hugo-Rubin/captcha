using Core.Logic.Captchas.Abstract;
using Core.Logic.ImageQuantizer.Quantizers.XiaolinWu;
using Core.Logic.Separacao;
using Core.Logic.Types;
using Core.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace Core.Logic.Captchas
{

    
//inicio
//- cluster = 0
//- X = 24

//loop para cada cluster
//- movo cursor para X, 0
//- ando para baixo enquanto pixel for branco ou preto - Se encontrar uma cor armazeno no cluster - Se chegar ao fim armazeno preto no cluster
//- X += 25 // Deveria ser 28 mas coloco margem de 3 pixels pq letras podem se sobrepor
//fim loop

//Se nenhum cluster for preto, removo pixel preto e pronto.

//Se algum cluster for preto faco corte cego e removo tudop que nao for preto
    public class CaptchaProjudiAM : Captcha
    {
        private const byte ToleranciaBrilhoLetras = 20;
         
        private Color[] ClusterColors;

        private List<byte>[] brilhosValidosParaLetras;
        private readonly List<byte> brilhosPreto = new List<byte>();
        private readonly List<ImgArray> clustersPorCorDeLetra = new List<ImgArray>();

        public override int NumeroMinimoDeLetras
        {
            get { return 5; }
        }


        public CaptchaProjudiAM(String fileName)
            : base(fileName)
        {
        }

        public CaptchaProjudiAM(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaProjudiAM(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override IEnumerable<Types.ImgArray> GetCaracteres()
        {
            return clustersPorCorDeLetra;
        }
        
        public override Bitmap RemoverFundo(Bitmap source)
        {
            //Se nenhum cluster for preto, removo pixel preto e pronto.
            var containsBlackCluster = ClusterColors.Any(c => c.IsBlackPixel());
            var result = new ImgArray(source.Width, source.Height);
            var currentCluster = 0;
            var currentColor = ClusterColors[currentCluster];
            var previousColor = currentColor;
            var primeiroPixelDoCluster = 0;
            var tamanhoAceitaveldeCluster = 25;

            while (clustersPorCorDeLetra.Count() < ClusterColors.Count())
            {
                clustersPorCorDeLetra.Add(new ImgArray(source.Width, source.Height));
            }
            
            if (true)//containsBlackCluster == false)
            {
                // varro para baixo extraindo pixels com cor do cluster
                for (int x = 0; x < source.Width; x++)
                {
                    for (int y = 0; y < source.Height; y++)
                    {
                        var pixel = source.GetPixel(x, y);
                        
                        //corrigir linha abaixo
                        var isValidColor = ColorBelongsToCluster(pixel, currentCluster);
                        
                        if (isValidColor == false)
                        {
                            if (pixel.IsWhitePixel()
                                || (CoresParecePreto(pixel) && containsBlackCluster == false))
                            {
                                // nothing to do because the pixel is either noise or white
                                continue;
                            }
                            else
                            {
                                // novo cluster encontrado, armazena cor anterior e comece a pintar novo cluster

                                if (x > primeiroPixelDoCluster + tamanhoAceitaveldeCluster
                                    && ColorBelongsToCluster(pixel, Math.Max(currentCluster+1, NumeroMinimoDeLetras-1)))
                                {
                                    currentCluster++;
                                    primeiroPixelDoCluster = x;
                                }
                                else
                                {
                                    //cluster anterior
                                }                                
                            }
                        }
                        
                        // mesmo cluster, apenas continue pintando a imagem
                        result.SetPixel(x, y, Color.Black);
                        clustersPorCorDeLetra[currentCluster].SetPixel(x, y, Color.Black);
                    }
                }
                for (var i = 0; i < clustersPorCorDeLetra.Count; i++)
                {
                    clustersPorCorDeLetra[i] = RemoverRuidos(clustersPorCorDeLetra[i], 5);
                }
                return RemoverRuidos(result, 5).ToBitmap();
            }
            
            //Se algum cluster for preto faco corte cego e removo tudop que nao for preto
                                    
            //while (clustersPorCorDeLetra.Count() < ClusterColors.Count())
            //{
            //    clustersPorCorDeLetra.Add(new ImgArray(source.Width, source.Height));
            //}
            //var wu = new WuColorQuantizer();
            //var pq = new PalleteQuantizer(source, wu, 16);
            //source = (Bitmap)pq.ApplyFilter();

            //var result = new ImgArray(source.Width, source.Height);

            //for (var y = 0; y < source.Height; y++)
            //{
            //    for (var x = 0; x < source.Width; x++)
            //    {
            //        var letraCorIdx = IsLetterColor(source.GetPixel(x, y));
            //        if (letraCorIdx.Chave)
            //        {
            //            result.SetPixel(x, y, Color.Black);
            //            clustersPorCorDeLetra[letraCorIdx.Valor].SetPixel(x, y, Color.Black);
            //        }
            //    }
            //}
            //for (var i = 0; i < clustersPorCorDeLetra.Count; i++)
            //{
            //    clustersPorCorDeLetra[i] = RemoverRuidos(clustersPorCorDeLetra[i], 5);
            //}
            //return RemoverRuidos(result, 5).ToBitmap();
        }

        protected override void ImageLoaded(Bitmap bmpSource)
        {
            base.ImageLoaded(bmpSource);
            ExtrairCoresDeClusters(bmpSource);
            ExtrairBrilhosValidos();
        }

        private void ExtrairBrilhosValidos()
        {
            brilhosValidosParaLetras = new List<byte>[ClusterColors.Length];
            var idx = 0;
            var calculaPreto = true;
            foreach (var cor in ClusterColors)
            {
                brilhosValidosParaLetras[idx] = new List<byte>();

                for (var i = -ToleranciaBrilhoLetras; i < ToleranciaBrilhoLetras; i++)
                {
                    var brilho = cor.BrilhoDoPixel() + i;                    
                    if (brilho > byte.MinValue && brilho < byte.MaxValue)
                    {
                        brilhosValidosParaLetras[idx].Add((byte)(brilho));
                    }

                    if (calculaPreto)
                    {
                        var brilhoPreto = Color.Black.BrilhoDoPixel() + i;
                        if (brilhoPreto > byte.MinValue && brilho < byte.MaxValue)
                        {
                            brilhosPreto.Add((byte)(brilhoPreto));
                        }
                    }
                }

                calculaPreto = false;

                idx++;
            }
        }
        
        private void ExtrairCoresDeClusters(Bitmap source)
        {
            ClusterColors = new Color[NumeroMinimoDeLetras];

            var centroPrimeiroCluster = 24;
            var tamanhoPasso = 25;
            var currentCluster = 0;
            //inicio
            //- cluster = 0
            //- X = 24
            for (int x = centroPrimeiroCluster; x <= (tamanhoPasso * (NumeroMinimoDeLetras-1) + centroPrimeiroCluster); x+=tamanhoPasso)
            {
                // x in 24 49 74 99 124
                
                // Inicializo com pixel preto
                ClusterColors[currentCluster] = Color.Black;

                for (int y = 0; y < source.Height; y++)
                {
                    var pixel = source.GetPixel(x, y);
                    if (pixel.IsBlackPixel() == false
                        && pixel.IsWhitePixel() == false)
                    {
                        // Salvo cor do cluster e saio do loop
                        ClusterColors[currentCluster] = pixel;
                        break;
                    }
                }

                currentCluster++;
            }
        }


        /// <summary>
        ///   Retorna true se for cor de letra e retorna o indice da letra encontrada
        /// </summary>
        /// <param name="color"> </param>
        /// <returns> </returns>
        private bool ColorBelongsToCluster(Color color, int clusterId)
        {
            if (color.IsWhitePixel())
            {
                return false;
            }

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
                    idx = brilhosValidosParaLetras[clusterId].FindIndex(item => item == (byte)i);
                    result = idx >= 0;
                }
                i++;
            }

            return result;
        }

        private bool CoresParecePreto(Color cor1)
        {
            if (cor1.IsBlackPixel())
            {
                return true;
            }

            var result = false;
            var brilho = cor1.BrilhoDoPixel();

            var i = brilho - ToleranciaBrilhoLetras;
            var max = brilho + ToleranciaBrilhoLetras;
            var idx = -1;
            while (!result && i < max)
            {
                if (i > byte.MinValue
                    && i < byte.MaxValue)
                {
                    idx = brilhosPreto.FindIndex(item => item == (byte)i);
                    result = idx >= 0;
                }
                i++;
            }

            return result;

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


    //public class CaptchaRJCopy : Captcha
    //{
    //    private const byte ToleranciaBrilhoLetras = 8;
    //     /// <summary>
    //    ///   Lista com as cores utilizadas apenas nas letras
    //    /// </summary>
    //    private static readonly Color[] CoresValidasParaLetras =
    //    {
    //        /* IMPORTANTE: CAPTURE APENAS UM PIXEL PARA CADA FAMILIA DE COR 
    //           PORQUE ESSA LISTA É USADA PARA CLASSIFICAR CLUSTER POR CORES */
    //        Color.FromArgb(245, 245, 250), //branco
    //        Color.FromArgb(10, 13, 2), //preto
    //        Color.FromArgb(233, 250, 31) //amarelo
    //    };
       

    //    private readonly List<byte> brilhosValidosParaLetras = new List<byte>();
    //    private readonly List<ImgArray> clustersPorCorDeLetra = new List<ImgArray>();

    //    public override int NumeroMinimoDeLetras
    //    {
    //        get { return 5; }
    //    }

    //    public override IEnumerable<ImgArray> GetCaracteres()
    //    {
    //        var idxInicial = new List<int>();
    //        var clusters = new List<ImgArray>();

    //        foreach (var cfsResult in clustersPorCorDeLetra.Select(cluster => new ColorFillingSegmentation2(cluster, 8, NumeroMinimoDePixelsEmCluster, false)).Select(cfs => cfs.GetCaracteres()))
    //        {
    //            clusters.AddRange(cfsResult);
    //            foreach (var img in cfsResult)
    //            {
    //                idxInicial.Add(img.GetMinX());
    //            }
    //        }

    //        var orderedClusters = new ImgArray[clusters.Count()];
    //        var i = 0;
    //        while (clusters.Any())
    //        {
    //            var minIdx = idxInicial.IndexOf(idxInicial.Min());
    //            idxInicial.RemoveAt(minIdx);
    //            if (clusters[minIdx].CountPixelsWithColor(Color.Black) >= NumeroMinimoDePixelsEmCluster)
    //            {
    //                orderedClusters[i++] = clusters[minIdx].CortarECentralizar(60, 60);
    //            }
    //            clusters.RemoveAt(minIdx);
    //        }

    //        return orderedClusters;
    //    }

    //    public override Bitmap RemoverFundo(Bitmap source)
    //    {
    //        while (clustersPorCorDeLetra.Count() < CoresValidasParaLetras.Count())
    //        {
    //            clustersPorCorDeLetra.Add(new ImgArray(source.Width, source.Height));
    //        }
    //        var wu = new WuColorQuantizer();
    //        var pq = new PalleteQuantizer(source, wu, 16);
    //        source = (Bitmap)pq.ApplyFilter();

    //        var result = new ImgArray(source.Width, source.Height);

    //        for (var y = 0; y < source.Height; y++)
    //        {
    //            for (var x = 0; x < source.Width; x++)
    //            {
    //                var letraCorIdx = IsLetterColor(source.GetPixel(x, y));
    //                if (letraCorIdx.Chave)
    //                {
    //                    result.SetPixel(x, y, Color.Black);
    //                    clustersPorCorDeLetra[letraCorIdx.Valor].SetPixel(x, y, Color.Black);
    //                }
    //            }
    //        }
    //        for (var i = 0; i < clustersPorCorDeLetra.Count; i++)
    //        {
    //            clustersPorCorDeLetra[i] = RemoverRuidos(clustersPorCorDeLetra[i], 5);
    //        }
    //        return RemoverRuidos(result, 5).ToBitmap();
    //    }

    //    /// <summary>
    //    ///   Pinta de branco qualquer desenho preto que seja menor ou igual ao tamanho do ruído informado
    //    /// </summary>
    //    /// <param name="imgArray"></param>
    //    /// <param name="tamanhoRuido"> </param>
    //    private ImgArray RemoverRuidos(ImgArray imgArray, int tamanhoRuido)
    //    {
    //        var x = 0;
    //        var y = 0;
    //        var idx = 0;
    //        var pixelsProcessados = new List<Point>();
    //        while (idx < imgArray.Width * imgArray.Height - 1)
    //        {
    //            var pixel = imgArray.GetPixel(x, y);
    //            // Procura proximo pixel preto
    //            while (!pixel.IsBlackPixel()
    //                   && idx < imgArray.Width * imgArray.Height - 1)
    //            {
    //                idx++;
    //                x = idx % imgArray.Width;
    //                y = (int)Math.Floor((decimal)(idx / imgArray.Width));
    //                pixel = imgArray.GetPixel(x, y);
    //            }
    //            var pointPixel = new Point(x, y);
    //            var fim = idx == imgArray.Width * imgArray.Height - 1;
    //            var sair = fim && !imgArray.GetPixel(x, y).IsBlackPixel();

    //            if (!sair && !pixelsProcessados.Contains(pointPixel))
    //            {
    //                var clusterPixels = imgArray.GetCluster(pointPixel);
    //                pixelsProcessados.AddRange(clusterPixels);

    //                if (clusterPixels.Count() < tamanhoRuido)
    //                {
    //                    foreach (var px in clusterPixels)
    //                    {
    //                        imgArray.SetPixel(px.X, px.Y, Color.White);
    //                    }
    //                }
    //            }

    //            idx++;
    //            x = idx % imgArray.Width;
    //            y = (int)Math.Floor((decimal)(idx / imgArray.Width));
    //        }
    //        return imgArray;
    //    }

    //    /// <summary>
    //    ///   Retorna true se for cor de letra e retorna o indice da letra encontrada
    //    /// </summary>
    //    /// <param name="color"> </param>
    //    /// <returns> </returns>
    //    private ChaveValor<bool, int> IsLetterColor(Color color)
    //    {
    //        var result = false;

    //        var brilho = color.BrilhoDoPixel();

    //        var i = brilho - ToleranciaBrilhoLetras;
    //        var max = brilho + ToleranciaBrilhoLetras;
    //        var idx = -1;
    //        while (!result && i < max)
    //        {
    //            if (i > byte.MinValue
    //                && i < byte.MaxValue)
    //            {
    //                idx = brilhosValidosParaLetras.FindIndex(item => item == (byte)i);
    //                result = idx >= 0;
    //            }
    //            i++;
    //        }

    //        return new ChaveValor<bool, int>
    //                   {
    //                       Chave = result,
    //                       Valor = (int)Math.Floor((decimal)(idx / (2 * ToleranciaBrilhoLetras)))
    //                   };
    //    }

    //    protected override void Init()
    //    {
    //        foreach (var cor in CoresValidasParaLetras)
    //        {
    //            for (var i = -ToleranciaBrilhoLetras; i < ToleranciaBrilhoLetras; i++)
    //            {
    //                var brilho = cor.BrilhoDoPixel() + i;
    //                if (brilho > byte.MinValue && brilho < byte.MaxValue)
    //                {
    //                    brilhosValidosParaLetras.Add((byte)(brilho));
    //                }
    //            }
    //        }
    //    }
    //}
}

