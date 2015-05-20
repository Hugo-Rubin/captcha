using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Logic.Types;

namespace Core.Logic.Captchas.Abstract
{
    public abstract class Captcha
    {
        /// <summary>
        /// Id do Ocr no banco de dados para gravação de logs
        /// </summary>
        public virtual int OcrId
        {
            get { return 1; }
        }

        protected Captcha(String fileName)
        {
            Init();
            LoadFromFile(fileName);
        }

        protected Captcha(Bitmap bmpSource)
        {
            Init();
            ImageLoaded(ref bmpSource);
            var bmp = RemoverFundo(bmpSource);
            LoadFromBitmap(bmp);
        }

        protected Captcha(NanoArray nanoArraySource)
        {
            Init();
            LoadFromNanoArray(nanoArraySource);
        }

        public ImgArray ImgArray { get; private set; }

        public int Width
        {
            get { return ImgArray.Width; }
        }

        public int Height
        {
            get { return ImgArray.Height; }
        }

        public abstract int NumeroMinimoDeLetras { get; }

        public virtual int NumeroMinimoDePixelsEmCluster
        {
            get { return 50; }
        }

        /// <summary>
        ///   Indica o tamanho da imagem de um caracter, usado para treinar a rede.
        ///   Se não for sobrescrito terá o tamanho padrão de 60 x 60
        /// </summary>
        public virtual Point TamanhoImagemLetra
        {
            get { return new Point(60, 60); }
        }

        public string PadraoIdentificado { get; protected set; }

        public void LoadFromFile(String bitmapFileName)
        {
            var bmpSource = (Bitmap)Image.FromFile(bitmapFileName);
            ImageLoaded(ref bmpSource);
            var bmp = RemoverFundo(bmpSource);
            if (null != bmp)
            {
                LoadFromBitmap(bmp);
            }
        }

        public void LoadFromBitmap(Bitmap bmpSource)
        {
            ImgArray = new ImgArray(bmpSource);
        }

        protected virtual void ImageLoaded(ref Bitmap bmpSource)
        {
        }

        public void LoadFromNanoArray(NanoArray nanoArraySource)
        {
            ImgArray = new ImgArray(nanoArraySource);
        }

        public NanoArray ToNanoArray()
        {
            return ImgArray.ToNanoArray();
        }
     
        public abstract IEnumerable<ImgArray> GetCaracteres();

        public abstract Bitmap RemoverFundo(Bitmap source);

        public virtual void Save(String fileName = "C:\\Captcha.png")
        {
            ImgArray.Save(fileName);
        }

        protected virtual void Init()
        {
        }
    }
}