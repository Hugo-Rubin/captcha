using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using Core.Common;
using Core.Common.Extensions;
using Core.Logic.Captchas;
using Core.Logic.Predict;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic
{
    public static class ServerUtil
    {
        private const string VImg = @"C:\Users\Hugo\file.txt";

        public static string ResourcesDir
        {
            get { return CustomConfigurationManager.ReadAppSetting("ResourcesDir") ?? AbsolutePath + @"resources\"; }
        }

        public static string LogDir
        {
            get { return CustomConfigurationManager.ReadAppSetting("LogDir") ?? AbsolutePath + @"log\"; }
        }

        public static string AbsolutePath
        {
            get
            {
                //TODO: Achar uma alternativa pra nao ter que colocar path hardcoded
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }


        public static void ValidatePath(string d)
        {
            if (!File.Exists(VImg))
                File.CreateText(VImg);

            if (!Directory.Exists(d))
                Directory.CreateDirectory(d);
        }

        public static bool IsBlackPixel(this Color pixel)
        {
            return ((pixel.A == 255) && (pixel.R == 0) && (pixel.G == 0) && (pixel.B == 0));
        }

        public static bool IsWhitePixel(this Color color)
        {
            return color.A == 0 || color.ToArgb() == Color.White.ToArgb();
        }
        
        public static void CopiarBitmapPara(this Bitmap src, ref Bitmap dest)
        {
            // TODO: Tratar exceções para caso de src menor que dest
            for (var x = 0; x < dest.Width; x++)
            {
                for (var y = 0; y < dest.Height; y++)
                {
                    dest.SetPixel(x, y, src.GetPixel(x, y));
                }
            }
        }

        public static void Init<T>(this T[] array, T defaultVaue)
        {
            if (array == null)
                return;
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = defaultVaue;
            }
        }

        //public static int GetMinX(this Bitmap bmp)
        //{
        //    int x1 = -1;
        //    for (int x = 0; x < bmp.Width; x++)
        //    {
        //        for (int y = 0; y < bmp.Height; y++)
        //        {
        //            Color c = bmp.GetPixel(x, y);
        //            if (c.IsBlackPixel())
        //            {
        //                x1 = x;
        //                break;
        //            }
        //        }
        //        if (x1 > -1)
        //        {
        //            break;
        //        }
        //    }
        //    return x1;
        //}
        ///// <summary>
        ///// Varre o bitmap e retorna o X do pixel preto mais próximo da
        ///// lateral direita da imagem
        ///// </summary>
        ///// <param name="bmp"></param>
        ///// <returns></returns>
        //public static int GetMaxX(this Bitmap bmp)
        //{
        //    int x2 = -1;
        //    for (int x = bmp.Width - 1; x > 0; x--)
        //    {
        //        for (int y = 0; y < bmp.Height; y++)
        //        {
        //            Color c = bmp.GetPixel(x, y);
        //            if (c.IsBlackPixel())
        //            {
        //                x2 = x;
        //                break;
        //            }
        //        }
        //        if (x2 > -1)
        //        {
        //            break;
        //        }
        //    }
        //    return x2;
        //}
        ///// <summary>
        ///// Varre o bitmap e retorna o Y do pixel preto mais próximo da
        ///// parte superior da imagem
        ///// </summary>
        ///// <param name="bmp"></param>
        ///// <returns></returns>
        //public static int GetMinY(this Bitmap bmp)
        //{
        //    int y1 = -1;
        //    for (int y = 0; y < bmp.Height; y++)
        //    {
        //        for (int x = 0; x < bmp.Width; x++)
        //        {
        //            Color c = bmp.GetPixel(x, y);
        //            if (c.IsBlackPixel())
        //            {
        //                y1 = y;
        //                break;
        //            }
        //        }
        //        if (y1 > -1)
        //        {
        //            break;
        //        }
        //    }
        //    return y1;
        //}
        ///// <summary>
        ///// Varre o bitmap e retorna o Y do pixel preto mais próximo da
        ///// parte inferior da imagem
        ///// </summary>
        ///// <param name="bmp"></param>
        ///// <returns></returns>
        //public static int GetMaxY(this Bitmap bmp)
        //{
        //    int y2 = -1;
        //    for (int y = bmp.Height - 1; y > 0; y--)
        //    {
        //        for (int x = 0; x < bmp.Width; x++)
        //        {
        //            Color c = bmp.GetPixel(x, y);
        //            if (c.IsBlackPixel())
        //            {
        //                y2 = y;
        //                break;
        //            }
        //        }
        //        if (y2 > -1)
        //        {
        //            break;
        //        }
        //    }
        //    return y2;
        //}
        /// <summary>
        ///   Transforma Matriz PixelIntensity em Bitmap preto/branco
        /// </summary>
        /// <returns> </returns>
        /// <summary>
        ///   Varre o bitmap e retorna o X do pixel preto mais próximo da
        ///   lateral esquerda da imagem
        /// </summary>
        /// <param name="bmp">
        /// </param>
        /// <returns> </returns>
        /// <summary>
        ///   Varre o bitmap e retorna o X do pixel não-preto mais próximo da
        ///   lateral esquerda da imagem
        /// </summary>
        /// <returns> </returns>
        public static int GetMinXNotBlack(this Bitmap bmp)
        {
            var x1 = -1;
            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var c = bmp.GetPixel(x, y);
                    if (!c.IsBlackPixel())
                    {
                        x1 = x;
                        break;
                    }
                }
                if (x1 > -1)
                {
                    break;
                }
            }
            return x1;
        }

        /// <summary>
        ///   Varre o bitmap e retorna o X do pixel não-preto mais próximo da
        ///   lateral direita da imagem
        /// </summary>
        /// <param name="bmp"> </param>
        /// <returns> </returns>
        public static int GetMaxXNotBlack(this Bitmap bmp)
        {
            var x2 = -1;
            for (var x = bmp.Width - 1; x > 0; x--)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var c = bmp.GetPixel(x, y);
                    if (!c.IsBlackPixel())
                    {
                        x2 = x;
                        break;
                    }
                }
                if (x2 > -1)
                {
                    break;
                }
            }
            return x2;
        }

        /// <summary>
        ///   Varre o bitmap e retorna o Y do pixel não-preto mais próximo da
        ///   parte superior da imagem
        /// </summary>
        /// <param name="bmp"> </param>
        /// <returns> </returns>
        public static int GetMinYNotBlack(this Bitmap bmp)
        {
            var y1 = -1;
            for (var y = 0; y < bmp.Height; y++)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    var c = bmp.GetPixel(x, y);
                    if (!c.IsBlackPixel())
                    {
                        y1 = y;
                        break;
                    }
                }
                if (y1 > -1)
                {
                    break;
                }
            }
            return y1;
        }

        /// <summary>
        ///   Varre o bitmap e retorna o Y do pixel não-preto mais próximo da
        ///   parte inferior da imagem
        /// </summary>
        /// <param name="bmp"> </param>
        /// <returns> </returns>
        public static int GetMaxYNotBlack(this Bitmap bmp)
        {
            var y2 = -1;
            for (var y = bmp.Height - 1; y > 0; y--)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    var c = bmp.GetPixel(x, y);
                    if (!c.IsBlackPixel())
                    {
                        y2 = y;
                        break;
                    }
                }
                if (y2 > -1)
                {
                    break;
                }
            }
            return y2;
        }

        /// <summary>
        ///   Recorta a imagem a partir do tamanho e posição do Rectangle
        /// </summary>
        /// <param name="srcBitmap"> </param>
        /// <param name="section"> </param>
        /// <returns> </returns>
        public static Bitmap CropRectangle(this Bitmap srcBitmap, Rectangle section)
        {
            var result = new Bitmap(section.Width, section.Height);
            var g = Graphics.FromImage(result);
            g.Clear(Color.White);
            g.DrawImage(srcBitmap, 0, 0, section, GraphicsUnit.Pixel);
            g.Dispose();
            return result;
        }

        [Obsolete("Usar ImgArray")]
        public static Bitmap CortarECentralizar(this Bitmap srcBmp, int newWidth, int newHeight)
        {
            if (srcBmp == null)
            {
                return new Bitmap(newWidth, newHeight).InserirFundoBranco();
            }

            var x1 = srcBmp.GetMinX();
            var x2 = srcBmp.GetMaxX();
            var y1 = srcBmp.GetMinY();
            var y2 = srcBmp.GetMaxY();

            var xMedio = (newWidth - (x2 - x1)) / 2;
            var yMedio = (newHeight - (y2 - y1)) / 2;

            if (newWidth > 0
                && newHeight > 0
                && x1 > -1
                && x2 > -1
                && y1 > -1
                && y2 > -1)
            {
                var rec = new Rectangle(x1 - xMedio, y1 - yMedio, newWidth, newHeight);
                return srcBmp.CropRectangle(rec);
            }

            return new Bitmap(newWidth, newHeight).InserirFundoBranco();
        }

        public static ImgArray CortarECentralizar(this ImgArray srcBmp, int newWidth, int newHeight)
        {
            if (srcBmp == null)
            {
                return new ImgArray(newWidth, newHeight);
            }

            var bmp = srcBmp.ToBitmap();

            var x1 = bmp.GetMinX();
            var x2 = bmp.GetMaxX();
            var y1 = bmp.GetMinY();
            var y2 = bmp.GetMaxY();

            var xMedio = (newWidth - (x2 - x1)) / 2;
            var yMedio = (newHeight - (y2 - y1)) / 2;

            if (newWidth > 0
                && newHeight > 0
                && x1 > -1
                && x2 > -1
                && y1 > -1
                && y2 > -1)
            {
                var rectangle = new Rectangle(x1 - xMedio, y1 - yMedio, newWidth, newHeight);
                return new ImgArray(CropRectangle(bmp, rectangle));
            }
            return new ImgArray(newWidth, newHeight);
        }

        [Obsolete("Usar ImgArray")]
        public static Bitmap RemoveWhiteBorders(this Bitmap srcBmp, int margemX = 0, int margemY = 0)
        {
            if (srcBmp == null)
            {
                return new Bitmap(1, 1);
            }

            var x1 = srcBmp.GetMinX();
            var x2 = srcBmp.GetMaxX();
            var y1 = srcBmp.GetMinY();
            var y2 = srcBmp.GetMaxY();

            if (x1 < 0 || x2 < 0 || y1 < 0 || y2 < 0)
            {
                return srcBmp;
            }

            return srcBmp.CortarECentralizar(x2 - x1 + 1 + 2 * margemX, y2 - y1 + 1 + 2 * margemY);

            // TODO: Foi utilizado ImgArrayLite para melhorar o desempenho. Converta tudo para ImgArrayLite

            /*ImgArrayLite img = new ImgArrayLite(srcBmp);

            if (crop)
            {
                int x1 = srcBmp.GetMinX();
                int x2 = srcBmp.GetMaxX();
                int y1 = srcBmp.GetMinY();
                int y2 = srcBmp.GetMaxY();

                if (x1 < 0 || x2 < 0 || y1 < 0 || y2 < 0)
                {
                    return srcBmp;
                }
                try
                {
                    unsafe
                    {
                        BitmapData bitdataOriginal = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                        int PixelSize = 4;

                        byte* checkY1 = (byte*)bitdataOriginal.Scan0 + (y1 * bitdataOriginal.Stride);
                        byte* nextY1 = (byte*)bitdataOriginal.Scan0 + ((y1 + 1) * bitdataOriginal.Stride);
                        byte* checkY2 = (byte*)bitdataOriginal.Scan0 + (y2 * bitdataOriginal.Stride);
                        byte* nextY2 = (byte*)bitdataOriginal.Scan0 + ((y2 - 1) * bitdataOriginal.Stride);

                        int countY1 = 0, countY2 = 0, countNextY1 = 0, countNextY2 = 0;

                        for (int x = 0; x < bitdataOriginal.Width; x++)
                        {
                            // TODO: Verificar pq imagens com letras coladas estao gerando excecao aqui
                            if (checkY1[x * PixelSize] == 0)
                            {
                                countY1++;
                            }
                            if (checkY2[x * PixelSize] == 0)
                            {
                                countY2++;
                            }
                            if (nextY1[x * PixelSize] == 0)
                            {
                                countNextY1++;
                            }
                            if (nextY2[x * PixelSize] == 0)
                            {
                                countNextY2++;
                            }
                        }

                        int countX1 = 0, countX2 = 0, countNextX1 = 0, countNextX2 = 0;

                        for (int y = 0; y < bitdataOriginal.Height; y++)
                        {
                            byte* checkX = (byte*)bitdataOriginal.Scan0 + (y * bitdataOriginal.Stride);

                            if (checkX[0] == 0)
                            {
                                countX1++;
                            }
                            if (checkX[PixelSize] == 0)
                            {
                                countNextX1++;
                            }
                            if (checkX[(bitdataOriginal.Width - 1) * PixelSize] == 0)
                            {
                                countX2++;
                            }
                            if (checkX[(bitdataOriginal.Width - 2) * PixelSize] == 0)
                            {
                                countNextX2++;
                            }
                        }

                        srcBmp.UnlockBits(bitdataOriginal);

                        int xInit = x1, yInit = y1, xEnd = x2, yEnd = y2;

                        if (countY1 <= 2)
                        {
                            if (countNextY1 > 3)
                            {
                                yInit++;
                            }
                        }
                        else
                        {
                            if (countY1 == 3 && countNextY1 > 6)
                            {
                                yInit++;
                            }
                        }

                        if (countY2 <= 2)
                        {
                            if (countNextY2 > 3)
                            {
                                yEnd--;
                            }
                        }
                        else
                        {
                            if (countY2 == 3 && countNextY2 > 6)
                            {
                                yEnd--;
                            }
                        }

                        if (countX1 <= 2)
                        {
                            if (countNextX1 > 3)
                            {
                                xInit++;
                            }
                        }
                        else
                        {
                            if (countX1 == 3 && countNextX1 > 6)
                            {
                                xInit++;
                            }
                        }

                        if (countX2 <= 2)
                        {
                            if (countNextX2 > 3)
                            {
                                xEnd--;
                            }
                        }
                        else
                        {
                            if (countX2 == 3 && countNextX2 > 6)
                            {
                                xEnd--;
                            }
                        }

                        Rectangle recCrop = new Rectangle(xInit, yInit, xEnd - xInit + 1, yEnd - yInit + 1);
                        return srcBmp.CropRectangle(recCrop);
                    }
                    Rectangle rec = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
                    return srcBmp.CropRectangle(rec);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                int x1 = srcBmp.GetMinX();
                int x2 = srcBmp.GetMaxX();

                Rectangle rec = new Rectangle(x1 - margemX, 0, x2 - x1 + 1 + (2 * margemX), srcBmp.Height + margemY);
                return srcBmp.CropRectangle(rec);
            }*/
        }

        [Obsolete("Usar ImgArray")]
        public static Bitmap InserirBordaX(this Bitmap src, int tamBorda = 3)
        {
            var result = new Bitmap(src.Width + 2 * tamBorda, src.Height).InserirFundoBranco();
            var g = Graphics.FromImage(result);
            g.DrawImage(src, tamBorda, 0);
            return result;
        }

        public static ImgArray InserirBordaX(this ImgArray src, int tamBorda = 3)
        {
            var result = new Bitmap(src.Width + 2 * tamBorda, src.Height).InserirFundoBranco();
            var g = Graphics.FromImage(result);
            g.DrawImage(src.ToBitmap(), tamBorda, 0);
            return new ImgArray(result);
        }

        [Obsolete("Usar ImgArray")]
        public static Bitmap InserirBordaY(this Bitmap src, int tamBorda = 3)
        {
            var result = new Bitmap(src.Width, src.Height + 2 * tamBorda).InserirFundoBranco();
            var g = Graphics.FromImage(result);
            g.DrawImage(src, 0, tamBorda);
            return result;
        }

        public static ImgArray InserirBordaY(this ImgArray src, int tamBorda = 3)
        {
            var result = new Bitmap(src.Width, src.Height + 2 * tamBorda).InserirFundoBranco();
            var g = Graphics.FromImage(result);
            g.DrawImage(src.ToBitmap(), 0, tamBorda);
            return new ImgArray(result);
        }

        /// <summary>
        ///   Remove bordas pretas na imagem de fundo preto com pixels brancos
        /// </summary>
        /// <param name="srcBmp"> </param>
        /// <param name="crop"></param>
        /// <returns> </returns>
        public static Bitmap RemoveBlackBorders(this Bitmap srcBmp, bool crop = false)
        {
            if (srcBmp == null)
            {
                return new Bitmap(1, 1);
            }

            var x1 = srcBmp.GetMinXNotBlack();
            var x2 = srcBmp.GetMaxXNotBlack();
            var y1 = srcBmp.GetMinYNotBlack();
            var y2 = srcBmp.GetMaxYNotBlack();

            if (crop)
            {
                unsafe
                {
                    var bitdataOriginal = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height),
                                                                 ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    const int pixelSize = 4;

                    var checkY1 = (byte*)bitdataOriginal.Scan0 + (y1 * bitdataOriginal.Stride);
                    var nextY1 = (byte*)bitdataOriginal.Scan0 + ((y1 + 1) * bitdataOriginal.Stride);
                    var checkY2 = (byte*)bitdataOriginal.Scan0 + (y2 * bitdataOriginal.Stride);
                    var nextY2 = (byte*)bitdataOriginal.Scan0 + ((y2 - 1) * bitdataOriginal.Stride);

                    int countY1 = 0, countY2 = 0, countNextY1 = 0, countNextY2 = 0;

                    for (var x = 0; x < bitdataOriginal.Width; x++)
                    {
                        if (checkY1[x * pixelSize] != 0)
                        {
                            countY1++;
                        }
                        if (checkY2[x * pixelSize] != 0)
                        {
                            countY2++;
                        }
                        if (nextY1[x * pixelSize] != 0)
                        {
                            countNextY1++;
                        }
                        if (nextY2[x * pixelSize] != 0)
                        {
                            countNextY2++;
                        }
                    }

                    int countX1 = 0, countX2 = 0, countNextX1 = 0, countNextX2 = 0;

                    for (var y = 0; y < bitdataOriginal.Height; y++)
                    {
                        var checkX = (byte*)bitdataOriginal.Scan0 + (y * bitdataOriginal.Stride);

                        if (checkX[0] != 0)
                        {
                            countX1++;
                        }
                        if (checkX[pixelSize] != 0)
                        {
                            countNextX1++;
                        }
                        if (checkX[(bitdataOriginal.Width - 1) * pixelSize] != 0)
                        {
                            countX2++;
                        }
                        if (checkX[(bitdataOriginal.Width - 2) * pixelSize] != 0)
                        {
                            countNextX2++;
                        }
                    }

                    srcBmp.UnlockBits(bitdataOriginal);

                    int xInit = x1, yInit = y1, xEnd = x2, yEnd = y2;

                    if (countY1 <= 2)
                    {
                        if (countNextY1 > 3)
                        {
                            yInit++;
                        }
                    }
                    else
                    {
                        if (countY1 == 3)
                        {
                            if (countNextY1 > 6)
                            {
                                yInit++;
                            }
                        }
                    }

                    if (countY2 <= 2)
                    {
                        if (countNextY2 > 3)
                        {
                            yEnd--;
                        }
                    }
                    else
                    {
                        if (countY2 == 3)
                        {
                            if (countNextY2 > 6)
                            {
                                yEnd--;
                            }
                        }
                    }

                    if (countX1 <= 2)
                    {
                        if (countNextX1 > 3)
                        {
                            xInit++;
                        }
                    }
                    else
                    {
                        if (countX1 == 3)
                        {
                            if (countNextX1 > 6)
                            {
                                xInit++;
                            }
                        }
                    }

                    if (countX2 <= 2)
                    {
                        if (countNextX2 > 3)
                        {
                            xEnd--;
                        }
                    }
                    else
                    {
                        if (countX2 == 3)
                        {
                            if (countNextX2 > 6)
                            {
                                xEnd--;
                            }
                        }
                    }
                    var recCrop = new Rectangle(xInit, yInit, xEnd - xInit + 1, yEnd - yInit + 1);
                    return srcBmp.CropRectangle(recCrop);
                }
            }

            var rec = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
            return srcBmp.CropRectangle(rec);
        }

        /// <summary>
        ///   Troca pixel transparente por pixel branco
        /// </summary>
        /// <param name="bmp"> </param>
        /// <returns> </returns>
        public static Bitmap InserirFundoBranco(this Bitmap bmp)
        {
            // Usar graphics é infinitamente mais rápido que varrer a imagem dando setpixel
            var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            g.Dispose();
            return bmp;
        }

        /// <summary>
        ///   Pinta a imagem borrada pelo gaussian blur, preparando-a para o reconhecimento
        /// </summary>
        /// <param name="blurredImg"> Imagem passada pelo Gaussian Blur </param>
        /// <returns> </returns>
        public static Bitmap PaintBlur(this Bitmap blurredImg)
        {
            for (var i = 0; i < blurredImg.Width; i++)
            {
                for (var j = 0; j < blurredImg.Height; j++)
                {
                    if (blurredImg.GetPixel(i, j).R < 248)
                        blurredImg.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                    else if (blurredImg.GetPixel(i, j).R != 255)
                        blurredImg.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                }
            }

            return blurredImg;
        }

        /// <summary>
        ///   Pinta a imagem borrada pelo gaussian blur, preparando-a para o reconhecimento
        /// </summary>
        /// <param name="blurredImg"> Imagem passada pelo Gaussian Blur </param>
        /// <returns> </returns>
        public static ImgArray PaintBlur(this ImgArray blurredImg)
        {
            for (var i = 0; i < blurredImg.Width; i++)
            {
                for (var j = 0; j < blurredImg.Height; j++)
                {
                    if (blurredImg.GetPixel(i, j).R < 248)
                        blurredImg.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                    else if (blurredImg.GetPixel(i, j).R != 255)
                        blurredImg.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                }
            }

            return blurredImg;
        }

        /// <summary>
        ///   Varre uma pasta trocando o nome dos arquivos (em a-Z) para seu código de acordo com nosso dicionário
        /// </summary>
        /// <returns> </returns>
        public static void TraduzirNome()
        {
            var di = new DirectoryInfo(@"D:\Arquivos e Pastas\Captcha\Training Set\Corrigidas 2\Predicted");
            var imagens = di.GetFiles("*.png");
            var r = new Random();
            var count = 0;

            char[] dicio =
                {
                    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', '1', 'm',
                    'n', '2', 'p', 'q', 'r', 's', 't', 'u', 'w', 'v', 'x', 'y', 'z',
                    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', '3', 'J', 'K', 'L', 'M',
                    'N', '4', 'P', 'Q', 'R', 'S', 'T', 'U', 'W', 'V', 'X', '~', 'Y', 'Z',
                    '5', '6', '7', '8', '9'
                };

            foreach (var i in imagens)
            {
                var nome = i.Name.ToCharArray();
                var idx = Array.IndexOf(dicio, nome[0]);
                var newName = i.DirectoryName + "\\" + idx + " (" + count + ").png";

                if (File.Exists(newName))
                {
                    var comp = r.Next(1000, 10000);
                    var sub = count.ToString(CultureInfo.InvariantCulture).Length + 5;
                    newName = newName.Substring(0, newName.Length - sub) + comp + ").png";
                }

                i.MoveTo(newName);
                count++;
            }
        }

        public static double[,] ScalarToArray(double[] y, int dataset)
        {
            var newY = new double[dataset, Convert.ToInt32(y.Max() + 1)];
            for (var i = 0; i < dataset; i++)
            {
                newY[i, Convert.ToInt32(y[i])] = 1;
            }
            return newY;
        }

        public static DateTime ToHorarioBrasileiro(this DateTime dt)
        {
            var dt2 = dt.ToUniversalTime();
            DateTime result;
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Central Brazilian Standard Time");
                result = TimeZoneInfo.ConvertTimeFromUtc(dt, tz);
            }
            catch (Exception)
            {
                result = dt2.AddHours(-3);
            }
            return result;
        }

        //TODO: Converter para vetor
        public static void SalvarVetorDePixels()
        {
            //DirectoryInfo di = new DirectoryInfo(@"C:\Users\Hugo\Captcha TEMP\TEST");
            //DirectoryInfo di = new DirectoryInfo(@"C:\Users\Hugo\Captcha TEMP\TEST Padrão\Renomeadas Para RN de Identificação\Blurred");

            var dicio = new[]
                            {
                                '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd',
                                'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'p', 'q', 'r',
                                's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'D', 'E',
                                'F', 'G', 'H', 'I', 'J', 'M', 'N', 'P', 'Q', 'R', 'T', 'U', 'Y'
                            };


            var di = new DirectoryInfo(@"D:\Backup\Todas\Predicted");

            ValidatePath(di.FullName);

            var di2 = di.GetDirectories();
            var s = new StringBuilder();

            var vImg = di.FullName + @"\y_Sintegra-SP.txt";

            foreach (var folder in from folder in di2 where folder.Name[0] != '_' let imagens = folder.GetFiles("*.png") from i in imagens select folder)
            {
                /*double[,] m = new double[1, 3600]; //[1, 3600] para imagens 60 x 60, [1, 18000] para imagens 200 x 90
                    Image img = Image.FromFile(i.FullName);
                    Bitmap bmp = (Bitmap)img;
                    int k = 0;
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            c = bmp.GetPixel(x, y);
                            //luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                            //m[0, k] = luminance / 255;
                            m[0, k] = c.R > 127 ? 1 : 0;
                            s.Append(string.Format("{0};", m[0, k]));
                            k++;*/

                /*c = bmp.GetPixel(x, y);
                    luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    m[0, k] = luminance / 255;
                    //m[0, x + (y * img.Width)] = luminance / 255;
                    s.Append(string.Format("{0};", m[0, k]));
                    k++;*/
                //}
                //}
                //int[] p = Predict.predict(m);
                //limp = i.Name.ToCharArray();
                //nome = limp[0].ToString() + limp[1].ToString();
                //nome = nome.Trim();
                s.Append(Array.IndexOf(dicio, folder.Name[0]) + ";\n");
                //s.Append(string.Format("{0};\n", nome));
                //s.Append(string.Format("{0};\n", p[0]));
                //count++;
            }

            var sw = new StreamWriter(vImg);
            sw.Write(s.ToString());
            sw.Close();
            sw.Dispose();
        }


        //TODO: Converter para vetor
        public static void SalvarVetorDePixelsCaptchaSisC()
        {
            var parent = new DirectoryInfo(@"C:\Users\Hugo\kabuto"); // diretório pai
            ValidatePath(parent.FullName);
            var subdirectories = parent.GetDirectories();

            foreach (var dir in subdirectories)
            {
                var s = new StringBuilder();
                //int luminance;

                //DirectoryInfo dir2 = new DirectoryInfo(@"C:\Users\Hugo\newTemplates"); // Caso queira criar o PI de apenas uma pasta, use dir2 aqui....

                var vImg = dir.FullName + @"\vTemplates_" + dir.Name + ".txt"; // ...aqui

                var imagens = dir.GetFiles("*.png"); // ... e aqui.

                foreach (var i in imagens)
                {
                    var img = BitmapUtils.LoadImageWithoutLockFile(i.FullName);
                    var bmp = (Bitmap)img;
                    var m = new double[1, bmp.Width * bmp.Height];
                    var k = 0;

                    s.Append(dir.Name + "|" + bmp.Width + "|" + bmp.Height + "|");
                    for (var x = 0; x < bmp.Width; x++)
                    {
                        for (var y = 0; y < bmp.Height; y++)
                        {
                            var c = bmp.GetPixel(x, y);
                            //luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                            //m[0, k] = luminance / 255;
                            m[0, k] = c.R > 127 ? 1 : 0;
                            s.Append(string.Format("{0};", m[0, k]));
                            k++;
                        }
                    }
                    s.Remove(s.Length - 1, 1);
                    if (i.Name != imagens[imagens.Length - 1].Name)
                    {
                        s.AppendLine();
                    }
                }

                var sw = new StreamWriter(vImg);
                sw.Write(s.ToString());
                sw.Close();
                sw.Dispose();
                GC.Collect();
            }
        }

        /// <summary>
        ///   Renomeia todos os arquivos .png em dado diretório
        /// </summary>
        /// <param name="tipo"> 'p' para renomear com predict 'n' para nenomear com nome personalizado 't' para renomear com padrão reconhecido </param>
        public static void RenomearImagens(char tipo)
        {
            var di = new DirectoryInfo(@"C:\Users\Hugo\DropBox\OCR\SintegraSP\Todas");
            ValidatePath(di.FullName);

            var count = 1;

            //string vImg = @"D:\DEV\Captcha Final\Resources\vImg.txt";

            var imagens = di.GetFiles("*.png");

            switch (tipo)
            {
                case 'p':
                    {
                        var prdct = PredictCaptchaSP.Instance;
                        foreach (var i in imagens)
                        {
                            var m = new byte[3600];
                            var img = BitmapUtils.LoadImageWithoutLockFile(i.FullName);
                            var bmp = (Bitmap)img;
                            var k = 0;

                            for (var x = 0; x < bmp.Width; x++)
                            {
                                for (var y = 0; y < bmp.Height; y++)
                                {
                                    var c = bmp.GetPixel(x, y);
                                    var luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                                    m[k] = (byte)(luminance / 255);
                                    //m[0, x + (y * img.Width)] = luminance / 255;
                                    //s.Append(string.Format("{0};", m[k]));
                                    k++;
                                }
                            }

                            var p = prdct.Recognize(new ImgArray(m, 60, 60)); // a1) Renomear com a resposta do OCR 
                            //s.Append(string.Format("{0};\n", p)); // a2) Renomear com a resposta do OCR 
                            //count++;

                            var newName = i.Directory + @"\Predicted\" + p + " (" + count++ +
                                             ")" + ".png"; // a3) Renomear com a resposta do OCR 
                            ValidatePath(i.Directory + @"\Predicted\");

                            while (File.Exists(newName))
                            {
                                var r = new Random();
                                var num = r.Next(100, 10000);
                                newName = i.Directory + @"\Predicted\" + p + " (" + num + ")" +
                                          ".png";
                            }

                            i.CopyTo(newName);
                            //File.Move(i.FullName, newName);
                        }
                        break;
                    }
                case 'n':
                    {
                        foreach (var i in imagens)
                        {
                            var limp = i.Name.ToCharArray();
                            var nome = limp[0] + limp[1].ToString(CultureInfo.InvariantCulture);
                            nome = nome.Trim(); // b3) Renomear com nome personalizado
                            //s.Append(string.Format("{0};\n", nome)); // b4) Renomear com nome personalizado
                            GC.Collect();

                            var newName = i.Directory + @"\Predicted\" + nome + " (" + count++ + ")" +
                                             ".png"; // b5) Renomear com nome personalizado
                            ValidatePath(i.Directory + @"\Predicted\");

                            while (File.Exists(newName))
                            {
                                var r = new Random();
                                var num = r.Next(100, 10000);
                                newName = i.Directory + @"\Predicted\" + nome + " (" + num + ")" + ".png";
                            }

                            i.CopyTo(newName);
                            //File.Move(i.FullName, newName);
                        }
                        break;
                    }
                case 't':
                    {
                        var padrao = new HashSet<TipoPadrao>();

                        foreach (var i in imagens)
                        {
                            //  Bitmap img = (Bitmap)Image.FromFile(i.FullName);

                            padrao.Add((PredictPadraoCaptchaNFE.Instance.Recognize(ImgArray.LoadFromFile(i.FullName)) ==
                                        'b'
                                            ? TipoPadrao.Bandeira
                                            : TipoPadrao.Distorcida));
                            // Descomentar se quiser renomear as imagens selecionadas com o padrão
                            string temp = null; // identificado pelo reconhecimento automático (sem redes neurais)
                            if (padrao.Contains(TipoPadrao.Bandeira))
                                // Quando descomentar essas linhas, comentar as linhas de 116 à 129
                                temp = "1";
                            //sb.AppendLine("1");
                            else if (padrao.Contains(TipoPadrao.Distorcida))
                                temp = "2";
                            //sb.AppendLine("2");

                            var newName = i.FullName.Substring(0, i.FullName.LastIndexOf('\\') + 1) +
                                             "Renomeadas Para RN de Identificação\\" + temp + " (" + count +
                                             ").png";
                            ValidatePath(i.FullName.Substring(0, i.FullName.LastIndexOf('\\') + 1) +
                                         "Renomeadas Para RN de Identificação\\");

                            while (File.Exists(newName))
                            {
                                var r = new Random();
                                var num = r.Next(100, 10000);
                                newName = i.FullName.Substring(0, i.FullName.LastIndexOf('\\') + 1) +
                                          "Renomeadas Para RN de Identificação\\" + temp + " (" + num +
                                          ").png";
                            }

                            i.CopyTo(newName);
                            //File.Move(i.FullName, newName);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        ///   Altera valores adjacentes de mesmo valor para o valor escolhido
        /// </summary>
        /// <param name="img"> </param>
        /// <param name="startPosition"> Posição onde o FloodFill começará o preenchimento. </param>
        /// <param name="color"> Valor do preenchimento </param>
        /// <param name="connectivity"> Quais direções considerar (4 para N/S/L/O ou 8 para N/NE/NO/L/O/S/SE/SO) </param>
        /// <returns> Vetor preenchido </returns>
        /*public static int[,] FloodFill(this int[,] img, Point startPosition, int color, int connectivity = 8)
        {
            int targetColor = img[startPosition.X, startPosition.Y];
            Queue<Point> q = new Queue<Point>();
            q.Enqueue(startPosition);
            while (q.Count != 0)
            {
                Point currentPosition = q.Dequeue();
                if (img[currentPosition.X, currentPosition.Y] == targetColor)
                {
                    img[currentPosition.X, currentPosition.Y] = color;

                    if (connectivity == 8)
                    {
                        q.Enqueue(new Point(currentPosition.X - 1, currentPosition.Y - 1)); // northWest
                        q.Enqueue(new Point(currentPosition.X, currentPosition.Y - 1)); // north 
                        q.Enqueue(new Point(currentPosition.X + 1, currentPosition.Y - 1)); // northEast
                        q.Enqueue(new Point(currentPosition.X - 1, currentPosition.Y)); // west
                        q.Enqueue(new Point(currentPosition.X + 1, currentPosition.Y)); // east
                        q.Enqueue(new Point(currentPosition.X - 1, currentPosition.Y + 1)); // southWest
                        q.Enqueue(new Point(currentPosition.X, currentPosition.Y + 1)); // south
                        q.Enqueue(new Point(currentPosition.X + 1, currentPosition.Y + 1)); // southEast
                    }
                    else
                    {
                        q.Enqueue(new Point(currentPosition.X, currentPosition.Y - 1)); // north 
                        q.Enqueue(new Point(currentPosition.X - 1, currentPosition.Y)); // west
                        q.Enqueue(new Point(currentPosition.X + 1, currentPosition.Y)); // east
                        q.Enqueue(new Point(currentPosition.X, currentPosition.Y + 1)); // south
                    }
                }
            }

            return img;
        }*/
        public static int[,] FloodFill(this int[,] img, Point startPosition, int color, int connectivity = 8)
        {
            var targetColor = img[startPosition.X, startPosition.Y];
            var q = new Queue<Point>();
            q.Enqueue(startPosition);
            while (q.Count != 0)
            {
                var currentPosition = q.Dequeue();
                var validRegion = GetBounds(img, currentPosition);

                if (img[currentPosition.X, currentPosition.Y] == targetColor)
                {
                    img[currentPosition.X, currentPosition.Y] = color;

                    if (connectivity == 8)
                    {
                        if (validRegion[0])
                        {
                            q.Enqueue(new Point(currentPosition.X - 1, currentPosition.Y)); // west
                            if (validRegion[2])
                            {
                                q.Enqueue(new Point(currentPosition.X - 1, currentPosition.Y - 1)); // northWest
                            }
                            if (validRegion[3])
                            {
                                q.Enqueue(new Point(currentPosition.X - 1, currentPosition.Y + 1)); // southWest
                            }
                        }
                        if (validRegion[1])
                        {
                            q.Enqueue(new Point(currentPosition.X + 1, currentPosition.Y)); // east
                            if (validRegion[2])
                            {
                                q.Enqueue(new Point(currentPosition.X + 1, currentPosition.Y - 1)); // northEast
                            }
                            if (validRegion[3])
                            {
                                q.Enqueue(new Point(currentPosition.X + 1, currentPosition.Y + 1)); // southEast
                            }
                        }
                        if (validRegion[2])
                        {
                            q.Enqueue(new Point(currentPosition.X, currentPosition.Y - 1)); // north 
                        }
                        if (validRegion[3])
                        {
                            q.Enqueue(new Point(currentPosition.X, currentPosition.Y + 1));
                            // south                            
                        }
                    }
                    else
                    {
                        q.Enqueue(new Point(currentPosition.X, currentPosition.Y - 1)); // north 
                        q.Enqueue(new Point(currentPosition.X - 1, currentPosition.Y)); // west
                        q.Enqueue(new Point(currentPosition.X + 1, currentPosition.Y)); // east
                        q.Enqueue(new Point(currentPosition.X, currentPosition.Y + 1)); // south
                    }
                }
            }

            return img;
        }

        /// <summary>
        ///   Retorna uma lista com todos os pixels do cluster informado
        /// </summary>
        /// <param name="img"> </param>
        /// <param name="startPosition"> </param>
        /// <param name="connectivity"> </param>
        /// <returns> </returns>
        public static List<Point> GetCluster(this ImgArray img, Point startPosition, int connectivity = 8)
        {
            int targetColor = img.GetByte(startPosition.X, startPosition.Y);
            var q = new Queue<Point>();
            q.Enqueue(startPosition);


            var pixelsProcessados = new List<Point>();

            while (q.Count != 0)
            {
                var currentPosition = q.Dequeue();
                var validRegion = GetBounds(img, currentPosition);

                if (img.GetByte(currentPosition.X, currentPosition.Y) == targetColor
                    && !pixelsProcessados.Contains(currentPosition))
                {
                    pixelsProcessados.Add(currentPosition);

                    if (connectivity == 8)
                    {
                        if (validRegion[0])
                        {
                            q.Enqueue(new Point(currentPosition.X - 1, currentPosition.Y)); // west
                            if (validRegion[2])
                            {
                                q.Enqueue(new Point(currentPosition.X - 1, currentPosition.Y - 1)); // northWest
                            }
                            if (validRegion[3])
                            {
                                q.Enqueue(new Point(currentPosition.X - 1, currentPosition.Y + 1)); // southWest
                            }
                        }
                        if (validRegion[1])
                        {
                            q.Enqueue(new Point(currentPosition.X + 1, currentPosition.Y)); // east
                            if (validRegion[2])
                            {
                                q.Enqueue(new Point(currentPosition.X + 1, currentPosition.Y - 1)); // northEast
                            }
                            if (validRegion[3])
                            {
                                q.Enqueue(new Point(currentPosition.X + 1, currentPosition.Y + 1)); // southEast
                            }
                        }
                        if (validRegion[2])
                        {
                            q.Enqueue(new Point(currentPosition.X, currentPosition.Y - 1)); // north 
                        }
                        if (validRegion[3])
                        {
                            q.Enqueue(new Point(currentPosition.X, currentPosition.Y + 1));
                            // south                            
                        }
                    }
                    else
                    {
                        q.Enqueue(new Point(currentPosition.X, currentPosition.Y - 1)); // north 
                        q.Enqueue(new Point(currentPosition.X - 1, currentPosition.Y)); // west
                        q.Enqueue(new Point(currentPosition.X + 1, currentPosition.Y)); // east
                        q.Enqueue(new Point(currentPosition.X, currentPosition.Y + 1)); // south
                    }
                }
            }
            return pixelsProcessados;
        }

        /// <summary>
        ///   Preenche pixels adjacentes de mesma cor com a cor escolhida
        /// </summary>
        /// <param name="img"> </param>
        /// <param name="startPosition"> Posição onde o FloodFill começará o preenchimento. </param>
        /// <param name="color"> Cor do preenchimento </param>
        /// <param name="connectivity"> Quais pixels/direção considerar (4 para N/S/L/O ou 8 para N/NE/NO/L/O/S/SE/SO) </param>
        /// <returns> Bitmap preenchido </returns>
        public static Bitmap FloodFill(this Bitmap img, Point startPosition, int color, int connectivity = 8)
        {
            var targetColor = img.GetPixel(startPosition.X, startPosition.Y);
            //TODO: 30 devera ser calculado de acordo com numero de clusters = 255/clusters
            var fillColor = Color.FromArgb((30 * color) % 255, (30 * color) % 255, (30 * color) % 255);
            var hash = new Dictionary<Point, Color> { { startPosition, targetColor } };

            while (hash.Count != 0)
            {
                var corPixel = hash.Last().Value;
                var pixelPoint = hash.Last().Key;
                hash.Remove(pixelPoint);

                if (corPixel == targetColor)
                {
                    img.SetPixel(pixelPoint.X, pixelPoint.Y, fillColor);

                    var validRegion = GetBounds(img, pixelPoint);

                    if (connectivity == 8)
                    {
                        //Esquerda
                        if (validRegion[0])
                        {
                            if (!hash.ContainsKey(new Point(pixelPoint.X - 1, pixelPoint.Y)))
                            {
                                hash.Add(new Point(pixelPoint.X - 1, pixelPoint.Y),
                                         img.GetPixel(pixelPoint.X - 1, pixelPoint.Y)); // west
                            }
                            // Acima
                            if (validRegion[2])
                            {
                                if (!hash.ContainsKey(new Point(pixelPoint.X - 1, pixelPoint.Y - 1)))
                                {
                                    hash.Add(new Point(pixelPoint.X - 1, pixelPoint.Y - 1),
                                             img.GetPixel(pixelPoint.X - 1, pixelPoint.Y - 1)); // northWest
                                }
                            }
                            // Abaixo
                            if (validRegion[3])
                            {
                                if (!hash.ContainsKey(new Point(pixelPoint.X - 1, pixelPoint.Y + 1)))
                                {
                                    hash.Add(new Point(pixelPoint.X - 1, pixelPoint.Y + 1),
                                             img.GetPixel(pixelPoint.X - 1, pixelPoint.Y + 1)); // southWest
                                }
                            }
                        }

                        //Direita
                        if (validRegion[1])
                        {
                            if (!hash.ContainsKey(new Point(pixelPoint.X + 1, pixelPoint.Y)))
                            {
                                hash.Add(new Point(pixelPoint.X + 1, pixelPoint.Y),
                                         img.GetPixel(pixelPoint.X + 1, pixelPoint.Y)); // east
                            }
                            // Acima
                            if (validRegion[2])
                            {
                                if (!hash.ContainsKey(new Point(pixelPoint.X + 1, pixelPoint.Y - 1)))
                                {
                                    hash.Add(new Point(pixelPoint.X + 1, pixelPoint.Y - 1),
                                             img.GetPixel(pixelPoint.X + 1, pixelPoint.Y - 1)); // northEast
                                }
                            }
                            // Abaixo
                            if (validRegion[3])
                            {
                                if (!hash.ContainsKey(new Point(pixelPoint.X + 1, pixelPoint.Y + 1)))
                                {
                                    hash.Add(new Point(pixelPoint.X + 1, pixelPoint.Y + 1),
                                             img.GetPixel(pixelPoint.X + 1, pixelPoint.Y + 1)); // southEast
                                }
                            }
                        }
                        // Acima
                        if (validRegion[2])
                        {
                            if (!hash.ContainsKey(new Point(pixelPoint.X, pixelPoint.Y - 1)))
                            {
                                hash.Add(new Point(pixelPoint.X, pixelPoint.Y - 1),
                                         img.GetPixel(pixelPoint.X, pixelPoint.Y - 1)); // north 
                            }
                        }
                        // Abaixo
                        if (validRegion[3])
                        {
                            if (!hash.ContainsKey(new Point(pixelPoint.X, pixelPoint.Y + 1)))
                            {
                                hash.Add(new Point(pixelPoint.X, pixelPoint.Y + 1),
                                         img.GetPixel(pixelPoint.X, pixelPoint.Y + 1)); // south
                            }
                        }
                    }
                    else
                    {
                        if (validRegion[0])
                        {
                            hash.Add(new Point(pixelPoint.X - 1, pixelPoint.Y),
                                     img.GetPixel(pixelPoint.X - 1, pixelPoint.Y)); // west
                        }
                        if (validRegion[1])
                        {
                            hash.Add(new Point(pixelPoint.X + 1, pixelPoint.Y),
                                     img.GetPixel(pixelPoint.X + 1, pixelPoint.Y)); // east
                        }
                        if (validRegion[2])
                        {
                            hash.Add(new Point(pixelPoint.X, pixelPoint.Y - 1),
                                     img.GetPixel(pixelPoint.X, pixelPoint.Y - 1)); // north 
                        }
                        if (validRegion[3])
                        {
                            hash.Add(new Point(pixelPoint.X, pixelPoint.Y + 1),
                                     img.GetPixel(pixelPoint.X, pixelPoint.Y + 1)); // south
                        }
                    }
                }
            }

            return img;
        }

        /*public static bool[] getBounds(this Bitmap img, Point p)
        {
            bool[] validPosition = new bool[4] { false, false, false, false }; // {esquerda, direita, acima, abaixo}
            if (p.X > 0) validPosition[0] = true; // Existe pixel à esquerda de p
            if (p.X < img.Width - 1) validPosition[1] = true; // Existe pixel à direita de p
            if (p.Y > 0) validPosition[2] = true; // Existe pixel acima de p
            if (p.Y < img.Height - 1) validPosition[3] = true; // Existe pixel abaixo de p

            return validPosition;
        }*/

        public static bool[] GetBounds(this Bitmap img, Point p)
        {
            // Verifica a existência de pixels ao redor do point informado
            return new[]
                       {
                           (p.X > 0), // Esquerda
                           (p.X < img.Width - 1), // Direita
                           (p.Y > 0), // Acima
                           (p.Y < img.Height - 1) // Abaixo
                       };
        }


        public static bool[] GetBounds(this ImgArray img, Point p)
        {
            // Verifica a existência de pixels ao redor do point informado
            return new[]
                       {
                           (p.X > 0), // Esquerda
                           (p.X < img.Width - 1), // Direita
                           (p.Y > 0), // Acima
                           (p.Y < img.Height - 1) // Abaixo
                       };
        }

        public static bool[] GetBounds(this int[,] img, Point p)
        {
            // Verifica a existência de pixels ao redor do point informado
            return new[]
                       {
                           (p.X > 0), // Esquerda
                           (p.X < img.GetLength(0) - 1), // Direita
                           (p.Y > 0), // Acima
                           (p.Y < img.GetLength(1) - 1) // Abaixo
                       };
        }

        public static Color InvertColor(Color colorToInvert)
        {
            const int rgbMax = 255;
            return Color.FromArgb(rgbMax - colorToInvert.R, rgbMax - colorToInvert.G, rgbMax - colorToInvert.B);
        }


        public static Bitmap InvertImageColors(this Bitmap bmp)
        {
            for (var y = 0; y < bmp.Height; y++)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    bmp.SetPixel(x, y, InvertColor(bmp.GetPixel(x, y)));
                }
            }

            return bmp;
        }

        public static ImgArray InvertImageColors(this ImgArray imgArray)
        {
            for (var y = 0; y < imgArray.Height; y++)
            {
                for (var x = 0; x < imgArray.Width; x++)
                {
                    imgArray.SetPixel(x, y, InvertColor(imgArray.GetPixel(x, y)));
                }
            }

            return imgArray;
        }


        public static void PrintMatrix(this int[,] img, bool invertAxis = false)
        {
            var x = 0;
            var y = 1;

            if (invertAxis)
            {
                x = 1;
                y = 0;
            }

            for (var i = 0; i < img.GetLength(y); i++)
            {
                for (var j = 0; j < img.GetLength(x); j++)
                {
                    Console.Write(img[i, j] + " ");
                }
                Console.Write("\n");
            }
        }

        public static void PrintMatrix(this ImgArray img)
        {
            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    int c = img.GetPixel(x, y).R;
                    if (c == 255) c = 1;
                    Console.Write(c + " ");
                }
                Console.Write("\n");
            }
        }

        public static ImgArray MinTemplateP(string arquivoTemplates)
        {
            var format = CultureInfo.CurrentCulture.NumberFormat;
            var separadorDecimal = format.NumberDecimalSeparator.First();

            var templates = new List<ChaveValor<char, ImgArray>>();

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
                    templates.Add(new ChaveValor<char, ImgArray>
                                      {
                                          Chave = letra,
                                          Valor = img
                                      });
                }
            }

            var minTemplate = templates[0].Valor;
            var imgTemplates = from t in templates
                               select t.Valor;

            foreach (var img in imgTemplates.Where(img => img.Width < minTemplate.Width))
            {
                return new ImgArray(img);
            }

            return minTemplate;
        }

        public static unsafe Bitmap AdjustContrast(this Bitmap bmp, double contrast)
        {
            //bmp = (Bitmap) Bitmap.FromFile(@"C:\Users\Hugo\1 (1).tif");

            var contrastLookup = new byte[256];
            var c = (100.0 + contrast) / 100.0;

            c *= c;

            for (var i = 0; i < 256; i++)
            {
                double newValue = i;
                newValue /= 255.0;
                newValue -= 0.5;
                newValue *= c;
                newValue += 0.5;
                newValue *= 255;

                if (newValue < 0)
                    newValue = 0;
                if (newValue > 255)
                    newValue = 255;
                contrastLookup[i] = (byte)newValue;
            }

            var bitmapdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                 ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            const int pixelSize = 4;

            for (var y = 0; y < bitmapdata.Height; y++)
            {
                var destPixels = (byte*)bitmapdata.Scan0 + (y * bitmapdata.Stride);
                for (var x = 0; x < bitmapdata.Width; x++)
                {
                    destPixels[x * pixelSize] = contrastLookup[destPixels[x * pixelSize]]; // B 
                    destPixels[x * pixelSize + 1] = contrastLookup[destPixels[x * pixelSize + 1]]; // G 
                    destPixels[x * pixelSize + 2] = contrastLookup[destPixels[x * pixelSize + 2]]; // R 
                    //destPixels[x * PixelSize + 3] = contrast_lookup[destPixels[x * PixelSize + 3]]; //A 
                }
            }
            bmp.UnlockBits(bitmapdata);
            return bmp;
        }

        public static unsafe Bitmap AdjustBrightness(this Bitmap bmp, double brightness)
        {
            //bmp = (Bitmap)Bitmap.FromFile(@"C:\Users\Hugo\1 (1).tif");

            var brightnessLookup = new byte[256];
            var c = brightness; // (100.0 + 15) / 100.0;

            for (var i = 0; i < 256; i++)
            {
                double newValue = i;
                //newValue /= 255.0;
                //newValue -= 0.5;
                newValue += c;
                //newValue += 0.5;
                //newValue *= 255;

                if (newValue < 0)
                    newValue = 0;
                if (newValue > 255)
                    newValue = 255;
                brightnessLookup[i] = (byte)newValue;
            }

            var bitmapdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                 ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            const int pixelSize = 4;

            for (var y = 0; y < bitmapdata.Height; y++)
            {
                var destPixels = (byte*)bitmapdata.Scan0 + (y * bitmapdata.Stride);
                for (var x = 0; x < bitmapdata.Width; x++)
                {
                    destPixels[x * pixelSize] = brightnessLookup[destPixels[x * pixelSize]]; // B 
                    destPixels[x * pixelSize + 1] = brightnessLookup[destPixels[x * pixelSize + 1]]; // G 
                    destPixels[x * pixelSize + 2] = brightnessLookup[destPixels[x * pixelSize + 2]]; // R 
                    //destPixels[x * PixelSize + 3] = contrast_lookup[destPixels[x * PixelSize + 3]]; //A 
                }
            }
            bmp.UnlockBits(bitmapdata);
            return bmp;
        }

        public static unsafe Bitmap MakeBlackAndWhite(this Bitmap bmp, byte threshold)
        {
            var newBitmap = new Bitmap(bmp.Width, bmp.Height, bmp.PixelFormat);

            //lock the original bitmap in memory
            var originalData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, bmp.PixelFormat);

            //lock the new bitmap in memory
            var newData = newBitmap.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.WriteOnly, newBitmap.PixelFormat);

            const int pixelSize = 4;

            for (var y = 0; y < originalData.Height; y++)
            {
                var oRow = (byte*)originalData.Scan0 + (y * originalData.Stride);

                //get the data from the new image
                var nRow = (byte*)newData.Scan0 + (y * newData.Stride);

                for (var x = 0; x < originalData.Width; x++)
                {
                    var bwValue = oRow[x * pixelSize] >= threshold ? (byte)255 : (byte)0;
                    nRow[x * pixelSize] = bwValue; // B 
                    nRow[x * pixelSize + 1] = bwValue; // G 
                    nRow[x * pixelSize + 2] = bwValue; // R 
                    //destPixels[x * PixelSize + 3] = contrast_lookup[destPixels[x * PixelSize + 3]]; //A 
                }
            }

            //unlock the bitmaps
            newBitmap.UnlockBits(newData);
            bmp.UnlockBits(originalData);

            return newBitmap;
        }

        public static unsafe int CountNumberOfColors(this Bitmap img)
        {
            var l = new List<byte>();

            var bitdata = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                                              ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            const int pixelSize = 4;

            for (var y = 0; y < bitdata.Height; y++)
            {
                var origPixels = (byte*)bitdata.Scan0 + (y * bitdata.Stride);
                for (var x = 0; x < bitdata.Width; x++)
                {
                    var blue = origPixels[x * pixelSize];
                    var green = origPixels[x * pixelSize + 1];
                    var red = origPixels[x * pixelSize + 2];

                    if (!l.Contains(red))
                        l.Add(red);
                    if (!l.Contains(green))
                        l.Add(green);
                    if (!l.Contains(blue))
                        l.Add(blue);
                }
            }

            img.UnlockBits(bitdata);

            return l.Count;
        }

        // Comparador que aceita duplicatas

        public static ChaveValor<Type, Type> GetTypeCaptchaAndPredictById(string id)
        {
            ChaveValor<Type, Type> result;
            switch (id)
            {
                case "RF":
                    result = new ChaveValor<Type, Type>
                                 {
                                     Chave = typeof(CaptchaRF),
                                     Valor = typeof(PredictCaptchaRF)
                                 };
                    break;
                case "RF3":
                    result = new ChaveValor<Type, Type>
                    {
                        Chave = typeof(CaptchaRF3),
                        Valor = typeof(PredictCaptchaRF3)
                    };
                    break;
                case "NFE":
                    result = new ChaveValor<Type, Type>
                                 {
                                     Chave = typeof(CaptchaNFE),
                                     Valor = typeof(PredictCaptchaNFE)
                                 };
                    break;

                case "SI":
                    result = new ChaveValor<Type, Type>
                                 {
                                     Chave = typeof(CaptchaSI),
                                     Valor = null
                                 };
                    break;
                case "SP":
                    result = new ChaveValor<Type, Type>
                                 {
                                     Chave = typeof(CaptchaSP),
                                     Valor = typeof(PredictCaptchaSP)
                                 };
                    break;
                case "RJ":
                    result = new ChaveValor<Type, Type>
                                 {
                                     Chave = typeof(CaptchaRJ),
                                     Valor = typeof(PredictCaptchaRJ)
                                 };
                    break;
                case "AM":
                    result = new ChaveValor<Type, Type>
                                 {
                                     Chave = typeof(CaptchaAM),
                                     Valor = typeof(PredictCaptchaAM)
                                 };
                    break;
                case "MG":
                    result = new ChaveValor<Type, Type>
                                 {
                                     Chave = typeof(CaptchaMG),
                                     Valor = typeof(PredictCaptchaMG)
                                 };
                    break;
                case "CRJ":
                    result = new ChaveValor<Type, Type>
                                 {
                                     Chave = typeof(CaptchaCRJ),
                                     Valor = typeof(PredictCaptchaCRJ)
                                 };
                    break;
                case "CRJv":
                    result = new ChaveValor<Type, Type>
                                 {
                                     Chave = typeof(CaptchaCRJ),
                                     Valor = typeof(PredictCaptchaCRJVerde)
                                 };
                    break;
                case "CRJa":
                    result = new ChaveValor<Type, Type>
                                 {
                                     Chave = typeof(CaptchaCRJ),
                                     Valor = typeof(PredictCaptchaCRJAzul)
                                 };
                    break;
                case "CAM":
                    result = new ChaveValor<Type, Type>
                                 {
                                     Chave = typeof(CaptchaCA),
                                     Valor = typeof(PredictCaptchaCAM)
                                 };
                    break;
                case "CA":
                    result = new ChaveValor<Type, Type>
                                 {
                                     Chave = typeof(CaptchaCA),
                                     Valor = typeof(PredictCaptchaCAM)
                                 };
                    break;
                case "CM":
                    result = new ChaveValor<Type, Type>
                                 {
                                     Chave = typeof(CaptchaCM),
                                     Valor = typeof(PredictCaptchaCAM)
                                 };
                    break;
                case "CCT":
                    result = new ChaveValor<Type, Type>
                                {
                                    Chave = typeof(CctCaptchaA),
                                    Valor = null
                                };
                    break;
                case "TRTSP":
                    result = new ChaveValor<Type, Type>
                                {
                                    Chave = typeof(CaptchaTRTSP),
                                    Valor = typeof(PredictCaptchaTRTSP)
                                };
                    break;
                case "TJPE":
                    result = new ChaveValor<Type, Type>
                                {
                                    Chave = typeof(CaptchaTJPE),
                                    Valor = typeof(PredictCaptchaTJPE)
                                };
                    break;
                case "TJMG":
                    result = new ChaveValor<Type, Type>
                                {
                                    Chave = null,
                                    Valor = null
                                };
                    break;
                case "ESAJ":
                    result = new ChaveValor<Type, Type>
                    {
                        Chave = typeof(CaptchaESAJ),
                        Valor = typeof(PredictCaptchaESAJ)
                    };
                    break;
                case "PJE":
                    result = new ChaveValor<Type, Type>
                    {
                        Chave = typeof(CaptchaPJE),
                        Valor = typeof(PredictCaptchaPJE)
                    };
                    break;
                case "ProjudiBA":
                    result = new ChaveValor<Type, Type>
                    {
                        Chave = typeof(CaptchaProjudiBA),
                        Valor = typeof(PredictCaptchaNFE)
                    };
                    break;
                case "ProjudiGeral":
                    result = new ChaveValor<Type, Type>
                    {
                        Chave = typeof(CaptchaProjudiGeral),
                        Valor = typeof(PredictCaptchaCAM)
                    };
                    break;
                case "ProjudiAM":
                    result = new ChaveValor<Type, Type>
                    {
                        Chave = typeof(CaptchaProjudiGeral),
                        Valor = typeof(PredictCaptchaCAM)
                    };
                    break;

                case "RF4":
                    result = new ChaveValor<Type, Type>
                    {
                        Chave = typeof(CaptchaRF4),
                        Valor = typeof(PredictCaptchaRF4)
                    };
                    break;
                default:
                    throw new Exception(
                        "Captcha não implementado ou não especificado em Bll.ServerUtil.GetTypeCaptchaByID!");
            }
            return result;
        }

        public static int GetMinX(this ImgArray imgArray)
        {
            var x1 = -1;
            for (var x = 0; x < imgArray.Width; x++)
            {
                for (var y = 0; y < imgArray.Height; y++)
                {
                    var c = imgArray.GetPixel(x, y);
                    if (c.IsBlackPixel())
                    {
                        x1 = x;
                        break;
                    }
                }
                if (x1 > -1)
                {
                    break;
                }
            }
            return x1;
        }

        public static Point GetMinXPoint(this ImgArray imgArray)
        {
            var x1 = -1;
            var y1 = -1;
            for (var x = 0; x < imgArray.Width; x++)
            {
                for (var y = 0; y < imgArray.Height; y++)
                {
                    var c = imgArray.GetPixel(x, y);
                    if (c.IsBlackPixel())
                    {
                        x1 = x;
                        y1 = y;
                        break;
                    }
                }
                if (x1 > -1)
                {
                    break;
                }
            }
            return new Point(x1, y1);
        }

        //<summary>
        //Varre o bitmap e retorna as coordenadas do pixel preto mais próximo da
        //lateral direita da imagem
        //</summary>
        //<param name="bmp"></param>
        //<returns></returns>
        public static int GetMaxX(this ImgArray imgArray)
        {
            var x2 = -1;
            for (var x = imgArray.Width - 1; x > 0; x--)
            {
                for (var y = 0; y < imgArray.Height; y++)
                {
                    var c = imgArray.GetPixel(x, y);
                    if (c.IsBlackPixel())
                    {
                        x2 = x;
                        break;
                    }
                }
                if (x2 > -1)
                {
                    break;
                }
            }
            return x2;
        }

        public static Point GetMaxXPoint(this ImgArray imgArray)
        {
            var x2 = -1;
            var y2 = -1;
            for (var x = imgArray.Width - 1; x > 0; x--)
            {
                for (var y = 0; y < imgArray.Height; y++)
                {
                    var c = imgArray.GetPixel(x, y);
                    if (c.IsBlackPixel())
                    {
                        x2 = x;
                        y2 = y;
                        break;
                    }
                }
                if (x2 > -1)
                {
                    break;
                }
            }
            return new Point(x2, y2);
        }

        //<summary>
        //Varre o bitmap e retorna o Y do pixel preto mais próximo da
        //parte superior da imagem
        //</summary>
        //<param name="bmp"></param>
        //<returns></returns>
        public static int GetMinY(this ImgArray imgArray)
        {
            var y1 = -1;
            for (var y = 0; y < imgArray.Height; y++)
            {
                for (var x = 0; x < imgArray.Width; x++)
                {
                    var c = imgArray.GetPixel(x, y);
                    if (c.IsBlackPixel())
                    {
                        y1 = y;
                        break;
                    }
                }
                if (y1 > -1)
                {
                    break;
                }
            }
            return y1;
        }


        //<summary>
        //Varre o bitmap e retorna o Y do pixel preto mais próximo da
        //parte inferior da imagem
        //</summary>
        //<param name="bmp"></param>
        //<returns></returns>
        public static int GetMaxY(this ImgArray imgArray)
        {
            var y2 = -1;
            for (var y = imgArray.Height - 1; y > 0; y--)
            {
                for (var x = 0; x < imgArray.Width; x++)
                {
                    var c = imgArray.GetPixel(x, y);
                    if (c.IsBlackPixel())
                    {
                        y2 = y;
                        break;
                    }
                }
                if (y2 > -1)
                {
                    break;
                }
            }
            return y2;
        }

        public static void OrderByPixels()
        {
            const string baseDir = @"C:\OCR\Testes\AM\Imagens";
            const string orderDir = @"C:\OCR\Testes\AM\Imagens\Ordenadas";

            if (!Directory.Exists(baseDir))
            {
                MessageBox.Show("Colocar as imagens a serem classificadas em:\n" + baseDir);
                return;
            }

            if (!Directory.Exists(orderDir))
            {
                Directory.CreateDirectory(orderDir);
            }

            var di = new DirectoryInfo(baseDir);

            var files = di.GetFiles("*.png");

            Func<FileInfo, int> orderBy =
                file =>
                new ImgArray((Bitmap)BitmapUtils.LoadImageWithoutLockFile(file.FullName)).CountPixelsWithColor(
                    Color.Black);

            var filesOdered = files.OrderBy(orderBy);

            foreach (var file in filesOdered)
            {
                new ImgArray((Bitmap)BitmapUtils.LoadImageWithoutLockFile(file.FullName)).Save(
                    String.Format("{0}\\{1}.png", orderDir, file.Name));
            }
        }

        public static ChaveValor<int, Point> GetAlturaETopoLinha(this ImgArray imgArray, Point point)
        {
            var altura = 0;
            // subir até o primeiro pixel preto
            while (imgArray.GetPixel(point.X, point.Y).IsBlackPixel())
            {
                point.Y--;
            }
            var topo = new Point(point.X, ++point.Y);
            while (imgArray.GetPixel(point.X, point.Y).IsBlackPixel())
            {
                altura++;
                point.Y++;
            }
            return new ChaveValor<int, Point> { Chave = altura, Valor = topo };
        }

        #region Métodos inutilizados no momento

        //public static void Resize(this Bitmap pic)
        //{
        //    Graphics g;
        //    g.DrawImage((Image) pic, )
        //}

        //public static Bitmap AdjustLevels(this Bitmap img, double[] input)
        //{
        //    return AdjustLevels(img, input, new double[2] { 0, 255 });
        //}

        public static Bitmap AdjustLevels(this Bitmap img, double[] input, double[] output)
        {
            if (input.Length != 3)
            {
                Log.Append("O vetor de níveis de entrada deve ter 3 valores (shadows, midtones, highlights).");
                return null;
            }

            if (output.Length != 2)
            {
                Log.Append("O vetor de níveis de saída deve ter 2 valores (shadows, highlights).");
                return null;
            }

            var inShadows = input[0];
            var inMidtones = input[1];
            var inHighlights = input[2];

            int sumR = 0, sumG = 0, sumB = 0;
            for (var w = 0; w < img.Height; w++)
            {
                for (var z = 0; z < img.Width; z++)
                {
                    var k = img.GetPixel(z, w);
                    sumR += k.R;
                    sumG += k.G;
                    sumB += k.B;
                }
            }

            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    // Update Highlights
                    var c = img.GetPixel(x, y);
                    if (c.R >= inHighlights && c.G >= inHighlights && c.B >= inHighlights)
                    {
                        img.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    }
                    else
                    {
                        img.SetPixel(x, y, Color.FromArgb((c.R / sumR) * 255, (c.G / sumG) * 255, (c.B / sumB) * 255));
                    }
                }
            }

            sumR = 0;
            sumG = 0;
            sumB = 0;
            for (var w = 0; w < img.Height; w++)
            {
                for (var z = 0; z < img.Width; z++)
                {
                    var k = img.GetPixel(z, w);
                    sumR += k.R;
                    sumG += k.G;
                    sumB += k.B;
                }
            }

            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    // Update Shadows
                    var c = img.GetPixel(x, y);
                    if (c.R <= inShadows && c.G <= inShadows && c.B <= inShadows)
                    {
                        img.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    }
                    else
                    {
                        img.SetPixel(x, y, Color.FromArgb((c.R / sumR) * 255, (c.G / sumG) * 255, (c.B / sumB) * 255));
                    }
                }
            }

            sumR = 0;
            sumG = 0;
            sumB = 0;
            for (var w = 0; w < img.Height; w++)
            {
                for (var z = 0; z < img.Width; z++)
                {
                    var k = img.GetPixel(z, w);
                    sumR += k.R;
                    sumG += k.G;
                    sumB += k.B;
                }
            }

            var midAdjust = (int)inMidtones - (255 / 2);

            var brightness = new BrightnessCorrection(midAdjust);
            brightness.ApplyInPlace(img);

            if (midAdjust > 0)
            {
                for (var y = 0; y < img.Height; y++)
                {
                    for (var x = 0; x < img.Width; x++)
                    {
                        // Update Midtones
                        var c = img.GetPixel(x, y);
                        if (c.R >= inHighlights && c.G >= inHighlights && c.B >= inHighlights)
                        {
                            var contrast = new ContrastCorrection(-midAdjust);
                            contrast.ApplyInPlace(img, new Rectangle(x, y, 1, 1));
                        }
                    }
                }
            }
            if (midAdjust < 0)
            {
                for (var y = 0; y < img.Height; y++)
                {
                    for (var x = 0; x < img.Width; x++)
                    {
                        // Update Midtones
                        var c = img.GetPixel(x, y);
                        if (c.R <= inShadows && c.G <= inShadows && c.B <= inShadows)
                        {
                            var contrast = new ContrastCorrection(midAdjust);
                            contrast.ApplyInPlace(img, new Rectangle(x, y, 1, 1));
                        }
                    }
                }
            }

            return img;
        }

        public static void CriarPastas(String destino)
        {
            var di = new DirectoryInfo(destino);

            // Cria as pastas A-Z, 0-9, _lixo
            for (var c = 'A'; c <= 'z'; c++)
            {
                if (char.IsSymbol(c) || c == ']' || c == '[' || c == '_')
                {
                    continue;
                }
                if (char.IsUpper(c))
                {
                    if (!Directory.Exists(di.FullName + @"\" + c + c))
                    {
                        Directory.CreateDirectory(di.FullName + @"\" + c + c);
                        continue;
                    }
                }

                if (!Directory.Exists(di.FullName + @"\" + c))
                {
                    Directory.CreateDirectory(di.FullName + @"\" + c);
                }
            }

            for (var i = 0; i <= 9; i++)
            {
                if (!Directory.Exists(di.FullName + @"\" + i))
                {
                    Directory.CreateDirectory(di.FullName + @"\" + i);
                }
            }

            // Cria a pasta _lixo
            //Directory.CreateDirectory(di.FullName + @"\" + "_lixo");

            MessageBox.Show("Pastas criadas com sucesso.");
        }

        #endregion

        #region Implementado na Library

        // Implementado na Library
        //public static double[,] PixelIntensity(this Bitmap img)
        //{
        //    double[,] m = new double[1, img.Height * img.Width];

        //    int k = 0;
        //    for (int x = 0; x < img.Width; x++)
        //    {
        //        for (int y = 0; y < img.Height; y++)
        //        {
        //            Color c = img.GetPixel(x, y);
        //            int luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
        //            //m[0, k++] = (luminance > 127) ? 1 : 0;
        //            m[0, k++] = luminance / 255;
        //        }
        //    }
        //    return m;
        //}

        //Implementado na library
        //public static Bitmap BitmapFromSmallArray(this int[][] imgSmallArray, int Width, int Height)
        //{
        //    Bitmap bmp = new Bitmap(Width, Height);
        //    List<double> lista = new List<double>();

        //    // Converter SmallArray em imgArray
        //    foreach (var item in imgSmallArray[0])
        //    {
        //        if (item == 0 || item == 1)
        //        {
        //            lista.Add(item);
        //        }
        //        else
        //        {
        //            for (int i = 0; i < item; i++)
        //            {
        //                lista.Add(1);
        //            }
        //        }
        //    }

        //    double[][] result = new double[1][];
        //    result[0] = new double[lista.Count];
        //    result[0] = lista.ToArray();

        //    return result.BitmapFromArray(Width, Height);

        //}

        /*  public static Bitmap BitmapFromArray(this double[][] imgArray, int Width, int Height)
           {
               Bitmap bmp = new Bitmap(Width, Height);

               int k = 0;
               for (int y = 0; y < bmp.Height; y++)
               {
                   int FimLinha = (y + 1) * bmp.Width;
                   for (int x = k; x < FimLinha; x++)
                   {
                       Color px;
                       if (imgArray[0][x] == 1)
                       {
                           px = Color.White;
                       }
                       else
                       {
                           px = Color.Black;
                       }
                       int x1 = x % bmp.Width;
                       bmp.SetPixel(x1, y, px);
                   }
                   k = FimLinha;
               }
               return bmp;
           }*/
        // Está implementado na Library
        //public static Bitmap BitmapFromArray(this double[][] imgArray, int Width, int Height)
        //{
        //    Bitmap bmp = new Bitmap(Width, Height);
        //    int k = 0;
        //    for (int x = 0; x < bmp.Width; x++)
        //    {
        //        for (int y = 0; y < bmp.Height; y++)
        //        {
        //            Color c = Color.White;
        //            if (imgArray[0][k++] == 0)
        //                c = Color.Black;
        //            bmp.SetPixel(x, y, c);
        //        }
        //    }
        //    return bmp;
        //}

        #endregion

        #region Nested type: ComparerWithDuplicates

        public class ComparerWithDuplicates : IComparer<int>
        {
            #region IComparer<int> Members

            public int Compare(int x, int y)
            {
                if (x < y)
                {
                    return -1;
                }
                return 1;
            }

            #endregion
        }

        #endregion

        // Comparador de strings q segue o modelo do Windows (1,2,3...10,11,12,...100,101,102 ao invés de 1,10,100,1000...2,20,200)

        #region Nested type: NumericComparer

        public class NumericComparer : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                if ((x is string) && (y is string))
                {
                    return StringLogicalComparer.Compare((string)x, (string)y);
                }
                return -1;
            }

            #endregion
        }

        #endregion

        #region Nested type: TypeSwitch

        /// <summary>
        ///   TypeSwitch is designed to prevent redundant casting and give a syntax that is similar to a normal switch/case statement on types.
        /// </summary>
        public static class TypeSwitch
        {
            public static void Do(object source, params CaseInfo[] cases)
            {
                var type = source.GetType();
                foreach (var entry in cases.Where(entry => entry.IsDefault || entry.Target.IsAssignableFrom(type)))
                {
                    entry.Action(source);
                    break;
                }
            }

            public static CaseInfo Case<T>(Action action)
            {
                return new CaseInfo
                           {
                               Action = x => action(),
                               Target = typeof(T)
                           };
            }

            public static CaseInfo Case<T>(Action<T> action)
            {
                return new CaseInfo
                           {
                               Action = x => action((T)x),
                               Target = typeof(T)
                           };
            }

            public static CaseInfo Default(Action action)
            {
                return new CaseInfo
                           {
                               Action = x => action(),
                               IsDefault = true
                           };
            }

            #region Nested type: CaseInfo

            public class CaseInfo
            {
                public bool IsDefault { get; set; }
                public Type Target { get; set; }
                public Action<object> Action { get; set; }
            }

            #endregion
        }

        #endregion
    }
}