using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Core.Common.Extensions;
using Core.Logic.Utils;

namespace Core.Logic.Types
{
    /// <summary>
    ///   Classe que manipula a imagem em escala de cinza, representada por um vetor, onde:
    ///   ImgArray[0] = img.GetPixel(0,0);
    ///   ImgArray[1] = img.GetPixel(1,0);
    ///   ImgArray[ImgArray.Length-1] = img.GetPixel(img.Width, img.Height);
    /// </summary>
    public class ImgArray : IEnumerator, IEnumerable
    {
        private readonly byte[] imgArray;
        private readonly int length;
        private int height;
        private int position = -1;

        private int width;

        public ImgArray(Bitmap bmpSourceBlackAndWhite)
        {
            width = bmpSourceBlackAndWhite.Width;
            height = bmpSourceBlackAndWhite.Height;
            length = width * height;
            imgArray = bmpSourceBlackAndWhite.PixelIntensityByte();
        }

        public ImgArray(Bitmap bmpSourceBlackAndWhite, bool isGrayscale, byte threshold)
        {
            width = bmpSourceBlackAndWhite.Width;
            height = bmpSourceBlackAndWhite.Height;
            length = width * height;
            imgArray = bmpSourceBlackAndWhite.PixelIntensityByte(isGrayscale, threshold);
        }

        public ImgArray(ImgArray imgSource)
        {
            width = imgSource.Width;
            height = imgSource.Height;
            length = width * height;
            imgArray = (byte[])imgSource.imgArray.Clone();
        }

        public ImgArray(NanoArray nanoArraySource)
        {
            width = nanoArraySource.Width;
            height = nanoArraySource.Height;
            length = width * height;
            imgArray = FromNanoArray(nanoArraySource).imgArray;
        }

        public ImgArray(byte[] imgArraySource, int width, int height)
        {
            this.width = width;
            this.height = height;
            length = this.width * this.height;
            imgArray = imgArraySource;
        }

        public ImgArray(int width, int height)
        {
            this.width = width;
            this.height = height;
            length = this.width * this.height;
            imgArray = new byte[length];
            imgArray.Init<byte>(1);
        }

        public ImgArray(List<Point> blackPixels, int width, int height)
        {
            this.width = width;
            this.height = height;
            length = width * height;
            imgArray = new byte[length];
            imgArray.Init<byte>(1);

            foreach (var pixel in blackPixels)
            {
                var idx = pixel.X * height + pixel.Y;
                if (idx < imgArray.Length)
                {
                    imgArray[idx] = 0;
                }
            }
        }

        public int Width
        {
            get { return width; }
            set {
                width = value < 1 ? 1 : value;
            }
        }

        public int Height
        {
            get { return height; }
            set {
                height = value < 1 ? 1 : value;
            }
        }

        public int Length
        {
            get { return length; }
        }

        public bool InvertedColors { get; private set; }

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

        public static ImgArray FromNanoArray(NanoArray imgNanoArray)
        {
            var lista = new List<byte>();
            var idx = 0;
            // Converter NanoArray em imgArray
            foreach (int item in imgNanoArray)
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

            return new ImgArray(lista.ToArray(), imgNanoArray.Width, imgNanoArray.Height);
        }

        public ImgArray Clone()
        {
            return new ImgArray(this);
        }

        public Color GetPixel(int x, int y)
        {
            // Está (X * Height + Y) e não (Y * Width + X) pois a gravação do vetor esta sendo feita em:
            // for (int x = 0; x < bmp.Width; x++)
            // {
            //     for (int y = 0; y < bmp.Height; y++)
            //     {
            if (x < 0 || y < 0 || x >= width || y >= height)
            {
                return Color.White;
            }

            byte p = 1;
            var idx = x * Height + y;
            if (idx < imgArray.Length)
            {
                p = imgArray[idx];
            }
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
            return ByteFromColor(GetPixel(x, y));
        }

        public void SetPixel(int x, int y, Color cor)
        {
            var idx = x * height + y;
            if (idx < imgArray.Length && idx > 0)
            {
                var c = ByteFromColor(cor);
                imgArray[idx] = c;
            }
        }

        public Bitmap ToBitmap()
        {
            var bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb).InserirFundoBranco();

            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    if (GetPixel(x, y).IsBlackPixel())
                    {
                        bmp.SetPixel(x, y, Color.Black);
                    }
                }
            }
            return bmp;
        }

        [Obsolete("Mantido apenas porque a rede do CaptchaTipo1 foi criada em formato invertido")]
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

        public void Save(String fileName = "C:\\ImgArray.png")
        {
            if (fileName == null)
            {
                throw new ArgumentNullException();
            }

            var dir = Path.GetDirectoryName(fileName);
            if (dir == null
                || Directory.Exists(dir) == false)
            {
                throw new DirectoryNotFoundException();
            }
            
            ToBitmap().Save(fileName, ImageFormat.Png);
        }

        public void SaveWithAleatory(String fileDir)
        {
            Save(String.Format(@"{0}\{1}.png", fileDir, DateTime.Now.Millisecond));
        }

        [Obsolete("Mantido apenas porque a rede do CaptchaTipo1 foi criada em formato invertido")]
        public void Save2(String fileName)
        {
            ToBitmap2().Save(fileName);
        }

        [Obsolete("Método não faz mais sentido pois o construtor padrão com X,Y já cria ImgArray com fundo branco")]
        public static ImgArray CreateEmpty(int width, int height)
        {
            var imgArray = new byte[width * height];
            imgArray.Init<byte>(1);
            return new ImgArray(imgArray, width, height);
        }

        //TODO: Dá pra melhorar
        public NanoArray ToNanoArray()
        {
            return new NanoArray(ToBitmap());
        }

        /// <summary>
        ///   Remover ruidos da imagem original do captcha deixando apenas as letras pretas com fundo branco
        /// </summary>
        /// <param name="imagem"> </param>
        /// <returns> </returns>
        public static ImgArray RemoverFundo(ImgArray imagem)
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
            return new ImgArray(grayScale);
        }

        public static ImgArray LoadFromFile(String fileName)
        {
            var bmp = (Bitmap)BitmapUtils.LoadImageWithoutLockFile(fileName);
            return new ImgArray(bmp);
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

        public int CountPixelsWithColor(Color color)
        {
            return (from px in imgArray
                    where px.Equals(ByteFromColor(color))
                    select px).Count();
        }

        public ImgArray InvertColors()
        {
            for (var i = 0; i < imgArray.Length; i++)
            {
                if (imgArray[i] == 0)
                {
                    imgArray[i] = 1;
                }
                else
                {
                    imgArray[i] = 0;
                }
            }
            InvertedColors = !InvertedColors;

            return new ImgArray(this);
        }

        protected byte ByteFromColor(Color color)
        {
            byte result = 1;
            if (color.IsBlackPixel())
            {
                result = 0;
            }
            return result;
        }

        public ImgArray GetSegment(Rectangle section)
        {
            var result = new ImgArray(section.Width, section.Height);
            for (var y = section.Y; y < section.Height + section.Y; y++)
            {
                for (var x = section.X; x < section.Width + section.X; x++)
                {
                    result.SetPixel(x - section.X, y - section.Y, GetPixel(x, y));
                }
            }

            return result;
        }

        public bool IsEqual(ImgArray img)
        {
            if (img.Width != Width || img.Height != Height)
            {
                return false;
            }
            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    if (img.GetPixel(x, y) != GetPixel(x, y))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}