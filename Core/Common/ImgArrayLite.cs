using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using Core.Common.Extensions;

namespace Core.Common
{
    /// <summary>
    ///   Classe que manipula a imagem em escala de cinza, representada por um vetor, onde:
    ///   ImgArray[0] = img.GetPixel(0,0);
    ///   ImgArray[1] = img.GetPixel(1,0);
    ///   ImgArray[ImgArray.Length-1] = img.GetPixel(img.Width, img.Height);
    /// </summary>
    public class ImgArrayLite : IEnumerator, IEnumerable
    {
        private readonly int height;
        private readonly byte[] imgArray;
        private readonly int length;
        private readonly int width;
        private int position = -1;

        public ImgArrayLite(Bitmap bmpSource)
        {
            width = bmpSource.Width;
            height = bmpSource.Height;
            length = width * height;
            imgArray = bmpSource.PixelIntensityByte();
        }

        public ImgArrayLite(ImgArrayLite imgSource)
        {
            width = imgSource.Width;
            height = imgSource.Height;
            length = width * height;
            imgArray = (byte[])imgSource.imgArray.Clone();
        }

        public ImgArrayLite(byte[] imgArraySource, int width, int height)
        {
            this.width = width;
            this.height = height;
            length = this.width * this.height;
            imgArray = imgArraySource;
        }

        public ImgArrayLite(int width, int height)
        {
            this.width = width;
            this.height = height;
            length = this.width * this.height;
            imgArray = new byte[length];
            imgArray.Init<byte>(1);
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int Length
        {
            get { return length; }
        }

        public byte this[int i]
        {
            get { return imgArray[i]; }
            set { imgArray[i] = value; }
        }

        #region Implementação de IEnumerator

        public object Current
        {
            get { return imgArray[position]; }
        }

        public bool MoveNext()
        {
            position++;
            return (position < imgArray.Length);
        }

        public void Reset()
        {
            position = 0;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion

        public ImgArrayLite Clone()
        {
            return new ImgArrayLite(this);
        }

        public Color GetPixel(int x, int y)
        {
            //TODO: O desempenho pode aumentar se ao invés de Color, retornarmos o byte mas altera o projeto todo
            // Está (X * Height + Y) e não (Y * Width + X) pois a gravação do vetor esta sendo feita em:
            // for (int x = 0; x < bmp.Width; x++)
            // {
            //     for (int y = 0; y < bmp.Height; y++)
            //     {

            var p = imgArray[x * Height + y];
            return Color.FromArgb(255, 255 * p, 255 * p, 255 * p);
        }

        public Color GetPixelXYInvertido(int x, int y)
        {
            //TODO: O desempenho pode aumentar se ao invés de Color, retornarmos o byte mas altera o projeto todo
            // Está (X * Height + Y) e não (Y * Width + X) pois a gravação do vetor esta sendo feita em:
            // for (int x = 0; x < bmp.Width; x++)
            // {
            //     for (int y = 0; y < bmp.Height; y++)
            //     {

            var p = imgArray[y * Width + x];
            return Color.FromArgb(255, 255 * p, 255 * p, 255 * p);
        }

        public byte GetByte(int x, int y)
        {
            return (byte)(GetPixel(x, y).IsBlackPixel() ? 0 : 1);
        }

        public void SetPixel(int x, int y, Color cor)
        {
            byte c = 1;
            if (cor.IsBlackPixel())
            {
                c = 0;
            }
            imgArray[x * height + y] = c;
        }

        public Bitmap ToBitmap()
        {
            var bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var c = Color.White;
                    // if (imgArray[k++] == 0)
                    if (GetPixel(x, y).IsBlackPixel())
                        c = Color.Black;

                    bmp.SetPixel(x, y, c);
                }
            }
            return bmp;
        }

        public Bitmap ToBitmap2()
        {
            var bmp = new Bitmap(Width, Height);

            for (var y = 0; y < bmp.Height; y++)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    var c = Color.White;
                    // if (imgArray[k++] == 0)
                    if (GetPixelXYInvertido(x, y).IsBlackPixel())
                    {
                        c = Color.Black;
                    }
                    bmp.SetPixel(x, y, c);
                }
            }
            return bmp;
        }

        public Bitmap ToBitmapGrayScale()
        {
            var bmp = new Bitmap(Width, Height);

            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var c = Color.White;
                    // if (imgArray[k++] == 0)
                    if (GetPixel(x, y).IsBlackPixel())
                        c = Color.Black;

                    bmp.SetPixel(x, y, c);
                }
            }
            return bmp;
        }

        public void Save(String fileName)
        {
            ToBitmap().Save(fileName);
        }

        public void Save2(String fileName)
        {
            ToBitmap2().Save(fileName);
        }

        public static ImgArrayLite CreateEmpty(int width, int height)
        {
            var imgArray = new byte[width * height];
            imgArray.Init<byte>(1);
            return new ImgArrayLite(imgArray, width, height);
        }

        /// <summary>
        ///   Remover ruidos da imagem original do captcha deixando apenas as letras pretas com fundo branco
        /// </summary>
        /// <param name="imagem"> </param>
        /// <returns> </returns>
        public static ImgArrayLite RemoverFundo(ImgArrayLite imagem)
        {
            var grayScale = imagem.ToBitmap().Clone(new Rectangle(0, 0, imagem.Width, imagem.Height),
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
                    if (imagem.GetPixel(x, y).IsBlackPixel())
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
            return new ImgArrayLite(grayScale);
        }

        public static ImgArrayLite LoadFromFile(String fileName)
        {
            var bmp = (Bitmap)Image.FromFile(fileName);
            return new ImgArrayLite(bmp);
        }

        /// <summary>
        ///   Limpa a imagem com pixel branco
        /// </summary>
        public void Clear()
        {
            for (var i = 0; i < imgArray.Length; i++)
            {
                imgArray[i] = 1;
            }
        }

        public void ZeroFill()
        {
            for (var i = 0; i < imgArray.Length; i++)
            {
                imgArray[i] = 0;
            }
        }
    }
}