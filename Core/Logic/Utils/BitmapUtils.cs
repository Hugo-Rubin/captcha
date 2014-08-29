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
        ///   Retorna uma imagem com fundo branco e apenas copia para ela os pixels pretos da imagem de origem
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
    }
}