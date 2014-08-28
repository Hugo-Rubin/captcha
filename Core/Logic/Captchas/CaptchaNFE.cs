using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Filtros;
using Core.Logic.Tratamento;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Captchas
{
    public class CaptchaNFE : Captcha
    {
        public CaptchaNFE(String fileName)
            : base(fileName)
        {
        }

        public CaptchaNFE(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaNFE(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 4; }
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            var grayScale = source.Clone(new Rectangle(0, 0, source.Width, source.Height),
                                            PixelFormat.Format32bppArgb);
            var g = Graphics.FromImage(grayScale);
            g.Clear(Color.White);
            //Define pixel preto para pintar em graphics
            var pt = new Bitmap(1, 1);
            pt.SetPixel(0, 0, Color.Black);
            var erro = false;

            for (var y = 0; y < grayScale.Height; y++)
            {
                for (var x = 0; x < grayScale.Width; x++)
                {
                    if (erro)
                    {
                        break;
                    }
                    if (source.GetPixel(x, y).IsBlackPixel())
                    {
                        try
                        {
                            g.DrawImageUnscaled(pt, x, y);
                        }
                        catch
                        {
                            erro = true;
                        }
                    }
                }
            }
            return grayScale;
        }

        //TODO: Revisar esse método!
        private void PreencherPixels(int amount = 1)
        {
            Color right, up, down;
            var left = right = up = down = ImgArray.GetPixel(0, 0);
            var preto = Color.Black;

            bool test2, sentido;
            var test1 = test2 = sentido = false;
            var count = 0;

            try
            {
                do
                {
                    for (var y = 0; y < ImgArray.Height; y++)
                    {
                        for (var x = 0; x < ImgArray.Width; x++)
                        {
                            var c = ImgArray.GetPixel(x, y);

                            if (x > 0 && x < ImgArray.Width - 1)
                            {
                                left = ImgArray.GetPixel(x - 1, y);
                                right = ImgArray.GetPixel(x + 1, y);
                                test1 = true;
                            }
                            if (y > 0 && y < ImgArray.Height - 1)
                            {
                                up = ImgArray.GetPixel(x, y + 1);
                                down = ImgArray.GetPixel(x, y - 1);
                                test2 = true;
                            }

                            if (c.IsBlackPixel())
                            {
                                if (test2)
                                {
                                    if (!up.IsBlackPixel()
                                        && ImgArray.GetPixel(x, y + 2).IsBlackPixel()
                                        && !ImgArray.GetPixel(x, y + 4).IsBlackPixel())
                                    {
                                        ImgArray.SetPixel(x, y + 1, preto);
                                    }

                                    if (y > 1 && !down.IsBlackPixel()
                                        && ImgArray.GetPixel(x, y - 2).IsBlackPixel())
                                    {
                                        ImgArray.SetPixel(x, y - 1, preto);
                                    }
                                }

                                if (test1)
                                {
                                    if (x > 1 && !left.IsBlackPixel() && ImgArray.GetPixel(x - 2, y).IsBlackPixel())
                                        ImgArray.SetPixel(x - 1, y, preto);
                                    if (!right.IsBlackPixel() && ImgArray.GetPixel(x + 2, y).IsBlackPixel())
                                        ImgArray.SetPixel(x + 1, y, preto);
                                }
                            }
                        }
                    }

                    for (var y = 0; y < ImgArray.Height - 1; y++)
                    {
                        for (var x = 0; x < ImgArray.Width - 1; x++)
                        {
                            var c = ImgArray.GetPixel(x, y);

                            if (x > 0 && x < ImgArray.Width - 1)
                            {
                                left = ImgArray.GetPixel(x - 1, y);
                                right = ImgArray.GetPixel(x + 1, y);
                                test1 = true;
                            }

                            if (y > 0 && y < ImgArray.Height - 1)
                            {
                                up = ImgArray.GetPixel(x, y + 1);
                                down = ImgArray.GetPixel(x, y - 1);
                                test2 = true;
                            }

                            if (c.IsBlackPixel())
                            {
                                if (test2 && sentido)
                                {
                                    if (!down.IsBlackPixel() && up.IsBlackPixel())
                                    {
                                        ImgArray.SetPixel(x, y - 1, preto);
                                    }

                                    sentido = false;
                                }
                                if (test1 && sentido == false)
                                {
                                    if (!left.IsBlackPixel() && right.IsBlackPixel())
                                    {
                                        ImgArray.SetPixel(x - 1, y, preto);
                                    }

                                    sentido = true;
                                }
                            }
                        }
                    }
                    count++;
                } while (count < amount);
            }
            catch (Exception e)
            {
                ServerLog.AppendErrorLog(e.Message, ImgArray.ToBitmap());
            }
        }

        private String DescreverPadrao(IEnumerable<TipoPadrao> padrao)
        {
            var result = String.Empty;
            var delimiter = "";
            foreach (var item in padrao)
            {
                result += delimiter + item;
                delimiter = ", ";
            }
            return result;
        }

        private ImgArray TratarImagem()
        {
            var preenchido = false;
            var id = new ReconhecimentoDePadraoNFE();
            var padrao = id.GetPadrao(ImgArray);
            PadraoIdentificado = DescreverPadrao(padrao);

            if (padrao.Contains(TipoPadrao.Distorcida))
            {
                DistorcaoInversa();
            }

            if (padrao.Contains(TipoPadrao.Bandeira))
            {
                if (padrao.Contains(TipoPadrao.Segmentada))
                {
                    //PreencherPixels(2);   
                    preenchido = true;
                    var gb = new GaussianBlur(1);
                    var blur = gb.ProcessImage(ImgArray.ToBitmap());
                    var bmp = blur.PaintBlur();
                    LoadFromBitmap(bmp);
                }
                var e = new Endireitamento(true, true);
                var imgArray = e.ApplyTo(ImgArray);
                LoadFromNanoArray(imgArray.ToNanoArray());
            }

            /* int intensidade = 1;
             if (!preenchido && Padrao.Contains(TipoPadrao.Segmentada))
             {
                 //apenas segmentada
                 intensidade = 3;
             }
             PreencherPixels(intensidade);*/
            var intensidade = 1;
            if (padrao.Contains(TipoPadrao.Segmentada))
            {
                intensidade = 3;
            }
            if (!preenchido)
            {
                PreencherPixels(intensidade);
            }
            return ImgArray;
        }

        /// <summary>
        ///   Distorção inversa vetorizada
        /// </summary>
        private void DistorcaoInversa()
        {
            var meanC = ImgArray.Width / 2;
            var meanR = ImgArray.Height / 2;
            const double stdevR = 35;
            const double stdevC = 35;

            var imgIn = ImgArray.Clone();
            ImgArray.Clear();

            for (var y = 0; y < imgIn.Height; y++)
            {
                for (var x = 0; x < imgIn.Width; x++)
                {
                    var scale = Gauss.Calculate(meanR, stdevR, y) * Gauss.Calculate(meanC, stdevC, x);
                    var newY = y + (meanR - y) * scale;
                    var newX = x + (meanC - x) * scale;

                    var newXf = (int)Math.Floor(newX);
                    var newYf = (int)Math.Floor(newY);

                    var newYc = (int)Math.Ceiling(newX); // or simply newRowF+1
                    var newXc = (int)Math.Ceiling(newY); // or simply newColF+1

                    var c = imgIn.GetPixel(x, y);

                    ImgArray.SetPixel(newXf, newYf, c);
                    ImgArray.SetPixel(newYc, newXc, c);
                }
            }
        }

        public override ImgArray[] GetCaracteresImgArray()
        {
            var bmpTratado = TratarImagem();
            var separar = new SeparacaoPadrao(this);
            return
                separar.ColorFillingSegmentation2AndSeamCarving2(bmpTratado).CortarECentralizarTodos(
                    TamanhoImagemLetra.X, TamanhoImagemLetra.Y);
        }
    }
}