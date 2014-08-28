using System;
using System.Drawing;
using Core.Common;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Captchas
{
    /// <summary>
    ///   Implementação do Captcha do Sintegra de SP
    /// </summary>
    public class CaptchaSP : Captcha
    {
        public CaptchaSP(String fileName)
            : base(fileName)
        {
        }

        public CaptchaSP(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaSP(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 4; }
        }

        public override ImgArray[] GetCaracteresImgArray()
        {
            var separacao = new SeparacaoPadrao(this);
            return separacao.ColorFillingSegmentation2AndSeamCarving2();
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            var removerFundo = new RemocaoFundoPadrao(source);
            source = removerFundo.LetrasTemBrilhoMenorQue(90);
            return ColorUtils.PreencherPixels(source);
        }
    }
}