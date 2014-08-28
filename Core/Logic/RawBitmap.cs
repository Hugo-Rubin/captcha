using System;
using System.Drawing;
using System.Drawing.Imaging;
using Core.Logic.Types;

namespace Core.Logic
{
    //TODO: Criar essa classe usando ImgArray porque não devemos mais usar Bitmap
    [Obsolete("Deveria usar código equivalente usando ImgArray")]
    public unsafe class RawBitmap : IDisposable
    {
        private readonly byte* begin;
        private readonly BitmapData bitmapData;
        private readonly Bitmap originBitmap;

        public RawBitmap(Bitmap originBitmap)
        {
            this.originBitmap = originBitmap;
            bitmapData = this.originBitmap.LockBits(new Rectangle(0, 0, this.originBitmap.Width, this.originBitmap.Height),
                                                 ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            begin = (byte*)(void*)bitmapData.Scan0;
        }

        public RawBitmap(ImgArray originImgArray)
        {
            originBitmap = originImgArray.ToBitmap();
            bitmapData = originBitmap.LockBits(new Rectangle(0, 0, originBitmap.Width, originBitmap.Height),
                                                 ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            begin = (byte*)(void*)bitmapData.Scan0;
        }

        public byte* Begin
        {
            get { return begin; }
        }

        public byte* this[int x, int y]
        {
            get { return begin + y * (bitmapData.Stride) + x * 3; }
        }

        public byte* this[int x, int y, int offset]
        {
            get { return begin + y * (bitmapData.Stride) + x * 3 + offset; }
        }

        //public unsafe void SetColor(int x, int y, int color)
        //{
        //    *(int*)(_begin + y * (_bitmapData.Stride) + x * 3) = color;
        //}

        public int Stride
        {
            get { return bitmapData.Stride; }
        }

        public int Width
        {
            get { return bitmapData.Width; }
        }

        public int Height
        {
            get { return bitmapData.Height; }
        }

        public Bitmap OriginBitmap
        {
            get { return originBitmap; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            originBitmap.UnlockBits(bitmapData);
        }

        #endregion

        public int GetOffset()
        {
            return bitmapData.Stride - bitmapData.Width * 3;
        }
    }
}