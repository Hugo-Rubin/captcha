using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Core.Common.Extensions;
using Core.Logic.Utils;

namespace Core.Logic.Types
{
    /// <summary>
    ///   Classe que manipula a imagem em escala de cinza, representada por um vetor, onde:
    ///   ImgArrayGrayscale[0] = img.GetPixel(0,0);
    ///   ImgArrayGrayscale[1] = img.GetPixel(1,0);
    ///   ImgArrayGrayscale[ImgArrayGrayscale.Length-1] = img.GetPixel(img.Width, img.Height);
    /// </summary>
    public class ImgArrayGrayscale : IEnumerator, IEnumerable
    {
        private readonly byte[] imgArrayGrayscale;
        private readonly int length;
        private int height;
        private int position = -1;

        private int width;

        public ImgArrayGrayscale(Bitmap bmpSourceBlackAndWhite)
        {
            width = bmpSourceBlackAndWhite.Width;
            height = bmpSourceBlackAndWhite.Height;
            length = width * height;
            imgArrayGrayscale = bmpSourceBlackAndWhite.PixelIntensityByte(true);
        }

        public ImgArrayGrayscale(ImgArrayGrayscale imgSource)
        {
            width = imgSource.Width;
            height = imgSource.Height;
            length = width * height;
            imgArrayGrayscale = (byte[])imgSource.imgArrayGrayscale.Clone();
        }

        public ImgArrayGrayscale(byte[] imgArrayGrayscaleSource, int width, int height)
        {
            this.width = width;
            this.height = height;
            length = this.width * this.height;
            imgArrayGrayscale = imgArrayGrayscaleSource;
        }

        public ImgArrayGrayscale(int width, int height)
        {
            this.width = width;
            this.height = height;
            length = this.width * this.height;
            imgArrayGrayscale = new byte[length];
            imgArrayGrayscale.Init<byte>(255);
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
            get { return imgArrayGrayscale[i]; }
            set { imgArrayGrayscale[i] = value; }
        }

        #region Implementação de IEnumerator

        public object Current
        {
            get { return imgArrayGrayscale[position]; }
        }

        public bool MoveNext()
        {
            position++;
            return (position < imgArrayGrayscale.Length);
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

        public ImgArrayGrayscale Clone()
        {
            return new ImgArrayGrayscale(this);
        }

        public Color GetPixel(int x, int y)
        {
            // Está (X * Height + Y) e não (Y * Width + X) pois a gravação do vetor esta sendo feita em:
            // for (int x = 0; x < bmp.Width; x++)
            // {
            //     for (int y = 0; y < bmp.Height; y++)
            //     {
            byte p = 0;
            var idx = x * Height + y;
            if (idx < imgArrayGrayscale.Length)
            {
                p = imgArrayGrayscale[idx];
            }
            return Color.FromArgb(255, p, p, p);
        }

        public Color GetPixelXYInvertido(int x, int y)
        {
            //TODO: O desempenho pode aumentar se ao invés de Color, retornarmos o byte mas altera o projeto todo
            // Está (X * Height + Y) e não (Y * Width + X) pois a gravação do vetor esta sendo feita em:
            // for (int x = 0; x < bmp.Width; x++)
            // {
            //     for (int y = 0; y < bmp.Height; y++)
            //     {

            var p = imgArrayGrayscale[y * Width + x];
            return Color.FromArgb(255, p, p, p);
        }

        public byte GetByte(int x, int y)
        {
            return ByteFromColor(GetPixel(x, y));
        }

        public void SetPixel(int x, int y, Color cor)
        {
            var idx = x * height + y;
            if (idx < imgArrayGrayscale.Length)
            {
                var c = ByteFromColor(cor);
                imgArrayGrayscale[idx] = c;
            }
        }

        public Bitmap ToBitmap()
        {
            var bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb).InserirFundoBranco();

            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var cor = GetPixel(x, y).R;
                    if (cor != 255)
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(cor, cor, cor));
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
                    var cor = GetPixelXYInvertido(x, y).R;
                    if (cor != 255)
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(cor, cor, cor));
                    }
                }
            }
            return bmp;
        }

        public void Save(String fileName = "C:\\ImgArrayGrayscale.png")
        {
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

        [Obsolete(
            "Método não faz mais sentido pois o construtor padrão com X,Y já cria ImgArrayGrayscale com fundo branco")]
        public static ImgArrayGrayscale CreateEmpty(int width, int height)
        {
            var imgArrayGrayscale = new byte[width * height];
            imgArrayGrayscale.Init<byte>(255);
            return new ImgArrayGrayscale(imgArrayGrayscale, width, height);
        }

        /// <summary>
        ///   Remover ruidos da imagem original do captcha deixando apenas as letras pretas com fundo branco
        /// </summary>
        /// <param name="imagem"> </param>
        /// <returns> </returns>
        public static ImgArrayGrayscale RemoverFundo(ImgArrayGrayscale imagem)
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
                    var cor = imagem.GetPixel(x, y).R;
                    if (cor != 255)
                    {
                        try
                        {
                            pt.SetPixel(0, 0, Color.FromArgb(cor, cor, cor));
                            g.DrawImageUnscaled(pt, x, y);
                        }
                        catch
                        {
                            erro = true;
                        }
                    }
                }
            }
            return new ImgArrayGrayscale(grayScale);
        }

        public static ImgArrayGrayscale LoadFromFile(String fileName)
        {
            var bmp = (Bitmap)BitmapUtils.LoadImageWithoutLockFile(fileName);
            return new ImgArrayGrayscale(bmp);
        }

        /// <summary>
        ///   Limpa a imagem com pixel branco
        /// </summary>
        public void Clear()
        {
            for (var i = 0; i < imgArrayGrayscale.Length; i++)
            {
                imgArrayGrayscale[i] = 255;
            }
        }

        public void ZeroFill()
        {
            for (var i = 0; i < imgArrayGrayscale.Length; i++)
            {
                imgArrayGrayscale[i] = 0;
            }
        }

        public int CountPixelsWithColor(Color color)
        {
            return (from px in imgArrayGrayscale
                    where px.Equals(ByteFromColor(color))
                    select px).Count();
        }

        public void InvertColors()
        {
            for (var i = 0; i < imgArrayGrayscale.Length; i++)
            {
                imgArrayGrayscale[i] = (byte)(255 - imgArrayGrayscale[i]);
            }
            InvertedColors = !InvertedColors;
        }

        protected byte ByteFromColor(Color color)
        {
            return color.R;
        }

        public ImgArrayGrayscale GetSegment(Rectangle section)
        {
            var result = new ImgArrayGrayscale(section.Width, section.Height);
            for (var y = section.Y; y < section.Height + section.Y; y++)
            {
                for (var x = section.X; x < section.Width + section.X; x++)
                {
                    result.SetPixel(x - section.X, y - section.Y, GetPixel(x, y));
                }
            }

            return result;
        }

        public bool IsEquals(ImgArrayGrayscale img)
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