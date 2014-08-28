using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Separacao
{
    public class SeamCarving2
    {
        private static int cont;
        private static ImgArray[] carac;
        private readonly int letras;
        private uint[][] custos;
        private int interval;
        private int qtdeClustersEsperados;
        private int[][] seams;

        public SeamCarving2(int tamanhoDaSaidaEsperada, int interval = 0)
        {
            letras = tamanhoDaSaidaEsperada;
            this.interval = interval;
        }

        //TODO: Nao usar variavel static

        public ImgArray[] GetClusters(ImgArray src, bool incluirDrawCarveNoRetorno = false)
        {
            // TODO: Inserir 1 pixel a mais de borda em todas as imagens na linha de baixo
            // *linha de baixo*

            if (interval == 0)
            {
                interval = src.Width / (letras * 2);
                qtdeClustersEsperados = (letras * 3) + 1;
            }
            else
            {
                qtdeClustersEsperados = letras;
            }

            //src = src.RemoveWhiteBorders(2, 2);
            Carve(src);
            var clusters = CheckClusters(src);

            if (incluirDrawCarveNoRetorno)
            {
                var result = new ImgArray[clusters.Length + 1];
                for (var i = 0; i < clusters.Length; i++)
                {
                    result[i] = clusters[i];
                }
                result[clusters.Length] = DrawCarves(src);
                return result;
            }
            return clusters;
        }

        [Obsolete("Usar ImgArray")]
        public ImgArray CarveAndDrawCarves(Bitmap cdata)
        {
            //cdata = cdata.RemoveWhiteBorders(2, 2);

            if (interval == 0)
            {
                interval = cdata.Width / (letras * 2);
            }

            var I = new ImgArray(cdata);
            //Bitmap I = cdata;
            //Carve(new ImgArray(I));
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

        public Bitmap DrawCarves(Bitmap cdata)
        {
            if (null == seams)
            {
                return cdata;
            }

            //cdata = cdata.RemoveWhiteBorders(2, 2);

            if (interval == 0)
            {
                interval = cdata.Width / (letras * 2);
            }

            for (var i = 0; i < cdata.Width; i += interval)
            {
                var x = i;
                for (var y = cdata.Height - 1; y > 0; y--)
                {
                    cdata.SetPixel(x, y, Color.Red);
                    x += seams[y][x];
                }
            }
            return cdata;
        }

        [Obsolete("Cuidado")]
        public ImgArray CarveWithReturn(ImgArray cdata)
        {
            //cdata = cdata.RemoveWhiteBorders(2, 2);

            if (interval == 0)
            {
                interval = cdata.Width / (letras * 2);
            }

            var result = new ImgArray(cdata);

            //Bitmap result = cdata.ToBitmap();
            //Carve(new ImgArray(result));

            Carve(result);

            /*
            for (int i = 0; i < result.Width; i += interval)
            {
                int x = i;
                for (int y = result.Height - 1; y > 0; y--)
                {
                    result.SetPixel(x, y, Color.Black);
                    x += seams[y][x];
                }
            }*/
            return result;
        }

        public ImgArray DrawCarves(ImgArray cdata)
        {
            if (null == seams)
            {
                return cdata;
            }

            //cdata = cdata.RemoveWhiteBorders(2, 2);

            if (interval == 0)
            {
                interval = cdata.Width / (letras * 2);
            }

            for (var i = 0; i < cdata.Width; i += interval)
            {
                var x = i;
                for (var y = cdata.Height - 1; y > 0; y--)
                {
                    cdata.SetPixel(x, y, Color.Black);
                    x += seams[y][x];
                }
            }
            return cdata;
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

        private ImgArray[] CheckClusters(ImgArray img, int count = 0)
        {
            //int interval = img.Width / (letras * 2);

            try
            {
                cont = count;
                var filledCluster = new List<bool>(new bool[qtdeClustersEsperados]);
                carac = new ImgArray[qtdeClustersEsperados];
                var width = new List<int>(new int[qtdeClustersEsperados]);
                var countPixel = new List<int>(qtdeClustersEsperados);
                var caracReturn = new ImgArray[letras];
                var iters = -1;
                // width.Initialize();

                for (var i = 0; i < img.Width; i += interval)
                {
                    var x = (i + interval) >= img.Width ? (img.Width - 1) : (i + interval) % img.Width;
                    var k = i;

                    countPixel.Add(0);
                    iters++;

                    if (filledCluster.Count <= iters)
                    {
                        filledCluster.Add(false);
                        width.Add(0);
                    }


                    for (var y = img.Height - 1; y >= 0; y--)
                    {
                        if ((i / interval < width.Count) && (width[i / interval] < (x - 1) - (k + 1)))
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
                                    carac[cont] = new ImgArray(img.Width, img.Height);
                                }
                                carac[cont].SetPixel(j, y, Color.FromArgb(0, 0, 0));
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

                if (cont == letras)
                {
                    for (var i = 0; i < letras; i++)
                    {
                        caracReturn[i] = carac[i].RemoveWhiteBorders();
                    }

                    return caracReturn;
                }

                /*if (naoSepararGrudado)
                {
                    return new ImgArray[1]; // BLL do ConsigRJ
                }*/

                if (cont < letras)
                {
                    SeparaCharGrudado(width.ToArray(), filledCluster.ToArray());
                    for (var i = 0; i < letras; i++)
                    {
                        caracReturn[i] = carac[i].RemoveWhiteBorders();
                    }

                    return caracReturn;
                }

                if (cont > letras)
                {
                    //Bitmap[] caracMaiores = new Bitmap[letras];
                    var temp = new ArrayList();
                    var min = new int[countPixel.Count];
                    var comp = new int[letras + 4];
                    var idx = 0;

                    for (var i = 0; i < countPixel.Count; i++)
                    {
                        temp.Add(countPixel[i]);
                        min[i] = countPixel[i];
                        //TODO: O que representa esse numero 26 abaixo?
                        if (countPixel[i] >= 26)
                        {
                            comp[idx++] = countPixel[i];
                        }
                    }

                    Array.Sort(min);
                    idx = 0;
                    //TODO: O que representa esse 4 HardCoded? É porque o nosso primeiro captcha tinha 4 letras?
                    //17/11/2012: while (temp.Count > 4)
                    while (temp.Count > letras)
                    {
                        temp.Remove(min[idx++]);
                    }

                    for (var i = 0; i < letras; i++)
                    {
                        caracReturn[i] = carac[Array.IndexOf(comp, temp[i])].RemoveWhiteBorders();
                    }
                    return caracReturn;
                }
            }
            catch (Exception e)
            {
                var bmp = new ImgArray[4];

                var aux = new Bitmap(1, 1);
                var g1 = Graphics.FromImage(aux);
                var fonte = new Font("Arial", 8);
                bmp[0] =
                    new ImgArray(new Bitmap((int)g1.MeasureString(e.Message, fonte).Width, 60).InserirFundoBranco());
                var g = Graphics.FromImage(bmp[0].ToBitmap());
                g.DrawString(e.Message, fonte, new SolidBrush(Color.Black), 1, 1);
                bmp[1] = new ImgArray(new Bitmap(60, 60).InserirFundoBranco());
                bmp[2] = new ImgArray(new Bitmap(60, 60).InserirFundoBranco());
                bmp[3] = new ImgArray(new Bitmap(60, 60).InserirFundoBranco());

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
            var sc = new[] { new SeamCarving2(2), new SeamCarving2(2) };

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
                            carac[indiceC[num]].ToBitmap().CopiarBitmapPara(ref ch[num]);

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

                ch[num] = sc[num].CarveWithReturn(new ImgArray(ch[num]).RemoveWhiteBorders(2, 2)).ToBitmap();

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
            else
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

            //TODO: TAMANHO DO ARQUIVO HARDCODED
            carac[count] = new ImgArray(200, 90);
            carac[count + 1] = new ImgArray(200, 90);

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

        private void Carve(ImgArray src)
        {
            custos = new uint[src.Height][];
            seams = new int[src.Height][];

            var nrows = src.Height;
            var ncols = src.Width;

            for (var i = 0; i < src.Height; i++)
            {
                custos[i] = new uint[src.Width];
                seams[i] = new int[src.Width];
            }

            for (var x = 0; x < ncols; x++)
            {
                for (var y = 0; y < nrows; y++)
                {
                    if (src.GetPixel(x, y).IsBlackPixel())
                    {
                        custos[y][x] = 1;
                    }
                }
            }

            for (var y = 2; y < nrows; y++)
            {
                for (var x = 0; x < ncols; x++)
                {
                    if (src.GetPixel(x, y).IsBlackPixel())
                    {
                        custos[y][x] = custos[y - 1][x] + 1;
                    }
                    else
                    {
                        custos[y][x] = custos[y - 1][x];
                    }
                }

                for (var x = 1; x < ncols; x++)
                {
                    uint leftCost;
                    if (src.GetPixel(x, y).IsBlackPixel())
                    {
                        leftCost = custos[y - 1][x - 1] + 1;
                    }
                    else
                    {
                        leftCost = custos[y - 1][x - 1];
                    }
                    if (leftCost < custos[y][x])
                    {
                        custos[y][x] = leftCost;
                        seams[y][x] = -1;
                    }
                }

                for (var x = 0; x < ncols - 1; x++)
                {
                    uint rightCost;
                    if (src.GetPixel(x, y).IsBlackPixel())
                    {
                        rightCost = custos[y - 1][x + 1] + 1;
                    }
                    else
                    {
                        rightCost = custos[y - 1][x + 1];
                    }
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