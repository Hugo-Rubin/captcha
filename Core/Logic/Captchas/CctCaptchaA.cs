using System;
using System.Drawing;
using Core.Common;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;

namespace Core.Logic.Captchas
{
    /// <summary>
    ///   Implementação do Captcha do Sistema de Consignações da Marinha
    /// </summary>
    public class CctCaptchaA : Captcha
    {
        public CctCaptchaA(String fileName)
            : base(fileName)
        {
        }

        public CctCaptchaA(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CctCaptchaA(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 4; }
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    source.SetPixel(x, y, ColorUtils.BrilhoDoPixel(source.GetPixel(x, y)) > 160 ? Color.White : Color.Black);
                }
            }

            return source;
        }

        public override ImgArray[] GetCaracteresImgArray()
        {
            throw new NotImplementedException();
        }
    }
}