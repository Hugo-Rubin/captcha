#region

using System;
using System.Drawing;

#endregion

namespace Core.Common
{
    public interface IImgArray
    {
        IImgArray Clone();
        Color GetPixel(int x, int y);
        Color GetPixelXyInvertido(int x, int y);
        byte GetByte(int x, int y);
        void SetPixel(int x, int y, Color cor);
        Bitmap ToBitmap();
        Bitmap ToBitmap2();
        Bitmap ToBitmapGrayScale();
        void Save(String fileName);
        void Save2(String fileName);
        IImgArray CreateEmpty(int width, int height);
        IImgArray RemoverFundo(IImgArray imagem);
        IImgArray LoadFromFile(String fileName);
        void Clear();
        void ZeroFill();
    }
}