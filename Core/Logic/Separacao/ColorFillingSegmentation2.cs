using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Logic.Types;

namespace Core.Logic.Separacao
{
    public class ColorFillingSegmentation2
    {
        private const int UnlabeledFlag = 255;
        private readonly List<ImgArray> carac = new List<ImgArray>();
        private readonly bool mesclarClusterAbaixo = true;
        private readonly bool mesclarClusterAcima;
        private readonly int tamanhoMinimoLetra;
        private int[,] labels;
        private int nSegments;

        /// <summary>
        ///   Separa clusters por conexão de pixels. 
        ///   Retorna clusters no tamanho da imagem original para respeitar ordem
        ///   no aso do CaptchaRJ por exemplo que usa segmentação por cores
        /// </summary>
        /// <param name="imgBlackAndWhite"> Recebe imagem com fundo BRANCO e letra preta </param>
        /// <param name="connectivity"> </param>
        /// <param name="minClusterLength"></param>
        /// <param name="mesclarClusterAbaixo"></param>
        /// <param name="mesclarClusterAcima"></param>
        /// <param name="tamanhoMinimoLetra"></param>
        public ColorFillingSegmentation2(ImgArray imgBlackAndWhite, int connectivity = 8, int minClusterLength = 50,
                                         bool mesclarClusterAbaixo = true, bool mesclarClusterAcima = false,
                                         int tamanhoMinimoLetra = 50)
        {
            this.mesclarClusterAbaixo = mesclarClusterAbaixo;
            this.mesclarClusterAcima = mesclarClusterAcima;
            this.tamanhoMinimoLetra = tamanhoMinimoLetra;
            ClassificarClusters(imgBlackAndWhite, connectivity);

            for (var i = 1; i < nSegments; i++)
            {
                var segment = GetSegmentById(i);
                if (segment.CountPixelsWithColor(Color.Black) >= minClusterLength)
                {
                    carac.Add(segment);
                }
            }
        }

        public List<ImgArray> GetCaracteres()
        {
            return carac;
        }

        private void ClassificarClusters(ImgArray imgArray, int connectivity = 8)
        {
            var initLabels = GetInitLabels(imgArray);

            var colorId = 1;

            while (true)
            {
                if (colorId < UnlabeledFlag)
                {
                    var sairDoLooping = false;
                    var x = 0;

                    var seed = new Point(-1, -1);

                    // Tem que varrer nessa ordem para respeitar a ordem das letras
                    while (x < initLabels.GetLength(0) && !sairDoLooping)
                    {
                        var y = 0;
                        while (y < initLabels.GetLength(1) && !sairDoLooping)
                        {
                            if (initLabels[x, y] == UnlabeledFlag)
                            {
                                seed = new Point
                                           {
                                               X = x,
                                               Y = y
                                           };
                                sairDoLooping = true;
                            }
                            y++;
                        }
                        x++;
                    }

                    if (seed.X > -1)
                    {
                        initLabels = FloodFill(initLabels, seed, colorId, connectivity);
                        colorId += 1;
                    }
                    else
                    {
                        // Não tem mais cluster sem classificação
                        break;
                    }
                }
            }
            labels = initLabels;
            nSegments = colorId;
        }

        private ImgArray ImgArrayFromSubMatrix(int[,] img, int id)
        {
            var result = new ImgArray(img.GetLength(0), img.GetLength(1));
            for (var y = 0; y < img.GetLength(1); y++)
            {
                for (var x = 0; x < img.GetLength(0); x++)
                {
                    if (img[x, y] == id)
                    {
                        result.SetPixel(x, y, Color.Black);
                    }
                }
            }
            return result;
        }

        private int[,] GetInitLabels(ImgArray imgFundoBranco)
        {
            var result = new int[imgFundoBranco.Width, imgFundoBranco.Height];
            for (var y = 0; y < imgFundoBranco.Height; y++)
            {
                for (var x = 0; x < imgFundoBranco.Width; x++)
                {
                    if (imgFundoBranco.GetByte(x, y) == 1)
                    {
                        result[x, y] = 0;
                    }
                    else
                    {
                        result[x, y] = UnlabeledFlag;
                    }
                }
            }
            return result;
        }

        private int[,] FloodFill(int[,] img, Point seed, int color, int connectivity)
        {
            var result = img.FloodFill(seed, color, connectivity);

            if (mesclarClusterAbaixo)
            {
                #region Pegar os limites inferiores do cluster encontrado

                var minX = result.GetLength(0);
                var maxX = 0;
                var maxY = 0;
                var numberOfPixels = 0;

                for (var x = 0; x < result.GetLength(0); x++)
                {
                    for (var y = 0; y < result.GetLength(1); y++)
                    {
                        if (result[x, y] == color)
                        {
                            minX = Math.Min(x, minX);
                            maxX = Math.Max(x, maxX);
                            maxY = Math.Max(y, maxY);
                            numberOfPixels++;
                        }
                    }
                }

                #endregion

                #region Procura e mescla cluster abaixo do cluster encontrado

                if (numberOfPixels < tamanhoMinimoLetra)
                {
                    var x2 = minX;
                    var y2 = maxY + 1;
                    var sairDoLooping = false;

                    var clusterEncontradoAbaixo = 0;

                    while ((x2 < maxX) && !sairDoLooping)
                    {
                        while ((y2 < result.GetLength(1)) && !sairDoLooping)
                        {
                            // Nao pode comparar com UNLABELED_FLAG porque a letra abaixo pode ja
                            // ter sido classificada
                            if (result[x2, y2] != 0)
                            {
                                clusterEncontradoAbaixo = result[x2, y2];
                                sairDoLooping = true;
                            }
                            y2++;
                        }
                        x2++;
                    }
                    if (clusterEncontradoAbaixo > 0)
                    {
                        result = clusterEncontradoAbaixo != 255 ? img.FloodFill(seed, clusterEncontradoAbaixo, connectivity) : img.FloodFill(new Point(x2, y2), color, connectivity);
                    }
                }

                #endregion
            }


            if (mesclarClusterAcima)
            {
                // tem que ver como vai ser o comportamento dessa parte agora
                // pra imagem que estamos testando ela nao sera executada...
                // tem que rodar tudo pra ver se vai dar pau em casos onde a parte de baixo esta a esquerda da parte de cima (vide paint)

                #region Pegar os limites superiores do cluster encontrado

                var minX = result.GetLength(0);
                var maxX = 0;
                var minY = int.MaxValue;
                var numberOfPixels = 0;
                var clusterEncontradoAcima = 0;
                for (var x = 0; x < result.GetLength(0); x++)
                {
                    for (var y = result.GetLength(1) - 1; y >= 0; y--)
                    {
                        if (result[x, y] == color)
                        {
                            minX = Math.Min(x, minX);
                            maxX = Math.Max(x, maxX);
                            minY = Math.Min(y, minY);
                            numberOfPixels++;
                        }
                    }
                }

                #endregion

                #region Procura e mescla cluster acima do cluster encontrado

                if (numberOfPixels < tamanhoMinimoLetra)
                {
                    var x2 = minX;
                    var y2 = 0;
                    var sairDoLooping = false;

                    while ((x2 < maxX) && !sairDoLooping)
                    {
                        while ((y2 < minY) && !sairDoLooping)
                        {
                            // Nao pode comparar com UNLABELED_FLAG porque a letra abaixo pode ja
                            // ter sido classificada
                            if (result[x2, y2] != 0)
                            {
                                clusterEncontradoAcima = result[x2, y2];
                                sairDoLooping = true;
                            }
                            y2++;
                        }
                        x2++;
                    }
                    if (clusterEncontradoAcima > 0)
                    {
                        result = clusterEncontradoAcima != 255 ? img.FloodFill(seed, clusterEncontradoAcima, connectivity) : img.FloodFill(new Point(x2, y2), color, connectivity);
                    }
                }

                #endregion
            }

            return result;
        }

        private ImgArray GetSegmentById(int segmentId)
        {
            if (!(segmentId >= 0
                  && segmentId <= nSegments))
            {
                throw new ArgumentException(
                    String.Format(
                        "ID do segmento precisa ser positivo e menor que o numero total de segmentos encontrados na imagem ({0}). ParamValue = {1}",
                        nSegments, segmentId), paramName: "segmentId");
            }
            return ImgArrayFromSubMatrix(labels, segmentId);
        }
    }
}