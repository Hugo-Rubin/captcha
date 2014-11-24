using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;
using Core.Logic.Utils;
using Core.Logic.Filtros;
using System.Drawing.Imaging;
using Core.Common.Extensions;
using Core.Logic.RemocaoFundo;
using Core.Logic.Separacao;

namespace Core.Logic.Captchas
{
    public class CaptchaTRTSP : Captcha
    {
        public CaptchaTRTSP(String fileName)
            : base(fileName)
        {
        }

        public CaptchaTRTSP(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaTRTSP(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 5; }
        }

        public override Point TamanhoImagemLetra
        {
            get { return new Point(60, 60); }
        }


        public override IEnumerable<ImgArray> GetCaracteres()
        {
            SeparacaoPadrao sp = new SeparacaoPadrao(this);
            return sp.ColorFillingSegmentation2AndSeamCarving2(null, false).CortarECentralizarTodos(TamanhoImagemLetra.X, TamanhoImagemLetra.Y);
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            ImgArray img = new ImgArray(source.KeepGrayscale());
            
            Erosion er = new Erosion(img.InvertColors());
            return er.Result.InvertColors().RemoverRuidos(50).ToBitmap();
        }

    }
}