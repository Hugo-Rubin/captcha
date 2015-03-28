using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

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
            throw new NotImplementedException();
        }

        public override System.Drawing.Bitmap RemoverFundo(System.Drawing.Bitmap source)
        {
            throw new NotImplementedException();
        }
    }
}
