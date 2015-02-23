using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Core.Logic.Types;

namespace Core.Logic.Utils
{
    public static class BitmapUtils
    {
        public static Image LoadImageWithoutLockFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            var ms = new MemoryStream(File.ReadAllBytes(path)); // Don't use using!!
            return Image.FromStream(ms);
        }

        public static Bitmap BitmapFundoBranco(int width, int height)
        {
            return new Bitmap(width > 0 ? width : 1, height > 0 ? height : 1).InserirFundoBranco();
        }

        public static void SalvarTodos(this IEnumerable<ImgArray> lista, string diretorio, string nome = "")
        {
            var i = 0;
            if (!File.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }
            foreach (var item in lista)
            {
                item.Save(String.Format(@"{0}\{1}{2}.png", diretorio, nome, i++));
            }
        }

        /// <summary>
        /// Retorna uma imagem com fundo branco e apenas copia para ela os pixels pretos da imagem de origem
        /// </summary>
        /// <param name="origem"> </param>
        /// <returns> </returns>
        public static ImgArray CopiarPixelsPretos(this Bitmap origem)
        {
            var destino = new ImgArray(origem.Width, origem.Height);

            for (var y = 0; y < origem.Height; y++)
            {
                for (var x = 0; x < origem.Width; x++)
                {
                    var c = origem.GetPixel(x, y);
                    if (c.IsBlackPixel())
                    {
                        destino.SetPixel(x, y, Color.Black);
                    }
                }
            }
            return destino;
        }

        /// <summary>
        /// Mantém apenas os pixels com a cor (RGB) passada por parâmetro na imagem, apagando todos os outros.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ImgArray KeepColor(this Bitmap origem, byte r, byte g, byte b)
        {
            var destino = new ImgArray(origem.Width, origem.Height);

            for (var y = 0; y < origem.Height; y++)
            {
                for (var x = 0; x < origem.Width; x++)
                {
                    var c = origem.GetPixel(x, y);
                    if (c.R == r && c.G == g && c.B == b)
                    {
                        destino.SetPixel(x, y, Color.Black);
                    }
                }
            }

            return destino;
        }

        /// <summary>
        /// Mantém apenas os pixels com a cor (RGB) passada por parâmetro na imagem, apagando todos os outros.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ImgArray KeepColor(this Bitmap origem, Color cor)
        {
            var destino = new ImgArray(origem.Width, origem.Height);

            for (var y = 0; y < origem.Height; y++)
            {
                for (var x = 0; x < origem.Width; x++)
                {
                    var c = origem.GetPixel(x, y);
                    if (c == cor)
                    {
                        destino.SetPixel(x, y, Color.Black);
                    }
                }
            }

            return destino;
        }

        /// <summary>
        /// Mantém apenas os pixels com as cores (RGB) passadas por parâmetro na imagem, apagando todos os outros.
        /// </summary>
        /// <param name="cores"></param>
        /// <returns></returns>
        public static ImgArray KeepColors(this Bitmap origem, List<Color> cores)
        {
            var destino = new ImgArray(origem.Width, origem.Height);
            
            for (var y = 0; y < origem.Height; y++)
            {
                for (var x = 0; x < origem.Width; x++)
                {
                    var c = origem.GetPixel(x, y);
                    if (cores.Contains(c))
                    {
                        destino.SetPixel(x, y, Color.Black);
                    }
                }
            }

            return destino;
        }

        /// <summary>
        /// Mantém apenas os pixels com as cores (RGB) passadas por parâmetro na imagem, apagando todos os outros.
        /// </summary>
        /// <param name="cores"></param>
        /// <returns></returns>
        public static Bitmap KeepColorsBMP(this Bitmap origem, List<Color> cores)
        {
            var destino = new Bitmap(origem.Width, origem.Height).InserirFundoBranco();

            for (var y = 0; y < origem.Height; y++)
            {
                for (var x = 0; x < origem.Width; x++)
                {
                    var c = origem.GetPixel(x, y);
                    if (cores.Contains(c))
                    {
                        destino.SetPixel(x, y, Color.Black);
                    }
                }
            }

            return destino;
        }


        /// <summary>
        /// Mantém apenas os pixels com a cor igual ou menor (RGB) que a passada por parâmetro na imagem, apagando todos os outros.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ImgArray KeepColorEqualOrLower(this Bitmap origem, byte r, byte g, byte b)
        {
            var destino = new ImgArray(origem.Width, origem.Height);

            for (var y = 0; y < origem.Height; y++)
            {
                for (var x = 0; x < origem.Width; x++)
                {
                    var c = origem.GetPixel(x, y);
                    if (c.R <= r && c.G <= g && c.B <= b)
                    {
                        destino.SetPixel(x, y, Color.Black);
                    }
                }
            }

            return destino;
        }
        
        /// <summary>
        /// Pinta os pixels em escala de cinza de preto, todos os outros de branco
        /// </summary>
        /// <param name="origem"></param>
        /// <returns></returns>
        public static ImgArray KeepGrayscale(this Bitmap origem)
        {
            var destino = new ImgArray(origem.Width, origem.Height);

            for (var y = 0; y < origem.Height; y++)
            {
                for (var x = 0; x < origem.Width; x++)
                {
                    var c = origem.GetPixel(x, y);
                    if (c.R == c.G && c.R == c.B)
                    {
                        destino.SetPixel(x, y, Color.Black);
                    }
                }
            }

            return destino;
        }
    }
}