using System;
using System.Drawing;
using Core.Logic.Types;

namespace Core.Logic.Tratamento
{
    /*Precisa de várias melhorias como a remoção de variáveis desnecessárias, passar oas variáveis da função cosseno por parâmetros,
     * remoção da matrix de conversão (não é necessária) entre outros.*/

    public class Endireitamento
    {
        private readonly uint[][] custo;
        private readonly bool endireitarEixoX;
        private readonly bool endireitarEixoY;
        private readonly int intervalo;
        private readonly bool sc;
        private readonly int[][] seam;

        /**
         * Construtor padrão. O efeito é aplicado no eixo x e y
         */

        public Endireitamento()
        {
            endireitarEixoX = true;
            endireitarEixoY = true;
        }

        public Endireitamento(bool endireitarEixoX, bool endireitarEixoY)
        {
            this.endireitarEixoX = endireitarEixoX;
            this.endireitarEixoY = endireitarEixoY;
        }

        public Endireitamento(bool endireitarEixoX, bool endireitarEixoY, bool sc, int[][] seam, uint[][] cost,
                              int intervalo)
        {
            this.endireitarEixoX = endireitarEixoX;
            this.endireitarEixoY = endireitarEixoY;
            this.sc = sc;
            this.seam = seam;
            custo = cost;
            this.intervalo = intervalo;
        }


        //TODO: Refatorar esse salsichão
        public int TipoCurva(ImgArray src)
        {
            var preto = Color.FromArgb(0, 0, 0);
            Point topDireita, bottomEsquerda, bottomDireita, bordaEsq, bordaDir;

            var topEsquerda = topDireita = bottomEsquerda = bottomDireita = bordaEsq = bordaDir = new Point();

            var brk = false;


            //------
            int nPixelsQ2, nPixelsQ3, nPixelsQ4;
            var nPixelsQ1 = nPixelsQ2 = nPixelsQ3 = nPixelsQ4 = 0;
            //------


            // Alterar o código para comparar o primeiro caractere com o último apenas

            // Encontra os limites esquerdo e direito da parte com texto
            for (var i = 0; i < src.Width; i++)
            {
                if (brk)
                    break;
                for (var j = 0; j < src.Height; j++)
                {
                    var c = src.GetPixel(i, j);

                    if (c.Equals(preto))
                    {
                        bordaEsq.X = j;
                        bordaEsq.Y = i;

                        for (var k = src.Width - 1; k > i; k--)
                        {
                            if (brk)
                                break;
                            for (var l = 0; l < src.Height; l++)
                            {
                                var cor = src.GetPixel(k, l);

                                if (cor.Equals(preto))
                                {
                                    bordaDir.X = l;
                                    bordaDir.Y = k;
                                    brk = true;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }

            brk = false;
            var meio = (bordaDir.Y + bordaEsq.Y) / 2;
            // Encontra o meio da parte válida da imagem (do começo do 1º caractere até o final do 4º e último)

            //Encontra o pixel mais alto à esquerda do meio da imagem
            for (var j = 0; j < src.Height; j++)
            {
                if (brk)
                    break;
                for (var i = 0; i < meio; i++)
                {
                    var c = src.GetPixel(i, j);

                    if (c.Equals(preto))
                    {
                        topEsquerda.X = j;
                        topEsquerda.Y = i;
                        brk = true;
                        break;
                    }
                }
            }

            brk = false;

            //Encontra o pixel mais baixo à esquerda do pixel mais alto da imagem
            for (var j = src.Height - 1; j > topEsquerda.X; j--)
            {
                if (brk)
                    break;
                for (var i = 0; i < topEsquerda.Y + 5; i++)
                //Até 5 pixels à direita do pixel mais alto para garantir que o pixel pertence ao mesmo caractere
                {
                    var c = src.GetPixel(i, j);

                    if (c.Equals(preto))
                    {
                        bottomEsquerda.X = j;
                        bottomEsquerda.Y = i;
                        brk = true;
                        break;
                    }
                }
            }

            brk = false;

            //Encontra o pixel mais alto à direita do meio da imagem
            for (var j = 0; j < src.Height; j++)
            {
                if (brk)
                    break;
                for (var i = meio; i < src.Width; i++)
                {
                    var c = src.GetPixel(i, j);

                    if (c.Equals(preto))
                    {
                        topDireita.X = j;
                        topDireita.Y = i;
                        brk = true;
                        break;
                    }
                }
            }

            brk = false;

            //Encontra o pixel mais baixo à direita do pixel mais alto da imagem
            for (var j = src.Height - 1; j > topDireita.X; j--)
            {
                if (brk)
                    break;
                for (var i = src.Width - 1; i > topDireita.Y - 5; i--)
                //Até 5 pixels à esquerda do pixel mais alto para garantir que o pixel pertence ao mesmo caractere
                {
                    var c = src.GetPixel(i, j);

                    if (c.Equals(preto))
                    {
                        bottomDireita.X = j;
                        bottomDireita.Y = i;
                        brk = true;
                        break;
                    }
                }
            }


            //--------
            var val2 = 0;

            brk = false;

            for (var i = src.Height - 1; i > 0; i--)
            {
                if (brk)
                    break;
                for (var j = 0; j < src.Width; j++)
                {
                    var c = src.GetPixel(j, i);

                    if (c.Equals(preto))
                    {
                        val2 = i;
                        brk = true;
                        break;
                    }
                }
            }

            var val1 = topEsquerda.X < topDireita.X ? topEsquerda.X : topDireita.X;

            var meioH = (val1 + val2) / 2;

            for (var i = bordaEsq.Y; i < meio; i++)
            {
                for (var j = topEsquerda.X; j < meioH; j++)
                {
                    var c = src.GetPixel(i, j);

                    if (c.IsBlackPixel())
                        nPixelsQ1++;
                }
            }

            for (var i = bordaEsq.Y; i < meio; i++)
            {
                for (var j = meioH; j < val2 + 1; j++)
                {
                    var c = src.GetPixel(i, j);

                    if (c.IsBlackPixel())
                        nPixelsQ2++;
                }
            }

            for (var i = meio; i < bordaDir.Y + 1; i++)
            {
                for (var j = topDireita.X; j < meioH; j++)
                {
                    var c = src.GetPixel(i, j);

                    if (c.IsBlackPixel())
                        nPixelsQ3++;
                }
            }

            for (var i = meio; i < bordaDir.Y; i++)
            {
                for (var j = meioH; j < val2 + 1; j++)
                {
                    var c = src.GetPixel(i, j);

                    if (c.IsBlackPixel())
                        nPixelsQ4++;
                }
            }

            if (nPixelsQ1 > nPixelsQ3 || nPixelsQ4 > nPixelsQ2)
                return 1;
            return 2;
            //-------- 


            /*int tamanhoEsq, tamanhoDir;
            
            //Guarda o tamanho aproximado do caractere mais à esquerda e mais à direita
            tamanhoEsq = bottomEsquerda.X - topEsquerda.X;
            tamanhoDir = bottomDireita.X - topDireita.X;

            if (tamanhoEsq > tamanhoDir)
            {
                //Arruma o tamanho do caractere esquerdo para deixá-lo do mesmo tamanho do direito
                int corrigeEsquerda = bottomEsquerda.X - tamanhoDir + 1;  /// Arrumar aqui <------------------------------
                if (corrigeEsquerda > topDireita.X)
                    return 1; // esquerda
                else
                    return 2; // direita
            }
            else
            {
                //Arruma o tamanho do caractere direito para deixá-lo do mesmo tamanho do esquerdo
                int corrigeDireita = bottomDireita.X - tamanhoEsq + 1;  /// Arrumar aqui <------------------------------
                if (corrigeDireita > topEsquerda.X)
                    return 2; // direita
                else
                    return 1; // esquerda
            }*/

            /* Como era:
            for (int j = 0; j < src.Height; j++)
            {
                for (int i = 0; i < src.Width; i++)
                {
                    Color c = src.GetPixel(i, j);

                    if (c.Equals(preto))
                    {
                        if (i < src.Width / 2)
                            return 1; // esquerda
                        else
                            return 2; // direita
                    }
                }
            }*/
        }

        //TODO: Refatorar esse salsichão
        public ImgArray ApplyTo(ImgArray src)
        {
            var srci = new ImgArray(src);
            var invertido = false;

            if (TipoCurva(src) == 2)
            {
                for (var y = 1; y < src.Height; y++)
                    for (var x = 1; x < src.Width; x++)
                    {
                        srci.SetPixel(src.Width - x, y, src.GetPixel(x, y));
                    }
                src = srci;
                invertido = true;
            }


            var w = src.Width;
            var h = src.Height;
            //TODO: Remover uso de matrizes desnecessárias
            var resultMatrix = new int[w][]; //h];

            //int[,] resultMatrix = new int[w, h]; // VERIFICAÇÃO DA MATRIZ


            // Copia para matriz
            for (var i = 0; i < w; i++)
            {
                resultMatrix[i] = new int[h];
                for (var j = 0; j < h; j++)
                {
                    var value = src.GetPixel(i, j).ToArgb();
                    //resultMatrix[i][j] = value;
                    resultMatrix[i][j] = value; // VERIFICAÇÃO DA MATRIZ
                }
            }
            var transformY = new int[w];
            if (endireitarEixoY)
            {
                // Cosseno horizontal
                for (var i = 0; i < w; i++)
                {
                    // TODO: Colocar variáveis como parâmetro
                    var x = (i / (double)(w - 1)) * 2.8738371546749493 * Math.PI; // Antes 2.8738371546749493 era 3
                    var valor = 0.28914178330494367 * (((Math.Cos(x) + 1) * ((h - 1) / 2.0)));
                    // Antes 0.28914178330494367 era 0.27
                    transformY[i] = (int)valor;
                }
            }
            var increment = new MinMaxReturn(transformY);
            var transformX = new int[h + increment.GetDelta()];
            var transformMatrix = new int[w][]; //h + increment.getDelta()];
            //int[][] transformMatrix = new int[(w)][h + increment.getDelta()]; // VERIFICAÇÃO DA MATRIZ
            for (var i = 0; i < w; i++)
            {
                transformMatrix[i] = new int[h + increment.GetDelta()];
                transformMatrix[i].Init(0xffffff);
                for (var j = 0; j < h; j++)
                {
                    var desloc = (j - transformY[i]);
                    transformMatrix[i][increment.GetSum() + desloc] = resultMatrix[i][j];
                }
            }

            if (endireitarEixoX)
            {
                // Cosseno vertical
                for (var i = 0; i < h + increment.GetDelta(); i++)
                {
                    // TODO: Colocar variáveis como parâmetro
                    var y = (i / (double)(h - 1)) * 0.037927948518231 * Math.PI + 16;
                    // Antes 0.5 era 0.037927948518231... e antes era 1.5
                    var valor = (((2 * Math.Cos(y) + 1) * ((w - 1) / 2.0)) * 0.005906369580265802) + 7;
                    // Antes 0.005906369580265802... era 0.07
                    transformX[i] = (int)valor;
                }
            }
            var incrementX = new MinMaxReturn(transformX);
            var transform2Matrix = new int[(w + incrementX.GetDelta())][]; // [h + increment.getDelta()];
            for (var i = 0; i < transform2Matrix.Length; i++)
            {
                transform2Matrix[i] = new int[h + increment.GetDelta()];
                transform2Matrix[i].Init(0xffffff);
            }
            for (var j = h + increment.GetDelta() - 1; j >= 0; j--)
            {
                for (var i = w - 1; i >= 0; i--)
                {
                    var desloc = (i - transformX[j]);
                    transform2Matrix[incrementX.Max + desloc][j] = transformMatrix[i][j];
                }
            }
            var resultImage = new int[(w + incrementX.GetDelta()) * (h + increment.GetDelta())];
            // Matrix de conversão
            for (var i = 0; i < transform2Matrix.Length; i++)
            {
                for (var j = 0; j < transform2Matrix[0].Length; j++)
                {
                    resultImage[j * (transform2Matrix.Length) + i] = transform2Matrix[i][j];
                }
            }
            var dest = new ImgArray(transform2Matrix.Length, transform2Matrix[0].Length);

            var lin = -1;

            for (var x = 0; x < resultImage.Length; x++)
            {
                var linha = x % dest.Width;
                int coluna;
                if (linha == 0)
                {
                    lin++;
                    coluna = 1; //x % dest.Height; 
                }
                else
                {
                    if (x == 0)
                        coluna = 1;
                    else
                        coluna = x % dest.Width;
                }


                var red = (resultImage[x] >> 16) & 0xFF;
                var green = (resultImage[x] >> 8) & 0xFF;
                var blue = resultImage[x] & 0xFF;
                var c = Color.FromArgb(red, green, blue);

                dest.SetPixel(coluna, lin, c);
            }

            if (invertido)
            {
                var srcii = new ImgArray(dest);
                for (var y = 1; y < dest.Height; y++)
                    for (var x = 1; x < dest.Width; x++)
                    {
                        srcii.SetPixel(dest.Width - x, y, dest.GetPixel(x, y));
                    }
                return srcii;
            }

            //if(sc)
            //dest = CorrigeSeamCarving(dest, seam, custo, intervalo);

            //dest = new Bitmap(transform2Matrix.Length, transform2Matrix[0].Length); // *1
            //dest.SetPixel(0, 0, transform2Matrix.Length, transform2Matrix[0].Length, resultImage, 0, transform2Matrix.Length); // *2

            var result = new ImgArray(src.Width, src.Height);
            var margemX = (dest.Width - 200) / 2;
            var margemY = (dest.Height - 90) / 2;

            for (var x = margemX; x < dest.Width - margemX - 1; x++)
            {
                for (var y = margemY; y < dest.Height - margemY - 2; y++)
                {
                    result.SetPixel(x - margemX, y - margemY, dest.GetPixel(x, y));
                }
            }

            if (sc)
                result = CorrigeSeamCarving(result, seam, custo, intervalo);

            return result;
            //return dest;
        }

        //TODO:Melhorar tirando matrizes
        public ImgArray CorrigeSeamCarving(ImgArray img, int[][] seam, uint[][] cost, int intervalo)
        {
            for (var i = 0; i < img.Width; i += intervalo)
            {
                var x = i;

                for (var y = img.Height - 1; y > 0; y--)
                {
                    img.SetPixel(x, y, Color.Red);
                    x += seam[y][x];
                }
            }
            return img;
        }

        public int[][] GetSeam()
        {
            return seam;
        }

        public uint[][] GetCost()
        {
            return custo;
        }
    }
}