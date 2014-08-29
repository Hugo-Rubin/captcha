using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Captchas
{
    /// <summary>
    ///   Implementação do Captcha do Sintegra de MG
    /// </summary>
    public class CaptchaMG : Captcha
    {
        public CaptchaMG(String fileName)
            : base(fileName)
        {
        }

        public CaptchaMG(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaMG(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 5; }
        }

        public override IEnumerable<ImgArray> GetCaracteres()
        {
            var separacao = new SeparacaoPadrao(this);
            return separacao.ColorFillingSegmentation2AndSeamCarving2().PreencherPixelEmTodos();
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            var removerFundo = new RemocaoFundoPadrao(source);
            source = removerFundo.LetrasTemBrilhoMenorQue(129);
            return RemoverRuido(source); // Sem preencher pixels
            //return ColorUtils.PreencherPixels(RemoverRuido(clean)); // Com preencher pixels
        }

        public Bitmap RemoverRuido(Bitmap source)
        {
            var limparPixels = new[]
                                   {
                                       new Point(218, 66), new Point(217, 71), new Point(218, 71), new Point(218, 72),
                                       new Point(217, 73), new Point(218, 73), new Point(216, 74),
                                       new Point(217, 74), new Point(218, 74), new Point(216, 75), new Point(217, 75),
                                       new Point(218, 75), new Point(216, 76), new Point(217, 76),
                                       new Point(218, 76), new Point(217, 77), new Point(218, 77), new Point(215, 78),
                                       new Point(216, 78), new Point(217, 78), new Point(218, 78)
                                   };

            foreach (var p in limparPixels)
            {
                source.SetPixel(p.X, p.Y, Color.White);
            }

            return source;
        }
    }
}