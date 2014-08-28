using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic
{
    public class TemplateMatching
    {
        private static readonly StringBuilder Sb = new StringBuilder();
        private static int count;
        private readonly ImgArray image;
        private readonly int limiteMenorJanela;
        private readonly int lowerBound;
        private readonly int[] minErrors;
        private readonly List<int> penalties;
        private readonly ImgArray smallestTemplate;
        private readonly List<ChaveValor<char, ImgArray>> templates;
        private readonly int upperBound;

        public TemplateMatching(ImgArray image, string caminhoTemplates)
        {
            this.image = image;
            templates = CarregarTemplates(caminhoTemplates);
            smallestTemplate = MinTemplate();
            var biggestTemplate = MaxTemplate();
            var limiteJan = image.Width - smallestTemplate.Width + 1;
            limiteMenorJanela = limiteJan <= 0 ? 1 : limiteJan;

            minErrors = new int[limiteMenorJanela / smallestTemplate.Width + 1];
            // modificado 23/05, antes era: this.limiteMenorJanela
            minErrors.Init(Int32.MaxValue);


            lowerBound = smallestTemplate.Length;
            upperBound = biggestTemplate.Length;

            penalties = new List<int>();
            var imgTemplates = from t in templates
                               select t.Valor;
            foreach (var img in imgTemplates)
            {
                penalties.Add(Normalize(img.Length));
            }
        }

        /*
         * 2 arrays com tamanho do numero total de posiçoes iniciais
         * um parar gravar o erro minimo até então encontrado
         * e outro para gravar o template referente ao erro minimo
         * 
         * int start_position_minimal_error[total_de_start_positions]
         * dar continue caso erro_atual > menor_erro
         * int start_position_template[total_de_start_positions]
         * 
         */


        /// <summary>
        ///   Retorna o menor template em largura
        /// </summary>
        /// <returns> </returns>
        private ImgArray MinTemplate()
        {
            var minTemplate = templates[0].Valor;
            var imgTemplates = from t in templates
                               select t.Valor;

            foreach (var img in imgTemplates.Where(img => img.Width < minTemplate.Width))
            {
                return new ImgArray(img);
            }

            return minTemplate;
        }

        private ImgArray MaxTemplate()
        {
            var maxTemplate = templates[0].Valor;
            var imgTemplates = from t in templates
                               select t.Valor;

            foreach (var img in imgTemplates.Where(img => img.Width > maxTemplate.Width))
            {
                return new ImgArray(img);
            }

            return maxTemplate;
        }

        private int Normalize(int value)
        {
            var r = (int)Math.Round(1.0 + (value - lowerBound) * (3.0 - 1.0) / (upperBound - lowerBound));
            switch (r)
            {
                case 1:
                    return 3;
                case 2:
                    return 2;
                case 3:
                    return 1;
                default:
                    return 1;
            }
        }

        public char[] Match(int numberOfClusters, int clusterSize, int clusterPosition, int otherClusterSize = 0)
        {
            var numCarac = 0;
            var possiblyMultiple = false;

            switch (numberOfClusters)
            {
                case 1:
                    {
                        possiblyMultiple = true;
                        numCarac = 4;
                        // 4 ou 5 caracteres no cluster, fazer uma busca incrementando de 1 em 1 em todo o cluster
                        break;
                    }
                case 2:
                    {
                        const int threshold = 10;
                        possiblyMultiple = true;
                        if (Math.Abs(clusterSize - otherClusterSize) < threshold)
                        {
                            numCarac = 2;
                        }
                        else
                        {
                            numCarac = clusterSize < otherClusterSize ? 2 : 3;
                        }

                        // Mais de um caractere em cada cluster. Opções: 2-2, 2-3 e 3-2.
                        break;
                    }
                case 3:
                    {
                        if (clusterPosition == 0)
                        {
                            numCarac = 1;
                        }
                        else
                        {
                            possiblyMultiple = true;

                            numCarac = clusterPosition == 1 ? 2 : 3;
                        }
                        // Pelo menos um cluster tem 2 caracteres, talvez 3.
                        // Chamar Match recebendo 1 caractere do 1º cluster e checando os 2 últimos para o caso de serem maiores que nosso maior template
                        // Caso os 2 seja, retornar 2 caracteres dos 2 últimos clusters, senão retornar 2 apenas do último
                        break;
                    }
                case 4:
                    {
                        if (clusterPosition == 3)
                        {
                            // arrumar aqui, procurar por 2 clusters quando for assim, depois q achou o 1o na posição 0, avanço a janela em templatePrimeiraPosicao.Width unidades e se o restante da janela for maior ou igual ao nosso menor template, faço template matching na janela seguinte
                            possiblyMultiple = true;
                            numCarac = 2;
                        }
                        else
                        {
                            numCarac = 1;
                        }

                        break;
                    }
                case 5:
                    {
                        numCarac = 1;
                        break;
                    }
            }

            var errosTemplate = new List<ChaveValor<int, ChaveValor<int, char>>>();
            var nextIndex = smallestTemplate.Width;
            var position = 0;
            var brk = false;
            var multiple = false;
            var hold = false;
            var holding = false;
            var windowPositionCounter = 0;

            #region Em Testes - 1

            var tempMinErrors = 0;
            var tempErrosTemplate = new ChaveValor<int, ChaveValor<int, char>>();
            var tempNextIndex = smallestTemplate.Width;

            #endregion

            // int[] idxMelhoresTemplates = new int[this.limiteMenorJanela];
            // ImgArray[] templatesEncontrados = new ImgArray[idxMelhoresTemplates.Length];

            // Para cada posição da imagem original até onde caiba um template
            for (var inicioJanela = 0;
                 inicioJanela < (numCarac > 1 ? limiteMenorJanela : 1);
                 inicioJanela += nextIndex, position++)
            {
                if (inicioJanela != 0)
                {
                    multiple = true;
                    windowPositionCounter++;
                }

                // Para cada template
                for (var idxTemplate = 0; idxTemplate < templates.Count; idxTemplate++)
                {
                    var templateWidth = templates.ElementAt(idxTemplate).Valor.Width;

                    if ((windowPositionCounter + 1 == numCarac && templateWidth > (image.Width - inicioJanela) + 5)
                        || (templateWidth > (image.Width - inicioJanela) + 5))
                    {
                        continue;
                    }

                    var templateHeight = templates.ElementAt(idxTemplate).Valor.Height;

                    var r = new Rectangle(inicioJanela, 0, templateWidth, image.Height);
                    //TODO: VERIFICAR o porque da linha abaixo usar crop só de vez em qdo
                    //ImgArray slidingWindow = new ImgArray(image.ToBitmap().CropRectangle(r).RemoveWhiteBorders(multiple ? false : true));
                    //var slidingWindow = new ImgArray(image.ToBitmap().CropRectangle(r).RemoveWhiteBorders());
                    var slidingWindow = new ImgArray(image.ToBitmap().CropRectangle(r)).RemoveWhiteBorders();

                    var errorCount = 0;

                    if (slidingWindow.Height < 12)
                    {
                        continue;
                    }

                    var jumpIndexesTemplate = new List<int>();
                    var jumpIndexesImage = new List<int>();
                    var heightDifference = templateHeight - slidingWindow.Height;
                    var widthDifference = templateWidth - slidingWindow.Width;
                    var widthDiscount = 0;
                    var heightDiscount = 0;
                    byte error = 1;

                    var penalizeWiderTemplate = 0;

                    var ocrTipo2Penalization = OcrTipo2PenalizationAndPixelsToSkip(widthDifference,
                                                                                      templateHeight, templateWidth,
                                                                                      inicioJanela, numCarac, error,
                                                                                      errorCount, idxTemplate,
                                                                                      windowPositionCounter, hold,
                                                                                      possiblyMultiple, multiple);
                    error = Convert.ToByte(ocrTipo2Penalization[0]);
                    errorCount = ocrTipo2Penalization[1];
                    hold = Convert.ToBoolean(ocrTipo2Penalization[2]);

                    if (widthDifference > 0)
                    {
                        widthDiscount = widthDifference * templateHeight;
                    }

                    // Se template for mais alto que imagem
                    if (heightDifference > 0)
                    {
                        for (var i = templateHeight - heightDifference;
                             i < templateHeight * templateWidth;
                             i += templateHeight)
                        {
                            for (var j = 0; j < heightDifference; j++)
                            {
                                jumpIndexesTemplate.Add(i + j);
                            }
                        }

                        heightDiscount = jumpIndexesTemplate.Count;
                    }
                    else
                    {
                        // Se imagem for mais alta que template
                        if (heightDifference < 0)
                        {
                            for (var i = image.Height + heightDifference;
                                 i < image.Height * image.Width;
                                 i += image.Height)
                            {
                                for (var j = 0; j < -heightDifference; j++)
                                {
                                    jumpIndexesImage.Add(i + j);
                                }
                            }
                        }
                    }

                    var limit = templates.ElementAt(idxTemplate).Valor.Length - heightDiscount - widthDiscount;

                    // Para cada posição do template, avançando juntamente com a imagem
                    for (int k = 0, m = 0; k <= limit || m <= limit; k++, m++)
                    {
                        // Verifico se o template é mais alto que a imagem, se sim, ajusto m para ser usado como indexador do template
                        if (heightDifference > 0)
                        {
                            while (jumpIndexesTemplate.Contains(m))
                            {
                                m++;
                            }
                        }
                        else
                        {
                            // Verifico se o template é mais baixo que a imagem, se sim, ajusto k para ser usado como indexador da imagem
                            // e somo 1 ao erro caso a cor do pixel na imagem seja preto
                            if (heightDifference < 0)
                            {
                                while (jumpIndexesImage.Contains(k))
                                {
                                    if (slidingWindow[k] == 0)
                                    {
                                        errorCount += error; //++;
                                    }

                                    k++;
                                }
                            }
                        }

                        // Se a coluna atual do template for menor que a coluna máxima da imagem, verifico a diferença do pixel, senão somo 1 ao erro
                        if (m / templateHeight < slidingWindow.Width)
                        {
                            // Se o pixel na imagem é diferente do pixel no template, contabilizo mais um erro
                            if (slidingWindow[k] != templates.ElementAt(idxTemplate).Valor[m])
                            {
                                errorCount += error; //++;
                            }
                        }
                        /*else
                        {
                            if (k != limit && m != limit && templates.ElementAt(idxTemplate).Valor[m] == 0)
                            {
                                error_count++; // = penalties[idxTemplate];
                            }
                        }*/

                        // Se erro for maior do que menor erro, passo para o próximo template
                        if (errorCount > minErrors[windowPositionCounter])
                        {
                            break;
                        }
                    }

                    if (widthDifference > 0)
                    {
                        penalizeWiderTemplate = CountValidPixels(templates.ElementAt(idxTemplate).Valor,
                                                                 templateWidth - 1, templateWidth - widthDifference - 1,
                                                                 0, templateHeight - heightDiscount, 0, true);
                    }

                    #region Em Testes - 3

                    if (hold && possiblyMultiple && errorCount == 0)
                    {
                        if (!holding)
                        {
                            tempMinErrors = errorCount; // modificado 23/05, antes era: minErrors[inicioJanela]
                            tempNextIndex = templates.ElementAt(idxTemplate).Valor.Width;
                            tempErrosTemplate.Chave = position;
                            tempErrosTemplate.Valor = new ChaveValor<int, char>
                                                             {
                                                                 Chave = errorCount,
                                                                 Valor = templates.ElementAt(idxTemplate).Chave
                                                             };
                            holding = true;
                        }
                        continue;
                    }

                    #endregion

                    // Se erro atual for menor do que o menor erro computado para essa região da imagem
                    if (errorCount + penalizeWiderTemplate < minErrors[windowPositionCounter])
                    //(int)(inicioJanela / nextIndex)])
                    {
                        minErrors[windowPositionCounter] = errorCount;
                        // modificado 23/05, antes era: minErrors[inicioJanela]
                        nextIndex = templates.ElementAt(idxTemplate).Valor.Width;
                        errosTemplate.Add(new ChaveValor<int, ChaveValor<int, char>>
                                               {
                                                   Chave = position,
                                                   Valor = new ChaveValor<int, char>
                                                               {
                                                                   Chave = errorCount,
                                                                   Valor = templates.ElementAt(idxTemplate).Chave
                                                               }
                                               });
                    }
                    else
                    {
                        if (errorCount + penalizeWiderTemplate == minErrors[windowPositionCounter])
                        {
                            if (templates.ElementAt(idxTemplate).Valor.Width > nextIndex)
                            {
                                minErrors[windowPositionCounter] = errorCount;
                                // modificado 23/05, antes era: minErrors[inicioJanela]
                                nextIndex = templates.ElementAt(idxTemplate).Valor.Width;
                                errosTemplate.Add(new ChaveValor<int, ChaveValor<int, char>>
                                                       {
                                                           Chave = position,
                                                           Valor = new ChaveValor<int, char>
                                                                       {
                                                                           Chave = errorCount,
                                                                           Valor =
                                                                               templates.ElementAt(idxTemplate).Chave
                                                                       }
                                                       });
                            }
                        }
                    }

                    #region Em Testes - 4

                    hold = false;

                    #endregion
                }

                #region Em Testes - 5

                if (holding)
                {
                    if (minErrors[windowPositionCounter] > 40)
                    {
                        minErrors[windowPositionCounter] = tempMinErrors;
                        nextIndex = tempNextIndex;
                        errosTemplate.Add(new ChaveValor<int, ChaveValor<int, char>>
                                               {
                                                   Chave = tempErrosTemplate.Chave,
                                                   Valor = new ChaveValor<int, char>
                                                               {
                                                                   Chave = tempErrosTemplate.Valor.Chave,
                                                                   Valor = tempErrosTemplate.Valor.Valor
                                                               }
                                               });
                    }
                    holding = false;
                }

                #endregion

                if (possiblyMultiple && (inicioJanela + nextIndex) > limiteMenorJanela && !multiple)
                {
                    brk = true;
                    break;
                }
            }

            //int minIndex = Array.IndexOf(error, error.Min());
            //position_template.Add(startPosition[minIndex], error[minIndex]);

            Sb.AppendLine(count++ + ", " + minErrors.Min());

            var result = new char[brk ? numCarac - 1 : numCarac];

            for (var i = 0; i < (brk ? 1 : numCarac); i++)
            {
                result[i] = (from x in errosTemplate
                             where x.Chave == i
                             where x.Valor.Chave == minErrors[i]
                             // modificado em 23/05, antes era: x.Chave < 25
                             select x.Valor.Valor).ElementAt(0);

            }

            return result;
        }

        public StringBuilder GetErrors()
        {
            return Sb;
        }

        public void ClearErrors()
        {
            Sb.Clear();
            count = 0;
        }

        private List<ChaveValor<char, ImgArray>> CarregarTemplates(String arquivoTemplates)
        {
            var format = CultureInfo.CurrentCulture.NumberFormat;
            var separadorDecimal = format.NumberDecimalSeparator.First();

            var result = new List<ChaveValor<char, ImgArray>>();

            using (TextReader tr = new StreamReader(arquivoTemplates))
            {
                String line;
                while ((line = tr.ReadLine()) != null)
                {
                    var temp = line.Split('|');
                    var letra = temp[0].First();
                    int tamX = Convert.ToByte(temp[1]);
                    int tamY = Convert.ToByte(temp[2]);
                    var img = new ImgArray(tamX, tamY);

                    var template = temp[3].Split(';');

                    for (var j = 0; j < template.Length; j++)
                    {
                        template[j] = template[j].Replace(',', separadorDecimal);
                        img[j] = byte.Parse(template[j], NumberStyles.Any);
                    }
                    result.Add(new ChaveValor<char, ImgArray>
                                      {
                                          Chave = letra,
                                          Valor = img
                                      });
                }
            }

            return result;
        }

        private int CountValidPixels(ImgArray img, int xStart, int xEnd, int yStart, int yEnd, int validColor,
                                     bool decrementX = false, bool decrementY = false)
        {
            var counter = 0;

            switch (decrementX)
            {
                case true:
                    {
                        if (decrementY)
                        {
                            for (var x = xStart; x > xEnd; x--)
                            {
                                for (var y = yStart; y > yEnd; y--)
                                {
                                    if (img.GetPixel(x, y).R == validColor)
                                    {
                                        counter++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (var x = xStart; x > xEnd; x--)
                            {
                                for (var y = yStart; y < yEnd; y++)
                                {
                                    if (img.GetPixel(x, y).R == validColor)
                                    {
                                        counter++;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case false:
                    {
                        if (decrementY)
                        {
                            for (var x = xStart; x < xEnd; x++)
                            {
                                for (var y = yStart; y > yEnd; y--)
                                {
                                    if (img.GetPixel(x, y).R == validColor)
                                    {
                                        counter++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (var x = xStart; x < xEnd; x++)
                            {
                                for (var y = yStart; y < yEnd; y++)
                                {
                                    if (img.GetPixel(x, y).R == validColor)
                                    {
                                        counter++;
                                    }
                                }
                            }
                        }
                        break;
                    }
            }

            return counter;
        }

        private int[] OcrTipo2PenalizationAndPixelsToSkip(int widthDifference, int templateHeight,
                                                          int templateWidth, int inicioJanela, int numCarac, int error,
                                                          int errorCount, int idxTemplate, int windowPositionCounter,
                                                          bool hold, bool possiblyMultiple, bool multiple)
        {
            if (widthDifference <= 0)
            {
                int dif;
                if (!multiple)
                {
                    dif = image.Width - templateWidth;
                }
                else
                {
                    dif = image.Width - inicioJanela - templateWidth;
                }

                if (dif > 1)
                {
                    if (numCarac == 1 || image.Width < 20)
                    {
                        errorCount += templateHeight * dif; // penalizo os templates menores
                    }
                    else
                    {
                        if (numCarac > 1 && image.Width >= 19)
                        {
                            if (templateWidth <= 10)
                            {
                                error = (byte)penalties[idxTemplate];
                            }
                        }
                    }
                }
            }

            if (windowPositionCounter + 1 == numCarac)
            {
                if (templateWidth < (image.Width - inicioJanela) - 2)
                {
                    errorCount += CountValidPixels(image, templateWidth, image.Width, 0, image.Height, 0);
                    // varro as colunas que tem na imagem, mas não no template, adicionando o erro correspondente a cada pixel preto à "error_count"
                }
            }

            if (templates.ElementAt(idxTemplate).Chave == 'I')
            {
                #region Em Testes - 2

                hold = true;

                #endregion

                if (!possiblyMultiple)
                {
                    error = 10;
                }
                else
                {
                    error = !multiple ? 15 : 20;
                }
            }

            if (templates.ElementAt(idxTemplate).Chave == '1')
            {
                if (possiblyMultiple)
                {
                    error = 2;
                }
            }

            if (templates.ElementAt(idxTemplate).Chave == 'Z' && templateWidth <= 11)
            {
                errorCount += 15;
            }

            if (templates.ElementAt(idxTemplate).Chave == 'C')
            {
                if (possiblyMultiple)
                {
                    errorCount += 10;
                }
            }

            if (templates.ElementAt(idxTemplate).Chave == 'L' || templates.ElementAt(idxTemplate).Chave == 'J' ||
                templates.ElementAt(idxTemplate).Chave == '7')
            {
                error = !possiblyMultiple ? 2 : 3;
            }

            var result = new int[3];
            result[0] = error;
            result[1] = errorCount;
            result[2] = (hold ? 1 : 0);
            return result;
        }
    }
}