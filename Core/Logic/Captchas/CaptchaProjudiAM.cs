using Core.Logic.Captchas.Abstract;
using Core.Logic.ImageQuantizer.Quantizers.XiaolinWu;
using Core.Logic.Separacao;
using Core.Logic.Types;
using Core.Logic.Utils;
using Core.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AForge.Imaging.Filters;


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
        private const byte ToleranciaBrilhoLetras = 11;
         
        private Color[] ClusterSampleColors;

        private List<byte>[] brilhosValidosParaLetras;
        private readonly List<byte> brilhosPreto = new List<byte>();
        private readonly List<ImgArray> clusters = new List<ImgArray>();

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
            return clusters.Select(cluster=> cluster.CortarECentralizar(60, 60));
        }
        
        public override Bitmap RemoverFundo(Bitmap source)
        {
            var result = new ImgArray(source.Width, source.Height);

            //Se nenhum cluster for preto, removo pixel preto e pronto.
            var containsBlackCluster = ClusterSampleColors.Any(c => c.IsBlackPixel());
            var currentCluster = 0;
            var currentColor = ClusterSampleColors[currentCluster];
            var previousColor = currentColor;
            var primeiroPixelDoCluster = 10;
            var tamanhoAceitaveldeCluster = 20;

            while (clusters.Count() < ClusterSampleColors.Count())
            {
                clusters.Add(new ImgArray(source.Width, source.Height));
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
                        var isValidColor = CorPertenceAoCluster(pixel, currentCluster);
                        
                        if (isValidColor == false)
                        {
                            if (pixel.IsWhitePixel()
                                || (containsBlackCluster == false && CorParecePreto(pixel)))
                            {
                                continue;
                            }
                            else
                            {
                                // novo cluster encontrado, armazena cor anterior e comece a pintar novo cluster

                                if (x > primeiroPixelDoCluster + tamanhoAceitaveldeCluster
                                    && CorPertenceAoCluster(pixel, Math.Max(currentCluster+1, NumeroMinimoDeLetras-1)))
                                {
                                    currentCluster++;
                                    primeiroPixelDoCluster = x+10;
                                }
                                else
                                {
                                    //cluster anterior
                                    result.SetPixel(x, y, Color.Black);
                                    clusters[Math.Max(0,currentCluster-1)].SetPixel(x, y, Color.Black);
                                    continue;
                                }                                
                            }
                        }
                        
                        // mesmo cluster, apenas continue pintando a imagem
                        result.SetPixel(x, y, Color.Black);
                        clusters[currentCluster].SetPixel(x, y, Color.Black);
                    }
                }
                
                return RemoverRuidos(result, 5).ToBitmap();
            }
            
            //Se algum cluster for preto faco corte cego e removo tudop que nao for preto
        }

        protected override void ImageLoaded(Bitmap bmpSource)
        {
            base.ImageLoaded(bmpSource);
            ExtrairCoresDeClusters(bmpSource);
            ExtrairBrilhosValidos();
        }

        private void ExtrairBrilhosValidos()
        {
            brilhosValidosParaLetras = new List<byte>[ClusterSampleColors.Length];
            var idx = 0;
            var calculaPreto = true;
            foreach (var cor in ClusterSampleColors)
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
            ClusterSampleColors = new Color[NumeroMinimoDeLetras];

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
                ClusterSampleColors[currentCluster] = Color.Black;

                for (int y = 0; y < source.Height; y++)
                {
                    var pixel = source.GetPixel(x, y);
                    if (pixel.IsBlackPixel() == false
                        && pixel.IsWhitePixel() == false)
                    {
                        // Salvo cor do cluster e saio do loop
                        ClusterSampleColors[currentCluster] = pixel;
                        break;
                    }
                }

                currentCluster++;
            }
        }


        private bool CorPertenceAoCluster(Color color, int clusterId)
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

        private bool CorParecePreto(Color cor)
        {
            if (cor.IsBlackPixel())
            {
                return true;
            }

            var result = false;
            var brilho = cor.BrilhoDoPixel();

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
}

