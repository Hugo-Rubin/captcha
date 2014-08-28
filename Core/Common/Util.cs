using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Core.Common
{
    public static class Util
    {
        public static bool IsBlackPixel(this Color pixel)
        {
            return (pixel.R == 0) && (pixel.G == 0) && (pixel.B == 0);
        }

        public static bool IsWhitePixel(this Color pixel)
        {
            return (pixel.R == 255) && (pixel.G == 255) && (pixel.B == 255);
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

        public static double[,] PixelIntensityInvertido(this Bitmap img)
        {
            var m = new double[1, img.Height * img.Width];

            var k = 0;
            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    var c = img.GetPixel(x, y);
                    var luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    //m[0, k++] = (luminance > 127) ? 1 : 0;
                    m[0, k++] = luminance / 255;
                }
            }
            return m;
        }

        public static double[,] PixelIntensity(this Bitmap img)
        {
            var m = new double[1, img.Height * img.Width];

            var k = 0;
            for (var x = 0; x < img.Width; x++)
            {
                for (var y = 0; y < img.Height; y++)
                {
                    var c = img.GetPixel(x, y);
                    var luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    //m[0, k++] = (luminance > 127) ? 1 : 0;
                    m[0, k++] = luminance / 255;
                }
            }
            return m;
        }

        public static int[] PixelIntensityInt(this Bitmap img)
        {
            var m = new int[img.Height * img.Width];

            var k = 0;
            for (var x = 0; x < img.Width; x++)
            {
                for (var y = 0; y < img.Height; y++)
                {
                    var c = img.GetPixel(x, y);
                    var luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    //m[0, k++] = (luminance > 127) ? 1 : 0;
                    m[k++] = Convert.ToInt32(luminance / 255);
                }
            }
            return m;
        }

        public static byte[] PixelIntensityByte(this Bitmap img, bool grayscale = false)
        {
            var result = new byte[img.Height * img.Width];
            var controller = 0;

            if (grayscale)
            {
                controller = 1;
                result.Init((byte)255);
            }
            else
            {
                result.Init((byte)1);
            }

            /*Antigo método de leitura com Image.GetPixel 
            int k = 0;             
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Color c = img.GetPixel(x, y);
                    int luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    m[k++] = Convert.ToByte(luminance / 255);
                }
            }*/

            var isColored = IsColored(img);

            unsafe
            {
                if ((!isColored && controller == 1) || controller == 0)
                {
                    var bmd = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly,
                                                  img.PixelFormat);
                    var pixelSize = GetPixelSize(img.PixelFormat);

                    if (controller == 0)
                    {
                        for (var y = 0; y < bmd.Height; y++)
                        {
                            var row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                            for (var x = 0; x < bmd.Width; x++)
                            {
                                var idx = x * bmd.Height + y;
                                var pixel = row[(int)(x * pixelSize)];
                                var color = Convert.ToByte(pixel > 127 ? 1 : 0);
                                result[idx] = color;
                            }
                        }
                    }
                    else
                    {
                        for (var y = 0; y < bmd.Height; y++)
                        {
                            var row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                            for (var x = 0; x < bmd.Width; x++)
                            {
                                var idx = x * bmd.Height + y;
                                var pixel = row[(int)(x * pixelSize)];
                                var color = Convert.ToByte(pixel);
                                result[idx] = color;
                            }
                        }
                    }

                    img.UnlockBits(bmd);
                }
                else
                {
                    var gscale = new Bitmap(MakeGrayscale(img)).RemoveTransparency(Color.White);
                    var bmd = gscale.LockBits(new Rectangle(0, 0, gscale.Width, gscale.Height),
                                                     ImageLockMode.ReadOnly, gscale.PixelFormat);
                    var pixelSize = GetPixelSize(gscale.PixelFormat);

                    for (var y = 0; y < bmd.Height; y++)
                    {
                        var row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                        for (var x = 0; x < bmd.Width; x++)
                        {
                            var idx = x * bmd.Height + y;
                            var pixel = row[(int)(x * pixelSize)];
                            var color = Convert.ToByte(pixel);
                            result[idx] = color;
                        }
                    }

                    gscale.UnlockBits(bmd);
                }
            }

            return result;
        }

        public static int[,] SmallPixelIntensity(this Bitmap img)
        {
            var m = img.PixelIntensity();
            var lista = new List<int>();
            var countBrancos = 0;

            foreach (var corPixel in m)
            {
                if (corPixel > 0)
                {
                    // Contar pixels brancos seguidos
                    countBrancos++;
                }
                else
                {
                    if (countBrancos > 0)
                    {
                        // Se estava em uma sequencia de brancos
                        lista.Add(countBrancos);
                        countBrancos = 0;
                    }
                    // Adiciona o preto após a sequencia de brancos
                    lista.Add(0);
                }
            }
            if (countBrancos > 0)
            {
                // Adiciona os pixels brancos do fim da imagem
                lista.Add(countBrancos);
            }

            var result = new int[1, lista.Count];
            var array = lista.ToArray();
            for (var i = 0; i < array.Length; i++)
            {
                result[0, i] = array[i];
            }
            return result;
        }

        public static int[] NanoPixelIntensity(this Bitmap img)
        {
            var m = img.PixelIntensityInt();
            var lista = new List<int>();
            var countBrancos = 0;
            var countPretos = 0;

            foreach (var corPixel in m)
            {
                if (corPixel > 0)
                {
                    countBrancos++;

                    if (countPretos > 0)
                    {
                        // Se estava em uma sequencia de pretos
                        lista.Add(countPretos);
                        countPretos = 0;
                    }
                }
                else
                {
                    countPretos++;
                    if (countBrancos > 0)
                    {
                        // Se estava em uma sequencia de brancos
                        lista.Add(countBrancos);
                        countBrancos = 0;
                    }
                }
            }
            if (countBrancos > 0)
            {
                // Adiciona os pixels brancos do fim da imagem
                lista.Add(countBrancos);
            }
            if (countPretos > 0)
            {
                // Adiciona os pixels pretos do fim da imagem
                lista.Add(countPretos);
            }

            return lista.ToArray();
        }

        public static Bitmap MatrizParaBitmap(double[,] matriz)
        {
            var bmp = new Bitmap(200, 90);
            for (var i = 0; i < matriz.GetLength(1); i++)
            {
                var linha = i / 200;
                var c = matriz[0, i] == 0 ? Color.Black : Color.White;

                bmp.SetPixel(i % 200, linha, c);
            }
            return bmp;
        }

        public static Bitmap MatrizParaBitmap(this int[,] matriz, int imgWidth, int imgHeight)
        {
            var bmp = new Bitmap(imgWidth, imgHeight, PixelFormat.Format24bppRgb);

            for (var y = 0; y < matriz.GetLength(1); y++)
            {
                for (var x = 0; x < matriz.GetLength(0); x++)
                {
                    Color c;
                    var px = matriz[x, y];
                    switch (px)
                    {
                        case 0:
                            c = Color.Black;
                            break;
                        case 255:
                            c = Color.White;
                            break;
                        default:
                            c = Color.FromArgb(px, px, px);
                            break;
                    }

                    bmp.SetPixel(x, y, c);
                }
            }
            return bmp;
        }

        public static int[,] BitmapParaMatriz(this Bitmap bitmap)
        {
            var bmp = new int[bitmap.Width, bitmap.Height];

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    //TODO: Olha a gambis pra funcionar com escala de cinza
                    bmp[x, y] = bitmap.GetPixel(x, y).R % 30;
                }
            }
            return bmp;
        }

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

        public static Bitmap BitmapFromSmallArray(this int[][] imgSmallArray, int width, int height)
        {
            var lista = new List<double>();

            // Converter SmallArray em imgArray
            foreach (var item in imgSmallArray[0])
            {
                if (item == 0 || item == 1)
                {
                    lista.Add(item);
                }
                else
                {
                    for (var i = 0; i < item; i++)
                    {
                        lista.Add(1);
                    }
                }
            }

            var result = new double[1][];
            result[0] = new double[lista.Count];
            result[0] = lista.ToArray();

            return result.BitmapFromArray(width, height);
        }

        //public static Bitmap BitmapFromNanoArray(this int[] imgNanoArray, int Width, int Height)
        //{
        //    Bitmap bmp = new Bitmap(Width, Height);
        //    List<int> lista = new List<int>();

        //    // Converter SmallArray em imgArray
        //    foreach (var item in imgNanoArray)
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

        //    int[] result = lista.ToArray();

        //    return result.BitmapFromArray(Width, Height);

        //}

        public static Bitmap BitmapFromNanoArray(this int[] imgNanoArray, int width, int height)
        {
            var lista = new List<int>();
            var idx = 0;
            // Converter NanoArray em imgArray
            foreach (var item in imgNanoArray)
            {
                // Indice Par = Brancos
                if (idx % 2 == 0)
                {
                    for (var i = 0; i < item; i++)
                    {
                        lista.Add(1);
                    }
                }
                else
                {
                    //Indice impar = Pretos
                    for (var i = 0; i < item; i++)
                    {
                        lista.Add(0);
                    }
                }
                idx++;
            }

            var result = lista.ToArray();

            return result.BitmapFromArray(width, height);
        }

        public static Bitmap BitmapFromArray(this double[][] imgArray, int width, int height)
        {
            var bmp = new Bitmap(width, height);

            var k = 0;
            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var c = Color.White;

                    if (imgArray[0][k++] == 0)
                        c = Color.Black;

                    bmp.SetPixel(x, y, c);
                }
            }

            return bmp;
        }

        public static Bitmap BitmapFromArray(this int[] imgArray, int width, int height)
        {
            var bmp = new Bitmap(width, height);

            var k = 0;
            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var c = Color.White;

                    if (imgArray[k++] == 0)
                        c = Color.Black;

                    bmp.SetPixel(x, y, c);
                }
            }

            return bmp;
        }

        /// <summary>
        ///   Remove transparências na imagem, trocando os pixels transparentes por uma por passada pelo usuário ou mantendo as cores originais, apenas alterando alpha.
        /// </summary>
        /// <param name="img"> </param>
        /// <param name="setColor"> Cor que será pintada sobre os pixels transparentes, use Color.Empty para manter as cores originais dos pixels. </param>
        /// <returns> </returns>
        public static unsafe Bitmap RemoveTransparency(this Bitmap img, Color setColor)
        {
            var bitImage = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format32bppArgb);

            var bitdata = bitImage.LockBits(new Rectangle(0, 0, bitImage.Width, bitImage.Height),
                                                   ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            var pixelSize = GetPixelSize(bitImage.PixelFormat);

            var keepColors = setColor.Equals(Color.Empty);

            if (!keepColors)
            {
                for (var y = 0; y < bitdata.Height; y++)
                {
                    var row = (byte*)bitdata.Scan0 + (y * bitdata.Stride);

                    for (var x = 0; x < bitdata.Width; x++)
                    {
                        var alpha = row[(int)(x * pixelSize + 3)];

                        if (alpha != 255)
                        {
                            row[(int)(x * pixelSize)] = setColor.B; // B 
                            row[(int)(x * pixelSize + 1)] = setColor.G; // G 
                            row[(int)(x * pixelSize + 2)] = setColor.R; // R 
                            row[(int)(x * pixelSize + 3)] = 255; //A 
                        }
                    }
                }
            }
            else
            {
                for (var y = 0; y < bitdata.Height; y++)
                {
                    var row = (byte*)bitdata.Scan0 + (y * bitdata.Stride);

                    for (var x = 0; x < bitdata.Width; x++)
                    {
                        row[(int)(x * pixelSize + 3)] = 255; //A 
                    }
                }
            }

            bitImage.UnlockBits(bitdata);

            return bitImage;
        }

        //<summary>
        //Varre o bitmap e retorna o X do pixel preto mais próximo da
        //lateral esquerda da imagem
        //</summary>
        //<param name="bmp"></param>
        //<returns></returns>
        public static int GetMinX(this Bitmap bmp)
        {
            var x1 = -1;
            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var c = bmp.GetPixel(x, y);
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

        public static int GetMinX(this ImgArrayLite imgArrayLite)
        {
            var x1 = -1;
            for (var x = 0; x < imgArrayLite.Width; x++)
            {
                for (var y = 0; y < imgArrayLite.Height; y++)
                {
                    var c = imgArrayLite.GetPixel(x, y);
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

        //<summary>
        //Varre o bitmap e retorna o X do pixel preto mais próximo da
        //lateral direita da imagem
        //</summary>
        //<param name="bmp"></param>
        //<returns></returns>
        public static int GetMaxX(this Bitmap bmp)
        {
            var x2 = -1;
            for (var x = bmp.Width - 1; x > 0; x--)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var c = bmp.GetPixel(x, y);
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

        public static int GetMaxX(this ImgArrayLite imArrayLite)
        {
            var x2 = -1;
            for (var x = imArrayLite.Width - 1; x > 0; x--)
            {
                for (var y = 0; y < imArrayLite.Height; y++)
                {
                    var c = imArrayLite.GetPixel(x, y);
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

        //<summary>
        //Varre o bitmap e retorna o Y do pixel preto mais próximo da
        //parte superior da imagem
        //</summary>
        //<param name="bmp"></param>
        //<returns></returns>
        public static int GetMinY(this Bitmap bmp)
        {
            var y1 = -1;
            for (var y = 0; y < bmp.Height; y++)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    var c = bmp.GetPixel(x, y);
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


        public static int GetMinY(this ImgArrayLite imgArrayLite)
        {
            var y1 = -1;
            for (var y = 0; y < imgArrayLite.Height; y++)
            {
                for (var x = 0; x < imgArrayLite.Width; x++)
                {
                    var c = imgArrayLite.GetPixel(x, y);
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
        public static int GetMaxY(this Bitmap bmp)
        {
            var y2 = -1;
            for (var y = bmp.Height - 1; y > 0; y--)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    var c = bmp.GetPixel(x, y);
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

        public static int GetMaxY(this ImgArrayLite imgArrayLite)
        {
            var y2 = -1;
            for (var y = imgArrayLite.Height - 1; y > 0; y--)
            {
                for (var x = 0; x < imgArrayLite.Width; x++)
                {
                    var c = imgArrayLite.GetPixel(x, y);
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

        /// <summary>
        ///   Remover ruidos da imagem original do captcha deixando apenas as letras pretas com fundo branco
        /// </summary>
        /// <param name="imagem"> </param>
        /// <returns> </returns>
        public static Bitmap RemoverFundo(this Image imagem)
        {
            var grayScale = ((Bitmap)imagem).Clone(new Rectangle(0, 0, imagem.Width, imagem.Height),
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
                    if ((imagem as Bitmap).GetPixel(x, y).IsBlackPixel())
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

        /// <summary>
        ///   Remover fundo usado no projeto "CaptchaFinal". Está diferente do método acima.
        /// </summary>
        /// <param name="imagem"> </param>
        /// <returns> </returns>
        public static Bitmap RemoverFundoCF(this Image imagem)
        {
            var grayScale = (Bitmap)imagem.Clone();
            //Bitmap grayScale = bm.Clone(new Rectangle(0, 0, bm.Width, bm.Height), PixelFormat.Format32bppArgb);

            var quebra = false;

            for (var y = 0; y < grayScale.Height; y++)
            {
                for (var x = 0; x < grayScale.Width; x++)
                {
                    if (quebra)
                        break;

                    var c = grayScale.GetPixel(x, y);
                    var preto = Color.FromArgb(0, 0, 0);

                    if (!c.Equals(preto))
                        try
                        {
                            grayScale.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                        }
                        catch
                        {
                            //MessageBox.Show("O tipo da imagem é incompatível com o programa." + "\nCertifique-se de que a imagem está no formato .PNG" + "\n\nMensagem de erro:\n" + ioe.Message + "\n\nDetalhes:\n" + ioe.StackTrace + "\n\n" + "Método: " + ioe.TargetSite, "Formato Incorreto", MessageBoxButtons.OK);
                            quebra = true;
                        }
                }
            }
            return grayScale;
        }

        public static double[][] MatrizMultiplaParaVetorDeVetor(double[,] matrizMulti)
        {
            var qtdeVetores = matrizMulti.Length;
            var tamVetor = matrizMulti.GetLength(0);

            var result = new double[tamVetor][];

            for (var y = 0; y < tamVetor; y++)
            {
                result[y] = new double[qtdeVetores];
                for (var x = 0; x < qtdeVetores; x++)
                {
                    result[y][x] = matrizMulti[y, x];
                }
            }

            return result;
        }

        public static int[][] MatrizMultiplaParaVetorDeVetor(int[,] matrizMulti)
        {
            var qtdeVetores = matrizMulti.Length;
            var tamVetor = matrizMulti.GetLength(0);

            var result = new int[tamVetor][];

            for (var y = 0; y < tamVetor; y++)
            {
                result[y] = new int[qtdeVetores];
                for (var x = 0; x < qtdeVetores; x++)
                {
                    result[y][x] = matrizMulti[y, x];
                }
            }

            return result;
        }

        public static double[][] MatrizMultiplaParaVetorDeVetorInt(this int[,] matrizMulti)
        {
            var result = new double[matrizMulti.GetLength(1)][];
            for (var y = 0; y < matrizMulti.GetLength(1); y++)
            {
                result[y] = new double[matrizMulti.GetLength(0)];
                for (var x = 0; x < matrizMulti.GetLength(0); x++)
                {
                    result[y][x] = matrizMulti[x, y];
                }
            }
            return result;
        }

        public static unsafe Bitmap Otsu(this Bitmap img)
        {
            var histData = new int[256];

            var bitmapdata = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                                                 ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            const int pixelSize = 4;

            for (var y = 0; y < bitmapdata.Height; y++)
            {
                var destPixels = (byte*)bitmapdata.Scan0 + (y * bitmapdata.Stride);
                for (var x = 0; x < bitmapdata.Width; x++)
                {
                    var h = 0xFF & destPixels[x * pixelSize];
                    histData[h]++;
                }
            }
            img.UnlockBits(bitmapdata);

            // Total number of pixels
            var total = img.Width * img.Height;

            float sum = 0;

            for (var t = 0; t < 256; t++)
            {
                sum += t * histData[t];
            }

            float sumB = 0;
            var wB = 0;

            float varMax = 0;
            byte threshold = 0;

            for (byte t = 0; t <= (byte)255; t++)
            {
                wB += histData[t]; // Weight Background
                if (wB == 0) continue;

                var wF = total - wB;
                if (wF == 0) break;

                sumB += (t * histData[t]);

                var mB = sumB / wB; // Mean Background
                var mF = (sum - sumB) / wF; // Mean Foreground

                // Calculate Between Class Variance
                var varBetween = wB * (float)wF * (mB - mF) * (mB - mF);

                // Check if new maximum found
                if (varBetween > varMax)
                {
                    varMax = varBetween;
                    threshold = t;
                }
            }

            var bwImage = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);

            var bitdata = bwImage.LockBits(new Rectangle(0, 0, bwImage.Width, bwImage.Height),
                                                  ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var bitdataOriginal = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                                                      ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            for (var y = 0; y < bitdataOriginal.Height; y++)
            {
                var origPixels = (byte*)bitdataOriginal.Scan0 + (y * bitdataOriginal.Stride);
                var destPixels = (byte*)bitdata.Scan0 + (y * bitdata.Stride);
                for (var x = 0; x < bitdataOriginal.Width; x++)
                {
                    var color = ((0xFF & origPixels[x * pixelSize]) >= threshold) ? (byte)255 : (byte)0;
                    destPixels[x * pixelSize] = color; // B 
                    destPixels[x * pixelSize + 1] = color; // G 
                    destPixels[x * pixelSize + 2] = color; // R 
                    //destPixels[x * PixelSize + 3] = contrast_lookup[destPixels[x * PixelSize + 3]]; //A 
                }
            }
            bwImage.UnlockBits(bitmapdata);
            img.UnlockBits(bitdataOriginal);

            return bwImage;
        }

        public static Bitmap MakeGrayscale(this Bitmap original)
        {
            //create a blank bitmap the same size as original
            var newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            var g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            var colorMatrix = new ColorMatrix(
                new[]
                    {
                        new[] {.3f, .3f, .3f, 0, 0},
                        new[] {.59f, .59f, .59f, 0, 0},
                        new[] {.11f, .11f, .11f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });

            //create some image attributes
            var attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                        0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }

        public static void GravarLinhasEmArquivo(string filePath, List<string> lines, bool overrite)
        {
            if (string.IsNullOrEmpty(filePath))
                return;
            if (lines == null || lines.Count == 0)
                return;

            StreamWriter fileWriter = null;

            try
            {
                if (!overrite && File.Exists(filePath))
                {
                    fileWriter = File.AppendText(filePath);
                }
                else
                {
                    fileWriter = File.CreateText(filePath);
                }

                foreach (var line in lines)
                {
                    fileWriter.WriteLine(line);
                }
            }
            finally
            {
                if (fileWriter != null)
                {
                    fileWriter.Close();
                }
            }
        }

        public static byte[] ToByteArray(this Image image, ImageFormat imageFormat)
        {
            if (image == null)
                return null;

            var ms = new MemoryStream();
            image.Save(ms, imageFormat);
            return ms.ToArray();
        }

        public static double GetPixelSize(PixelFormat pf)
        {
            switch (pf)
            {
                case PixelFormat.Format16bppArgb1555:
                    return 2;
                case PixelFormat.Format16bppGrayScale:
                    return 2;
                case PixelFormat.Format16bppRgb555:
                    return 2;
                case PixelFormat.Format16bppRgb565:
                    return 2;
                case PixelFormat.Format1bppIndexed:
                    return 0.125;
                case PixelFormat.Format24bppRgb:
                    return 3;
                case PixelFormat.Format32bppArgb:
                    return 4;
                case PixelFormat.Format32bppPArgb:
                    return 4;
                case PixelFormat.Format32bppRgb:
                    return 4;
                case PixelFormat.Format48bppRgb:
                    return 6;
                case PixelFormat.Format4bppIndexed:
                    return 0.5;
                case PixelFormat.Format64bppArgb:
                    return 8;
                case PixelFormat.Format64bppPArgb:
                    return 8;
                case PixelFormat.Format8bppIndexed:
                    return 1;
                default:
                    return 1;
            }
        }

        private static unsafe bool IsColored(Image image)
        {
            using (var bmp = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(image, 0, 0);
                }

                var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                                               bmp.PixelFormat);

                var pt = (int*)data.Scan0;
                var res = true;

                for (var i = 0; i < data.Height * data.Width; i++)
                {
                    var color = Color.FromArgb(pt[i]);

                    if (color.A != 0 && (color.R != color.G || color.G != color.B))
                    {
                        res = false;
                        break;
                    }
                }

                bmp.UnlockBits(data);

                return !res;
            }
        }

        public static Image ToImage(this byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return (null);
            }

            return (Image.FromStream(new MemoryStream(byteArray)));
        }
    }
}