using Core.Logic.Captchas.Abstract;
using Core.Logic.Separacao;
using Core.Logic.Tratamento;
using Core.Logic.Types;
using Core.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Core.Logic.Captchas
{
    public class CaptchaProjudiGeral : Captcha
    {
        private const float MaximumLetterBrightness = 0.15F;

        public override int NumeroMinimoDeLetras
        {
            get { return 5; }
        }

        public CaptchaProjudiGeral(String fileName) : base(fileName) { }

        public CaptchaProjudiGeral(Bitmap bmpSource) : base(bmpSource) { }

        public CaptchaProjudiGeral(NanoArray nanoArraySource) : base(nanoArraySource) { }

        public override IEnumerable<ImgArray> GetCaracteres()
        {
            var cfs2 = new ColorFillingSegmentation2(ImgArray, 8, 2, true, true, 120);
            var sp = new SeparacaoPadrao(this, cfs2);

            return sp.ColorFillingSegmentation2AndCorteCego(21);
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            var output = source
                .KeepPixelsWithMaximumBrightness(MaximumLetterBrightness)
                .RemoverMargem(2);
                     
            ForwardDerivative fd = new ForwardDerivative();
            fd.Apply(output, true, true);
            fd.Apply(output, true, true);
            fd.Apply(output, true, false);

            return output
                .RemoverRuidos(30)
                .PreencherPixels()
                .CortarECentralizar(output.Width, output.Height)
                .ToBitmap();
                //.ApplyMedianFilter(5);
        }
    }
}
