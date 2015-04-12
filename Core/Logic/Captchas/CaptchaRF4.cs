using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Core.Common.Extensions;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;
using Core.Logic.Utils;
using Core.Logic.RemocaoFundo;
using Core.Logic.Separacao;

namespace Core.Logic.Captchas
{
    public class CaptchaRF4 : Captcha
    {
        public CaptchaRF4(string fileName)
            : base(fileName)
        {
        }

        public CaptchaRF4(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaRF4(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 6; }
        }

        public override IEnumerable<ImgArray> GetCaracteres()
        {
            var pontosDeCorte = new List<int>() { -1, 31, 61, 90, 121, 149, this.ImgArray.Width - 1 };
            var caracteres = new List<ImgArray>();

            for (int i = 0; i < pontosDeCorte.Count - 1; i++)
            {
                ImgArray carac = new ImgArray(pontosDeCorte[i + 1] - pontosDeCorte[i], this.ImgArray.Height);

                for (int y = 0; y < this.ImgArray.Height; y++)
                {
                    for (int x = pontosDeCorte[i] + 1; x <= pontosDeCorte[i + 1]; x++)
                    {
                        carac.SetPixel(x - pontosDeCorte[i], y, this.ImgArray.GetPixel(x, y));
                    }
                }

                carac = carac.RemoverRuidos(15).CortarECentralizar(this.TamanhoImagemLetra.X, this.TamanhoImagemLetra.Y);
                //caracteres.Add(carac);

                /// **********************
                /// Código abaixo em teste 
                /// **********************
                var cfs = new ColorFillingSegmentation2(carac, 4, 20, false, false, 20);
                var clusters = cfs.GetCaracteres();
                var remove = new List<ImgArray>();

                foreach (var c in clusters)
                {
                    int altura = c.GetValidHeight();
                    int largura = c.GetValidWidth();

                    if (altura <= 7 && largura > altura * 1.5)
                    {
                        remove.Add(c);
                    }
                }

                foreach (var r in remove)
                {
                    clusters.Remove(r);
                }

                caracteres.Add(clusters.MesclarImagens().CortarECentralizar(this.TamanhoImagemLetra.X, this.TamanhoImagemLetra.Y));
                /// *********************
                /// Código acima em teste 
                /// *********************
            }

            return caracteres;
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            /// Solução 1:
            ImgArray img = new ImgArray(AdjustLevels(source, 0, 1, 2, 0, 255));

            var fd = new Tratamento.ForwardDerivative();

            img = fd.Apply(img, true, false).RemoverRuidos(15, 4);

            img = LimparBordas(img, VarrerLinhas(img), new List<int>());

            img = img.GetSegment(new Rectangle(10, 0, img.Width - 10, img.Height));

            img = img.CortarECentralizar(source.Width, source.Height);

            /// Solução 2:

            /*ImgArray img = new ImgArray(source);

            var fd = new Tratamento.ForwardDerivative();

            img = fd.Apply(img, true, true);
            img = fd.Apply(img, true, false).RemoverRuidos(40);

            img = LimparBordas(img, VarrerLinhas(img), VarrerColunas(img));*/

            /*var th = new Tratamento.ThinningZhangSuen();
            img = th.Apply(img);*/

            /// Solução 3:

            /*ImgArray img = new ImgArray(AdjustLevels(source, 0, 1, 2, 0, 255)); //.InvertColors();

            //Erosion er = new Erosion(img);
            //img = er.Result.InvertColors();

            var fd = new Tratamento.ForwardDerivative();
            img = fd.Apply(img, true, true).RemoverRuidos(6, 4);
            //img = fd.Apply(img, true, false);

            img = LimparBordas(img, VarrerLinhas(img), VarrerColunas(img));*/

            return img.ToBitmap();
        }

        /// <summary>
        /// Altera os níveis de cor da imagem
        /// </summary>
        /// <param name="img"></param>
        /// <param name="shadows">0 - 253</param>
        /// <param name="midtones">-255 - 255</param>
        /// <param name="highlights">2 - 255</param>
        /// <param name="outputLowLevel">0 - 255</param>
        /// <param name="outputHighLevel">0 - 255</param>
        /// <returns></returns>
        private Bitmap AdjustLevels(Bitmap img, byte shadows, float midtones, byte highlights, byte outputLowLevel, byte outputHighLevel)
        {
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color c = img.GetPixel(x, y);
                    byte rgb = (byte)((c.R + c.G + c.B) / 3);
                    byte leveledColor;

                    if (rgb <= shadows)
                    {
                        leveledColor = outputLowLevel;
                    }
                    else
                    {
                        if (rgb >= highlights)
                        {
                            leveledColor = outputHighLevel;
                        }
                        else
                        {
                            byte adjustment = (byte)(rgb + midtones);
                            leveledColor = (byte)(adjustment <= shadows ? shadows + 1 : (adjustment >= highlights ? highlights - 1 : adjustment));
                        }
                    }

                    img.SetPixel(x, y, Color.FromArgb(leveledColor, leveledColor, leveledColor));
                }
            }

            return img;
        }

        /// <summary>
        /// Retorna uma lista com o número de pixels pretos em cada linha
        /// </summary>
        /// <returns></returns>
        private List<int> VarrerLinhas(ImgArray img)
        {
            var contagemTotal = new List<int>(img.Height);
            int contagemAtual = 0;

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    if (img.GetPixel(x, y).IsBlackPixel())
                    {
                        contagemAtual++;
                    }
                }

                contagemTotal.Add(contagemAtual);
                contagemAtual = 0;
            }

            return contagemTotal;
        }

        /// <summary>
        /// Retorna uma lista com o número de pixels pretos em cada coluna
        /// </summary>
        /// <returns></returns>
        private List<int> VarrerColunas(ImgArray img)
        {
            var contagemTotal = new List<int>(img.Width);
            int contagemAtual = 0;

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    if (img.GetPixel(x, y).IsBlackPixel())
                    {
                        contagemAtual++;
                    }
                }

                contagemTotal.Add(contagemAtual);
                contagemAtual = 0;
            }

            return contagemTotal;
        }

        private ImgArray LimparBordas(ImgArray img, List<int> contagemDePixelsLinhas, List<int> contagemDePixelsColunas, int limiarLinha = 5, int limiarColuna = 5)
        {
            int sequenciaInvalida = 0;
            int posXTopo = 0;

            for (int linha = img.Height / 2; linha >= 0; linha--)
            {
                if (contagemDePixelsLinhas[linha] < limiarLinha)
                {
                    sequenciaInvalida++;
                }
                else
                {
                    sequenciaInvalida = 0;
                }

                if (sequenciaInvalida == 2)
                {
                    posXTopo = linha + 2;
                    break;
                }
            }

            sequenciaInvalida = 0;
            int posXBase = img.Height - 1;

            for (int linha = img.Height / 2; linha < img.Height; linha++)
            {
                if (contagemDePixelsLinhas[linha] < limiarLinha)
                {
                    sequenciaInvalida++;
                }
                else
                {
                    sequenciaInvalida = 0;
                }

                if (sequenciaInvalida == 2)
                {
                    posXBase = linha - 1;
                    break;
                }
            }

            /*sequenciaInvalida = 0;
            int posYEsquerda = 0;

            for (int coluna = img.Width / 2; coluna >= 0; coluna--)
            {
                if (contagemDePixelsColunas[coluna] < limiarLinha)
                {
                    sequenciaInvalida++;
                }
                else
                {
                    sequenciaInvalida = 0;
                }

                if (sequenciaInvalida == 2)
                {
                    posYEsquerda = coluna;
                    break;
                }
            }

            sequenciaInvalida = 0;
            int posYDireita = img.Width - 1;

            for (int coluna = img.Width / 2; coluna <= img.Width; coluna++)
            {
                if (contagemDePixelsColunas[coluna] < limiarLinha)
                {
                    sequenciaInvalida++;
                }
                else
                {
                    sequenciaInvalida = 0;
                }

                if (sequenciaInvalida == 2)
                {
                    posYDireita = coluna;
                    break;
                }
            }*/

            return img.GetSegment(new Rectangle(0, posXTopo, img.Width, posXBase - posXTopo));
        }



    }
}