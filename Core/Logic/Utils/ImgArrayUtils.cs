using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Core.Logic.Types;

namespace Core.Logic.Utils
{
    /// <summary>
    ///   Inclua nessa classe qualquer método de extensão que manipule ImgArray
    ///   A idéia é acabarmos com a ServerUtils quando tivermos os métodos organizados.
    /// </summary>
    public static class ImgArrayUtils
    {
        public static IEnumerable<ImgArray> CortarECentralizarTodos(this IEnumerable<ImgArray> src, int width, int height)
        {
            foreach (var item in src)
            {
                yield return item.CortarECentralizar(width, height);
            }
        }

        /// <summary>
        ///   Salva todos os bitmaps da lista no diretório informado. 
        ///   Cria diretório se for preciso.
        ///   Os arquivos serão Salvos no formato: {DIRETORIO}\{NOME}{INDICE_DA_IMAGEM).png
        /// </summary>
        /// <param name="lista"> Lista de imagens para salvar </param>
        /// <param name="diretorio"> Diretório para salvar as imagens </param>
        /// <param name="nome"> Nome opcional para as imagens (o indice da imagem sera incluido no final do nome) </param>
        public static int SalvarTodos(this List<ImgArray> lista, string diretorio, string nome = "")
        {
            var i = 0;
            if (!File.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }
            lista.RemoveAll(item => item == null);
            lista.ForEach(img => img.Save(String.Format(@"{0}\{1}{2}.png", diretorio, nome, i++)));
            return lista.Count;
        }

        public static int SalvarTodos(this ImgArray[] vetor, string diretorio, string nome = "")
        {
            return new List<ImgArray>(vetor).SalvarTodos(diretorio, nome);
        }

        public static ImgArray RemoveWhiteBorders(this ImgArray src, int margemX = 0, int margemY = 0)
        {
            if (src == null)
            {
                return new ImgArray(1, 1);
            }

            var x1 = src.GetMinX();
            var x2 = src.GetMaxX();
            var y1 = src.GetMinY();
            var y2 = src.GetMaxY();

            if (x1 < 0 || x2 < 0 || y1 < 0 || y2 < 0)
            {
                return src;
            }

            return src.CortarECentralizar(x2 - x1 + 1 + 2 * margemX, y2 - y1 + 1 + 2 * margemY);
        }

        public static List<ImgArray> RemoveWhiteBordersTodos(this List<ImgArray> lista, int margemX = 0, int margemY = 0)
        {
            var result = new List<ImgArray>();
            lista.ForEach(img => result.Add(img.RemoveWhiteBorders(margemX, margemY)));
            return result;
        }


        public static Bitmap[] ToBitmap(ImgArray[] vetor)
        {
            var result = new Bitmap[vetor.Length];
            var lista = new List<ImgArray>(vetor);
            var i = 0;
            lista.ForEach(img => result[i++] = img.ToBitmap());
            return result;
        }

        public static List<Bitmap> ToBitmap(List<ImgArray> lista)
        {
            var result = new List<Bitmap>();
            lista.ForEach(img => result.Add(img.ToBitmap()));
            return result;
        }

        public static double[,] PixelIntensity(this ImgArray img)
        {
            var m = new double[1, img.Height * img.Width];

            var k = 0;
            for (var x = 0; x < img.Width; x++)
            {
                for (var y = 0; y < img.Height; y++)
                {
                    var c = img.GetPixel(x, y);
                    var luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    m[0, k++] = luminance / 255;
                }
            }
            return m;
        }

        public static double[,] PixelIntensityParaRedeInvertida(this ImgArray img)
        {
            var m = new double[1, img.Height * img.Width];

            var k = 0;
            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    var c = img.GetPixel(x, y);
                    var luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);

                    m[0, k++] = luminance / 255;
                }
            }

            return m;
        }

        /// <summary>
        ///   Preenche os pixels no caso de imagem pontilhada
        /// </summary>
        /// <param name="imagem"> </param>
        /// <param name="amount"> Quantidade de vezes que o preenchimento deve ser aplicado </param>
        /// <returns> </returns>
        public static ImgArray PreencherPixels(this ImgArray imagem, int amount = 1)
        {
            //  ImgArray grayScale = ImgArray.RemoverFundo(Imagem);
            var grayScale = imagem;
            //Bitmap grayScale = (Bitmap) Imagem;
            var left = grayScale.GetPixel(0, 0);
            var right = grayScale.GetPixel(0, 0);
            var up = grayScale.GetPixel(0, 0);
            var down = grayScale.GetPixel(0, 0);
            bool test1 = false, test2 = false;
            var sentido = false;
            var count = 0;
            //Bitmap image = (Bitmap) pictureBox1.Image;

            try
            {
                do
                {
                    for (var y = 0; y < grayScale.Height; y++)
                    {
                        for (var x = 0; x < grayScale.Width; x++)
                        {
                            var c = grayScale.GetPixel(x, y);

                            if (x > 0 && x < grayScale.Width - 2)
                            {
                                left = grayScale.GetPixel(x - 1, y);
                                right = grayScale.GetPixel(x + 1, y);
                                test1 = true;
                            }
                            if (y > 0 && y < grayScale.Height - 4)
                            {
                                up = grayScale.GetPixel(x, y + 1);
                                down = grayScale.GetPixel(x, y - 1);
                                test2 = true;
                            }

                            if (c.IsBlackPixel())
                            {
                                if (test2)
                                {
                                    if (!up.IsBlackPixel() && grayScale.GetPixel(x, y + 2).IsBlackPixel() &&
                                        !grayScale.GetPixel(x, y + 4).IsBlackPixel())
                                        grayScale.SetPixel(x, y + 1, Color.FromArgb(0, 0, 0));
                                    if (y > 1 && !down.IsBlackPixel() && grayScale.GetPixel(x, y - 2).IsBlackPixel())
                                        grayScale.SetPixel(x, y - 1, Color.FromArgb(0, 0, 0));
                                }

                                if (test1)
                                {
                                    if (x > 1 && !left.IsBlackPixel() && grayScale.GetPixel(x - 2, y).IsBlackPixel())
                                        grayScale.SetPixel(x - 1, y, Color.FromArgb(0, 0, 0));
                                    if (!right.IsBlackPixel() && grayScale.GetPixel(x + 2, y).IsBlackPixel())
                                        grayScale.SetPixel(x + 1, y, Color.FromArgb(0, 0, 0));
                                }
                            }
                            test1 = false;
                            test2 = false;
                        }
                    }

                    // Pinta os pixels
                    for (var y = 0; y < grayScale.Height - 1; y++)
                    {
                        for (var x = 0; x < grayScale.Width - 1; x++)
                        {
                            var c = grayScale.GetPixel(x, y);

                            if (x > 0 && x < grayScale.Width - 1)
                            {
                                left = grayScale.GetPixel(x - 1, y);
                                right = grayScale.GetPixel(x + 1, y);
                                test1 = true;
                            }
                            if (y > 0 && y < grayScale.Height - 1)
                            {
                                up = grayScale.GetPixel(x, y + 1);
                                down = grayScale.GetPixel(x, y - 1);
                                test2 = true;
                            }

                            if (c.IsBlackPixel())
                            {
                                if (test2 && sentido)
                                {
                                    if (!down.IsBlackPixel() && up.IsBlackPixel())
                                        grayScale.SetPixel(x, y - 1, Color.FromArgb(0, 0, 0));
                                    sentido = false;
                                }
                                if (test1 && sentido == false)
                                {
                                    if (!left.IsBlackPixel() && right.IsBlackPixel())
                                        grayScale.SetPixel(x - 1, y, Color.FromArgb(0, 0, 0));
                                    sentido = true;
                                }
                            }
                            test1 = false;
                            test2 = false;
                        }
                    }
                    count++;
                } while (count < amount);
            }
            catch (ArgumentOutOfRangeException e)
            {
                ServerLog.AppendErrorLog(
                    String.Format("{0} \t {1} = {2} \t {3}", e.Message, e.ParamName, e.ActualValue, e.StackTrace),
                    imagem.ToBitmap());
            }
            catch (Exception e)
            {
                ServerLog.AppendErrorLog(e.Message, imagem.ToBitmap());
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return grayScale;
        }

        public static List<ImgArray> PreencherPixelEmTodos(this List<ImgArray> lista, int amount = 1)
        {
            var result = new List<ImgArray>();
            lista.RemoveAll(item => item == null);
            lista.ForEach(img => result.Add(img.PreencherPixels(amount)));
            return result;
        }

        public static ImgArray[] PreencherPixelEmTodos(this ImgArray[] vetor, int amount = 1)
        {
            return PreencherPixelEmTodos(new List<ImgArray>(vetor), amount).ToArray();
        }

        public static int[,] ToIntArray(this ImgArray img)
        {
            var matrix = new int[img.Width, img.Height];

            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    matrix[x, y] = img.GetPixel(x, y).IsBlackPixel() ? 1 : 0;
                }
            }

            return matrix;
        }

        public static ImgArray ToImgArray(this int[,] matrix)
        {
            var img = new ImgArray(matrix.GetLength(0), matrix.GetLength(1));

            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    if (matrix[x, y] == 1)
                        img.SetPixel(x, y, Color.Black);
                }
            }

            return img;
        }

        /// <summary>
        ///   Pinta de branco qualquer desenho preto que seja menor ou igual ao tamanho do ruído informado
        /// </summary>
        /// <param name="imgArray"> </param>
        /// <param name="tamanhoRuido"> </param>
        public static ImgArray RemoverRuidos(this ImgArray imgArray, int tamanhoRuido)
        {
            var x = 0;
            var y = 0;
            var idx = 0;

            var pixelsProcessados = new List<Point>();
            while (idx < imgArray.Width * imgArray.Height - 1)
            {
                var pixel = imgArray.GetPixel(x, y);
                // Procura proximo pixel preto
                while (!pixel.IsBlackPixel()
                       && idx < imgArray.Width * imgArray.Height - 1)
                {
                    idx++;
                    x = idx % imgArray.Width;
                    y = (int)Math.Floor((decimal)(idx / imgArray.Width));
                    pixel = imgArray.GetPixel(x, y);
                }
                var pointPixel = new Point(x, y);
                var fim = idx == imgArray.Width * imgArray.Height - 1;
                var sair = fim && !imgArray.GetPixel(x, y).IsBlackPixel();

                if (!sair && !pixelsProcessados.Contains(pointPixel))
                {
                    var clusterPixels = imgArray.GetCluster(pointPixel);
                    pixelsProcessados.AddRange(clusterPixels);

                    if (clusterPixels.Count() < tamanhoRuido)
                    {
                        foreach (var px in clusterPixels)
                        {
                            imgArray.SetPixel(px.X, px.Y, Color.White);
                        }
                    }
                }

                idx++;

                x = idx % imgArray.Width;
                y = (int)Math.Floor((decimal)(idx / imgArray.Width));
            }
            return imgArray;
        }

        public static int GetNumberOfBlackPixelsRow(this ImgArray img, int row)
        {
            var nPixels = 0;
            for (var x = 0; x < img.Width; x++)
            {
                if (img.GetPixel(x, row).IsBlackPixel())
                {
                    nPixels++;
                }
            }

            return nPixels;
        }

        public static int GetNumberOfBlackPixelsCol(this ImgArray img, int col)
        {
            var nPixels = 0;
            for (var y = 0; y < img.Height; y++)
            {
                if (img.GetPixel(col, y).IsBlackPixel())
                {
                    nPixels++;
                }
            }

            return nPixels;
        }

        public static ImgArray InvertHorizontally(this ImgArray img)
        {
            var inverted = new ImgArray(img.Width, img.Height);

            for (var y = 0; y < img.Height; y++)
            {
                for (var x = img.Width - 1; x >= 0; x--)
                {
                    inverted.SetPixel(img.Width - x, y, img.GetPixel(x, y));
                }
            }

            return inverted;
        }

        public static ImgArray Flip(this ImgArray img, RotateFlipType rotateType)
        {
            var rotatedBmp = img.ToBitmap();
            rotatedBmp.RotateFlip(rotateType);

            return new ImgArray(rotatedBmp);
        }

        public static ImgArray RemoverMargem(this ImgArray img, int tam = 1)
        {
            if (img.GetPixel(0, 0) == Color.White || img.GetPixel(img.Width - 1, img.Height - 1) == Color.White)
            {
                return img;
            }

            int y = 0;
            int x = 0;

            while(y < tam)
            {
                for (x = 0; x < img.Width; x++)
                {
                    img.SetPixel(x, y, Color.White);
                }
                y++;
            }

            y = 0;
            while (y < tam)
            {
                for (x = 0; x < img.Width; x++)
                {
                    img.SetPixel(x, img.Height - 1 - y, Color.White);
                }
                y++;
            }

            x = 0;
            while(x < tam)
            {
                for (y = 0; y < img.Height; y++)
                {
                    img.SetPixel(x, y, Color.White);
                }
                x++;
            }

            x = 0;
            while (x < tam)
            {
                for (y = 0; y < img.Height; y++)
                {
                    img.SetPixel(img.Width - 1 - x, y, Color.White);
                }
                x++;
            }

            return img;
        }

        public static ImgArray PintarPixelsAbaixo(this ImgArray imgArray, Point p1, int altura, Color cor)
        {
            var alturaFinal = p1.Y + altura - 1;
            while (p1.Y <= alturaFinal)
            {
                imgArray.SetPixel(p1.X, p1.Y, cor);
                p1.Y++;
            }
            return imgArray;
        }

        public static ImgArray ApagarMargem(this ImgArray img, Byte cor = 255)
        {
            for (int y = 0; y < img.Height; y += img.Height - 1)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    img.SetPixel(x, y, Color.FromArgb(cor, cor, cor));
                }
            }

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x += img.Width - 1)
                {
                    img.SetPixel(x, y, Color.FromArgb(cor, cor, cor));
                }
            }

            return img;
        }

        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        public static List<Point> ToList(this ImgArray img)
        {
            List<Point> pixelsPretos = new List<Point>();

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    if (img.GetPixel(x, y).IsBlackPixel())
                    {
                        pixelsPretos.Add(new Point(x, y));
                    }
                }
            }
            
            return pixelsPretos;
        }

    }
}