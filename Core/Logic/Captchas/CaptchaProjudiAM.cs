using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;
using Core.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PalleteQuantizer.Helpers;
using PalleteQuantizer.Quantizers.XiaolinWu;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;

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
        private const byte BrightnessTolerance = 25;
        private Color backGroundColor = Color.White;
        
        private Color[] clusterSampleColors;

        private Range[] clustersBrightnessRange;
        private readonly Range blackColorBrightnessRange = new Range();

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

        public override IEnumerable<ImgArray> GetCaracteres()
        {
            return clusters.Select(cluster => cluster);
            //.CortarECentralizar(60, 60));
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            var result = new ImgArray(source.Width, source.Height);

            //Se nenhum cluster for preto, removo pixel preto e pronto.
            var containsBlackCluster = clusterSampleColors.Any(c => c.IsBlackPixel());
            var currentCluster = 0;
            var primeiroPixelDoCluster = 10;
            const int tamanhoAceitaveldeCluster = 20;
            
            while (clusters.Count() < clusterSampleColors.Count())
            {
                clusters.Add(new ImgArray(source.Width, source.Height));
            }

            if (true)//containsBlackCluster == false)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    for (var y = 0; y < source.Height; y++)
                    {
                        var pixel = source.GetPixel(x, y);
                        
                        var isValidColor = ColorBelongsToCluster(pixel, currentCluster);

                        if (isValidColor == false)
                        {
                            if (IsBackgroundOrNoise(pixel, containsBlackCluster))
                            {
                                continue;
                            }

                            var isNextCluster = x > primeiroPixelDoCluster + tamanhoAceitaveldeCluster && ColorBelongsToCluster(pixel, Math.Min(currentCluster + 1, NumeroMinimoDeLetras - 1));
                            //var isPreviousCluster = x < primeiroPixelDoCluster + tamanhoAceitaveldeCluster / 2 && ColorBelongsToCluster(pixel, Math.Max(0, currentCluster - 1));

                            if (isNextCluster)
                            {
                                currentCluster++;
                                clusters[currentCluster].SetPixel(x, y, Color.Black);
                                primeiroPixelDoCluster = x + 10;
                            }
                            //else if (isPreviousCluster)
                            //{
                            //    ////cluster anterior
                            //    result.SetPixel(x, y, Color.Black);
                            //    clusters[Math.Max(0, currentCluster - 1)].SetPixel(x, y, Color.Black);
                            //    continue;
                            //}
                        }
                        else
                        {
                            clusters[currentCluster].SetPixel(x, y, Color.Black);
                        }
                        result.SetPixel(x, y, Color.Black);                         
                    }
                }

                return RemoveNoise(result, 5).ToBitmap();
            }

            //Se algum cluster for preto faco corte cego e removo tudo que nao for preto
        }

        private bool IsBackgroundOrNoise(Color pixel, bool containsBlackCluster)
        {
            return pixel.RGBEquals(backGroundColor) == false || pixel.IsWhitePixel()
                   || (containsBlackCluster == false && IsBlackSibling(pixel));
        }

        protected override void ImageLoaded(ref Bitmap bmpSource)
        {
            base.ImageLoaded(ref bmpSource);
            bmpSource = (Bitmap)PalleteQuantizer(bmpSource);
            ExtractClusterColorSamples(bmpSource);
            ExtractClusterBrightnessSamples();
        }

        protected Image PalleteQuantizer(Bitmap source)
        {
            var activeQuantizer = new WuColorQuantizer();

            const int parallelTaskCount = 1;
            const int colorCount = 8;

            return ImageBuffer.QuantizeImage(source, activeQuantizer, null, colorCount, parallelTaskCount);
        }

        class Range
        {
            public int Start { get; set; }
            public int End { get; set; }

            public bool Contains(int value)
            {
                return value >= Start && value <= End;
            }
        }

        private void ExtractClusterBrightnessSamples()
        {
            clustersBrightnessRange = new Range[clusterSampleColors.Length];
            var idx = 0;
            foreach (var brightness in clusterSampleColors.Select(cor => cor.Brightness()))
            {
                clustersBrightnessRange[idx] =
                    new Range
                        {
                            Start = Math.Max(0, brightness - BrightnessTolerance),
                            End = Math.Min(byte.MaxValue, brightness + BrightnessTolerance),
                        };
                idx++;
            }
        }

        private void ExtractClusterColorSamples(Bitmap captcha)
        {
            clusterSampleColors = new Color[NumeroMinimoDeLetras];

            const int centroPrimeiroCluster = 24;
            const int tamanhoPasso = 25;
            var currentCluster = 0;

            backGroundColor = captcha.GetPixel(0, 0);

            for (var x = centroPrimeiroCluster;
                x <= (tamanhoPasso * (NumeroMinimoDeLetras - 1) + centroPrimeiroCluster);
                x += tamanhoPasso)
            {
                clusterSampleColors[currentCluster] = Color.Black;
                var pixelCount = 0;

                for (var y = 0; y < captcha.Height; y++)
                {
                    var pixel = captcha.GetPixel(x, y);
                    if (pixel.RGBEquals(backGroundColor) == false
                        && IsBlackSibling(pixel) == false                        
                        && pixel.IsWhitePixel() == false)
                    {
                        if (pixelCount++ == 3)  // Pega terceiro pixel de cima para baixo para evitar ruido
                        {
                            clusterSampleColors[currentCluster] = pixel;
                            break;
                        }
                    }
                    else
                    {
                        pixelCount = 0;
                    }
                }
                currentCluster++;
            }
        }

        private bool ColorBelongsToCluster(Color color, int clusterId)
        {
            if (color.IsWhitePixel() || color.RGBEquals(backGroundColor))
            {
                return false;
            }
            var brilho = color.Brightness();
            return clustersBrightnessRange[clusterId].Contains(brilho);
        }

        private bool IsBlackSibling(Color color)
        {
            if (color.IsBlackPixel())
            {
                return true;
            }
            var brightness = color.Brightness();
            return blackColorBrightnessRange.Contains(brightness);
        }

        /// <summary>
        ///   Removes any black shape where length <= noiseLength
        /// </summary>
        /// <param name="imgArray"></param>
        /// <param name="noiseLength"> </param>
        private ImgArray RemoveNoise(ImgArray imgArray, int noiseLength)
        {
            var x = 0;
            var y = 0;
            var idx = 0;
            var processedPixels = new List<Point>();
            while (idx < imgArray.Width * imgArray.Height - 1)
            {
                var pixel = imgArray.GetPixel(x, y);
                // Seek for the next black pixel
                while (!pixel.IsBlackPixel()
                       && idx < imgArray.Width * imgArray.Height - 1)
                {
                    idx++;
                    x = idx % imgArray.Width;
                    y = (int)Math.Floor((decimal)(idx / imgArray.Width));
                    pixel = imgArray.GetPixel(x, y);
                }
                var pointPixel = new Point(x, y);
                var end = idx == imgArray.Width * imgArray.Height - 1;
                var shouldBreak = end && !imgArray.GetPixel(x, y).IsBlackPixel();

                if (!shouldBreak && !processedPixels.Contains(pointPixel))
                {
                    var clusterPixels = imgArray.GetCluster(pointPixel);
                    processedPixels.AddRange(clusterPixels);

                    if (clusterPixels.Count() < noiseLength)
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

        protected override void Init()
        {
            base.Init();
            blackColorBrightnessRange.Start = 0;
            blackColorBrightnessRange.End = Math.Min(byte.MaxValue, BrightnessTolerance);
        }
    }
}

