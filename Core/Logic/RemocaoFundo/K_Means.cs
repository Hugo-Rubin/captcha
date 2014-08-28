using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Core.Logic.RemocaoFundo
{
    public class KMeans
    {
        /* Algoritmo =>
        * 1) Escolha um número fixo de grupos (clusters)
        * 2) Escolha os centros dos grupos
        * 3) Associe cada um dos pontos ao centro mais próximo
        * 4) Quando todos os pontos foram assinalados, recalcule a posição dos centros
        * 5) Repita os passos 3) e 4) até que os centros não se movam mais.
        */

        private readonly Bitmap imagem;
        private readonly int numGrupos;
        private List<int> centros;
        private List<List<Point>> grupos;

        public KMeans(int numGrupos, Image image)
        {
            this.numGrupos = numGrupos;
            imagem = (Bitmap)image;
            SetDefaultCenters();
            grupos = new List<List<Point>>();

            for (var i = 0; i < this.numGrupos; i++)
            {
                grupos.Add(new List<Point>());
            }
        }

        public Bitmap Apply()
        {
            var terminou = false;
            while (!terminou)
            {
                for (var y = 0; y < imagem.Height; y++)
                {
                    for (var x = 0; x < imagem.Width; x++)
                    {
                        var c = new Point(x, y);
                        CloserCenter(c);
                    }
                }
                var newCenters = NewCenters();
                if (!CompareCenters(newCenters))
                {
                    centros = newCenters;
                    grupos = new List<List<Point>>();
                    for (var i = 0; i < numGrupos; i++)
                    {
                        grupos.Add(new List<Point>());
                    }
                }
                else
                {
                    terminou = true;
                }
            }
            return PaintImage();
        }

        private Bitmap PaintImage()
        {
            var result = new Bitmap(imagem.Width, imagem.Height, PixelFormat.Format32bppRgb);
            for (var i = 0; i < numGrupos; i++)
            {
                foreach (var coord in grupos[i])
                {
                    //System.out.println(this.grupos.get(i).size() + " pixel com Cor = " + i*part*255*this.numGrupos*2);
                    var c = (int)Math.Round((decimal)i * 255 / (numGrupos - 1));
                    var r = (c >> 16) & 0x000000FF;
                    var g = (c >> 8) & 0x000000FF;
                    var b = c & 0x000000FF;
                    result.SetPixel(coord.X, coord.Y, Color.FromArgb(255, r, g, b));
                }
            }
            return result;
        }

        private bool CompareCenters(IList<int> newCenters)
        {
            var igual = true;
            for (var i = 0; i < newCenters.Count() && igual; i++)
            {
                if (!centros[i].Equals(newCenters[i]))
                {
                    igual = false;
                }
            }
            return igual;
        }

        private void SetDefaultCenters()
        {
            var part = 1.0f / (numGrupos);
            centros = new List<int>();
            for (var i = 0; i < numGrupos; i++)
            {
                centros.Add((int)Math.Floor((i + 1) * part * 255));
            }
        }

        private void CloserCenter(Point c)
        {
            var pixel = imagem.GetPixel(c.X, c.Y);

            //int cor = alpha << 24 | red << 16 | green << 8 | blue; // verificar valor de cor caso pare no breakpoint acima
            int cor = pixel.R;
            //System.out.println(pixel);
            var menor = 256;
            var centro = 0;
            for (var i = 0; i < numGrupos; i++)
            {
                var dif = Math.Abs(cor - centros[i]);
                if (dif < menor)
                {
                    menor = dif;
                    centro = i;
                }
            }
            grupos[centro].Add(c);
        }

        private List<int> NewCenters()
        {
            var newCentros = new List<int>();

            for (var i = 0; i < numGrupos; i++)
            {
                if (grupos[i].Count > 0)
                {
                    // inserir breakpoint aqui
                    var soma = 0;
                    foreach (var coord in grupos[i])
                    {
                        var pixel = imagem.GetPixel(coord.X, coord.Y);
                        int red = pixel.R;
                        var cor = red;
                        soma += cor;
                    }
                    soma = (int)Math.Floor((decimal)soma / grupos[i].Count()); // inserir breakpoint aqui
                    newCentros.Add(soma);
                }
                else
                {
                    // inserir breakpoint aqui
                    newCentros.Add(0);
                    //cont++;
                    //string c = cont.ToString();
                    //System.Windows.Forms.MessageBox.Show("Não separou");

                    // ServerLog.AppendErrorLog("K_Means.cs - Não separou! - Linha 143", new Bitmap(0,0));
                    // System.Windows.Forms.MessageBox.Show("Não Separou!", c);
                }
            }
            return newCentros;
        }
    }
}