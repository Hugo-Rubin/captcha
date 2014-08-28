using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Logic.Types;

namespace Core.Logic.Obsolete
{
    [Obsolete("Essa classe deixou de ser usada, utilize SeamCarving2")]
    public class SeamCarving
    {
        private const int Letras = 4;
        private const int Clusters = 13;
        private static int cont;

        //TODO: Nao usar variavel static
        private static Bitmap[] carac;
        private uint[][] custos;
        private int[][] seams;

        [Obsolete("Usar ImgArray")]
        public Bitmap DrawCarves(Bitmap cdata, int interval)
        {
            if (interval <= 0)
                interval = 10;

            var I = new Bitmap(cdata);
            Carve(I);

            for (var i = 0; i < I.Width; i += interval)
            {
                var x = i;
                for (var y = I.Height - 1; y > 0; y--)
                {
                    I.SetPixel(x, y, Color.Red);
                    x += seams[y][x];
                }
            }

            return I;
        }

        public ImgArray DrawCarves(ImgArray cdata, int interval)
        {
            if (interval <= 0)
            {
                interval = 10;
            }
            var result = new ImgArray(cdata);
            Carve(result.ToBitmap());
            for (var i = 0; i < result.Width; i += interval)
            {
                var x = i;
                for (var y = result.Height - 1; y > 0; y--)
                {
                    result.SetPixel(x, y, Color.Red);
                    x += seams[y][x];
                }
            }
            return result;
        }

        public Bitmap ShowCosts()
        {
            var max = custos.Max(c => c.Max());

            var bmp = new Bitmap(custos[0].Length, custos.Length);

            for (var i = 0; i < custos.Length; i++)
            {
                for (var j = 0; j < custos[i].Length; j++)
                {
                    var cor = (int)Math.Round((double)custos[i][j] / max * 255);
                    bmp.SetPixel(j, i, Color.FromArgb(cor, cor, cor));
                }
            }

            return bmp;
        }

        [Obsolete("Usar ImgArray")]
        public Bitmap[] CheckClusters(Bitmap img, int interval, int count)
        {
            try
            {
                cont = count;
                var filledCluster = new bool[Clusters];
                carac = new Bitmap[Clusters];
                var width = new int[Clusters];
                var countPixel = new int[Clusters];
                var caracReturn = new Bitmap[4];
                var iters = -1;
                width.Initialize();

                for (var i = 0; i < img.Width; i += interval)
                {
                    var x = (i + interval) % img.Width;
                    var k = i;
                    countPixel[++iters] = 0;

                    for (var y = img.Height - 1; y > 0; y--)
                    {
                        if ((i / interval < width.Length) && (width[i / interval] < (x - 1) - (k + 1)))
                        {
                            width[i / interval] = (x - 1) - (k + 1);
                        }

                        k += seams[y][k];

                        for (var j = k + 1; j < x; j++)
                        {
                            if (img.GetPixel(j, y).IsBlackPixel())
                            {
                                countPixel[iters]++;
                                if (!filledCluster[i / interval])
                                {
                                    filledCluster[i / interval] = true;
                                    carac[cont] = new Bitmap(img.Width, img.Height);
                                    var g = Graphics.FromImage(carac[cont]);
                                    g.Clear(Color.White);

                                    carac[cont].SetPixel(j, y, Color.FromArgb(0, 0, 0));
                                }
                                else
                                {
                                    carac[cont].SetPixel(j, y, Color.FromArgb(0, 0, 0));
                                }
                            }
                        }
                        x += seams[y][x];
                    }

                    if (filledCluster[i / interval] && countPixel[iters] < 26)
                    {
                        filledCluster[i / interval] = false;
                    }

                    if (filledCluster[i / interval]) // && cont < letras)
                    {
                        cont++;
                    }
                }

                if (cont == Letras)
                {
                    for (var i = 0; i < Letras; i++)
                    {
                        caracReturn[i] = carac[i];
                    }

                    return caracReturn;
                }

                if (cont < Letras)
                {
                    SeparaCharGrudado(width, filledCluster);
                    for (var i = 0; i < Letras; i++)
                    {
                        caracReturn[i] = carac[i];
                    }

                    return caracReturn;
                }

                if (cont > Letras)
                {
                    //Bitmap[] caracMaiores = new Bitmap[letras];
                    var temp = new ArrayList();
                    var min = new int[countPixel.Length];
                    var comp = new int[Letras + 4];
                    var idx = 0;

                    for (var i = 0; i < countPixel.Length; i++)
                    {
                        temp.Add(countPixel[i]);
                        min[i] = countPixel[i];
                        if (countPixel[i] >= 26)
                        {
                            comp[idx++] = countPixel[i];
                        }
                    }

                    Array.Sort(min);
                    idx = 0;

                    while (temp.Count > 4)
                    {
                        temp.Remove(min[idx++]);
                    }

                    for (var i = 0; i < Letras; i++)
                    {
                        caracReturn[i] = carac[Array.IndexOf(comp, temp[i])];
                    }

                    /*caracMaiores[0] = carac[Array.IndexOf(comp, temp[0])];
                    caracMaiores[1] = carac[Array.IndexOf(comp, temp[1])];
                    caracMaiores[2] = carac[Array.IndexOf(comp, temp[2])];
                    caracMaiores[3] = carac[Array.IndexOf(comp, temp[3])];*/

                    return caracReturn;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);

                var bmp = new Bitmap[4];

                var aux = new Bitmap(1, 1);
                var g1 = Graphics.FromImage(aux);
                var fonte = new Font("Arial", 8);
                bmp[0] = new Bitmap((int)g1.MeasureString(e.Message, fonte).Width, 60).InserirFundoBranco();
                var g = Graphics.FromImage(bmp[0]);
                g.DrawString(e.Message, fonte, new SolidBrush(Color.Red), 1, 1);
                bmp[1] = new Bitmap(60, 60).InserirFundoBranco();
                bmp[2] = new Bitmap(60, 60).InserirFundoBranco();
                bmp[3] = new Bitmap(60, 60).InserirFundoBranco();

                return bmp;
            }

            return carac;
        }


        private void SeparaCharGrudado(int[] width, IList<bool> filledCluster)
        {
            var ch = new Bitmap[2];
            var maxCost = new uint[2];
            var initCost = new uint[2];
            var intervalo = new int[2];
            var num = 0;
            var currentIndex = 0;
            var sc = new[] { new SeamCarving(), new SeamCarving() };

            var maiorC1 = 0;
            var maiorC2 = 0;

            ch.Initialize();
            maxCost.Initialize();
            initCost.Initialize();
            intervalo.Initialize();

            var indiceC = new int[2];

            foreach (var item in width)
            {
                if (maiorC1 < item)
                {
                    if (maiorC2 < maiorC1)
                        maiorC2 = maiorC1;
                    maiorC1 = item;
                    indiceC[0] = width.ToList().IndexOf(maiorC1);
                }

                if (indiceC[0] != currentIndex && item <= maiorC1 && maiorC2 < item)
                    maiorC2 = item;

                currentIndex++;
            }


            indiceC[1] = width.ToList().IndexOf(maiorC2);

            for (var pos = indiceC[0]; pos > 0; pos--)
            {
                if (!filledCluster[pos - 1])
                    indiceC[0]--;
            }

            for (var pos = indiceC[1]; pos > 0; pos--)
            {
                if (!filledCluster[pos - 1])
                    indiceC[1]--;
            }

            do
            {
                var brk = false;

                for (var y = carac[indiceC[num]].Height - 1; y > 4; y--)
                {
                    if (brk) break;

                    for (var x = 0; x < carac[indiceC[num]].Width; x++)
                    {
                        //if (carac[indiceC[num]].GetPixel(x, y).IsBlackPixel())
                        if (custos[y][x] > 0 && custos[y][x] == custos[y - 3][x] && custos[y][x] == custos[y - 4][x])
                        {
                            ch[num] = new Bitmap(carac[indiceC[num]].Width, y + 1);
                            carac[indiceC[num]].CopiarBitmapPara(ref ch[num]);

                            initCost[num] = custos[y][x];
                            intervalo[num] = x;

                            /*for (int i = x + 1; i < carac[indiceC[num]].Width; i++)
                            {
                                if (!carac[indiceC[num]].GetPixel(i, y).IsBlackPixel())
                                {
                                    intervalo[num] = i + 5;
                                    initCost[num] = custos[y][i];
                                    break;
                                }
                            }*/

                            brk = true;
                            break;
                        }
                    }
                }

                ch[num] = sc[num].DrawCarves(ch[num], intervalo[num]);

                for (var i = 0; i < ch[num].Width; i += intervalo[num])
                {
                    var x = i;
                    for (var y = ch[num].Height - 1; y > 0; y--)
                    {
                        if (sc[num].custos[y][x] > maxCost[num])
                            maxCost[num] = sc[num].custos[y][x];

                        x += sc[num].seams[y][x];
                    }
                }

                num++;
            } while (num < 2);

            var custo0 = Math.Abs((int)maxCost[0] - (int)initCost[0]);
            var custo1 = Math.Abs((int)maxCost[1] - (int)initCost[1]);

            if (custo0 < custo1) // se o custo[0] for menor que custo[1], aplico a separação em 0 e descarto 1.
            {
                for (var i = carac.Length - 1; i > indiceC[0]; i--)
                {
                    carac[i] = carac[i - 1];
                    // desloco os caracteres armazenados à direita do caractere atual uma posição à direita
                }

                sc[0].DesenhaCharGrudado(sc[0].seams, ch[0], intervalo[0], indiceC[0]);
                // "recebe" carac com os 4 caracteres separados
            }

            else if (custo0 > custo1) // se o custo[1] for menor que custo[0], aplico a separação em 1 e descarto 0.
            {
                for (var i = carac.Length - 1; i > indiceC[1]; i--)
                {
                    carac[i] = carac[i - 1];
                }

                sc[1].DesenhaCharGrudado(sc[1].seams, ch[1], intervalo[1], indiceC[1]);
            }
            else if (custo0 == custo1)
                if (custo0 < 4)
                {
                    sc[0].DesenhaCharGrudado(sc[0].seams, ch[0], intervalo[0], indiceC[0]);
                    sc[1].DesenhaCharGrudado(sc[1].seams, ch[1], intervalo[1], indiceC[1]);
                }
        }

        [Obsolete("Usar ImgArray")]
        private void DesenhaCharGrudado(IList<int[]> seam, Bitmap ch, int interval, int posicao)
        {
            const int numLetras = 2;
            var count = posicao;

            carac[count] = new Bitmap(200, 90);
            carac[count + 1] = new Bitmap(200, 90);

            for (var i = 0; i < ch.Width; i += interval)
            {
                int x;

                if ((i + interval) >= ch.Width)
                    x = ch.Width - 1;
                else
                    x = i + interval;

                var k = i;

                for (var y = ch.Height - 1; y > 0; y--)
                {
                    k += seam[y][k];

                    for (var j = k + 1; j < x; j++)
                    {
                        if (ch.GetPixel(j, y).IsBlackPixel())
                            carac[count].SetPixel(j, y, Color.FromArgb(0, 0, 0));
                    }

                    x += seam[y][x];
                }
                if (count <= numLetras)
                    count++;
            }
        }

        [Obsolete("Usar ImgArray")]
        private void Carve(Bitmap I)
        {
            custos = new uint[I.Height][];
            seams = new int[I.Height][];

            var nrows = I.Height;
            var ncols = I.Width;

            for (var i = 0; i < I.Height; i++)
            {
                custos[i] = new uint[I.Width];
                seams[i] = new int[I.Width];
            }

            for (var x = 0; x < ncols; x++)
            {
                for (var y = 0; y < nrows; y++)
                {
                    if (I.GetPixel(x, y).IsBlackPixel())
                        custos[y][x] = 1;
                    else
                        custos[y][x] = 0;
                }
            }

            for (var y = 2; y < nrows; y++)
            {
                for (var x = 0; x < ncols; x++)
                {
                    if (I.GetPixel(x, y).IsBlackPixel())
                        custos[y][x] = custos[y - 1][x] + 1;
                    else
                        custos[y][x] = custos[y - 1][x];
                }

                for (var x = 1; x < ncols; x++)
                {
                    uint leftCost;
                    if (I.GetPixel(x, y).IsBlackPixel())
                        leftCost = custos[y - 1][x - 1] + 1;
                    else
                        leftCost = custos[y - 1][x - 1];

                    if (leftCost < custos[y][x])
                    {
                        custos[y][x] = leftCost;
                        seams[y][x] = -1;
                    }
                }

                for (var x = 0; x < ncols - 1; x++)
                {
                    uint rightCost;
                    if (I.GetPixel(x, y).IsBlackPixel())
                        rightCost = custos[y - 1][x + 1] + 1;
                    else
                        rightCost = custos[y - 1][x + 1];

                    if (rightCost < custos[y][x])
                    {
                        custos[y][x] = rightCost;
                        seams[y][x] = 1;
                    }
                }
            }
        }
    }
}