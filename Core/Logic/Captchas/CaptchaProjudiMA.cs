using System.Collections.Generic;
using System.Drawing;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;

namespace Core.Logic.Captchas
{
    public class CaptchaProjudiMA : Captcha
    {
        public CaptchaProjudiMA(string fileName) : base(fileName){}

        public CaptchaProjudiMA(Bitmap bmpSource) : base(bmpSource){}

        public CaptchaProjudiMA(NanoArray nanoArraySource) : base(nanoArraySource){}

        public override int NumeroMinimoDeLetras
        {
            get { return 5; }
        }

        public override IEnumerable<ImgArray> GetCaracteres()
        {
            throw new System.NotImplementedException();
        }

        public override Bitmap RemoverFundo(Bitmap source)
        {
            throw new System.NotImplementedException();
        }
    }
}
