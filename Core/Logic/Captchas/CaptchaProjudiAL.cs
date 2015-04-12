using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Core.Logic.Utils;
using Core.Logic.Separacao;

namespace Core.Logic.Captchas
{
    public class CaptchaProjudiAL : Captcha
    {
        public override int NumeroMinimoDeLetras
        {
            get { return 4; }
        }

        public CaptchaProjudiAL(String fileName)
            : base(fileName)
        {
        }

        public CaptchaProjudiAL(Bitmap bmpSource)
            : base(bmpSource)
        {
        }

        public CaptchaProjudiAL(NanoArray nanoArraySource)
            : base(nanoArraySource)
        {
        }

        public override IEnumerable<Types.ImgArray> GetCaracteres()
        {
            var cfs = new ColorFillingSegmentation2(this.ImgArray, 8, 1);
            var chars = cfs.GetCaracteres();

            return chars.CortarECentralizarTodos(TamanhoImagemLetra.X, TamanhoImagemLetra.Y);
        }

        public override System.Drawing.Bitmap RemoverFundo(System.Drawing.Bitmap source)
        {

            Func<Color, Boolean> condition = (Color c) => {
                if (c.R > 160 && c.G > 235 && c.B > 235)
                {
                    return true;
                } 
                return false; 
            };

            var output = source.KeepColors(condition);

            return output.ToBitmap();
            
        }
    }
}
