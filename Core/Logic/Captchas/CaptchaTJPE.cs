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
using Core.Logic.Tratamento;

namespace Core.Logic.Captchas
{
    public class CaptchaTJPE : Captcha
    {
        public CaptchaTJPE(String fileName)
            : base(fileName)
        {
        }

        public CaptchaTJPE(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaTJPE(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override int NumeroMinimoDeLetras
        {
            get { return 5; }
        }

        public override Point TamanhoImagemLetra
        {
            get { return new Point(65, 65); }
        }


        public override IEnumerable<ImgArray> GetCaracteres()
        {
            ColorFillingSegmentation2 cfs = new ColorFillingSegmentation2(this.ImgArray.RemoverMargem(2));
            var chars = cfs.GetCaracteres();
                        
            //SeparacaoPadrao sp = new SeparacaoPadrao(this);
            //var chars = sp.ColorFillingSegmentation2AndSeamCarving2().ToList<ImgArray>();

            for (int i = 0; i < chars.Count; i++)
            {
                    int pCount = chars[i].CountPixelsWithColor(Color.Black);
                    if (pCount < 25)
                    {
                        chars.RemoveAt(i);
                    }
            }

            return chars.CortarECentralizarTodos(TamanhoImagemLetra.X, TamanhoImagemLetra.Y);
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            ImgArray img = new ImgArray(source.KeepColorEqualOrLower(50, 50, 50));

            ForwardDerivative fd = new ForwardDerivative();
            fd.Apply(img, true, true);
            fd.Apply(img, true, false);

            Bitmap pbmp = img.PreencherPixels().RemoverRuidos(15).ToBitmap();
            return pbmp.ApplyMedianFilter(5);
        }

    }
}