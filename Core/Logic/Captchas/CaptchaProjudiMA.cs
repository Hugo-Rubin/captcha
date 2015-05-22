using System.Collections.Generic;
using System.Drawing;
using Core.Common.Extensions;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;
using PalleteQuantizer.Helpers;
using PalleteQuantizer.Quantizers.XiaolinWu;

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
            var activeQuantizer = new WuColorQuantizer();
            const int parallelTaskCount = 1;

            source = (Bitmap)ImageBuffer.QuantizeImage(source, activeQuantizer, null, 30, parallelTaskCount);

            var pixels = new List<Color>();
            for (var x = source.Width - 1; x >= 0; x--)
            {
                pixels.Add(source.GetPixel(x, source.Height - 1));
                pixels.Add(source.GetPixel(x, source.Height - 2));
            }
            
            source = source.Where(
                    (p, x1, y1) =>
                        x1 > 10 
                        && pixels.Exists(item => item.RGBEquals(p)) == false
                        && p.IsBlackPixel() == false
            );

            
            
            source = (Bitmap) ImageBuffer.QuantizeImage(source, activeQuantizer, null, 2, parallelTaskCount);

            source.Save(@"C:\OCR\MATest1.png");
            return source;
        }
    }
}
