using Core.Logic.Captchas.Abstract;
using Core.Logic.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Core.Logic.Utils;
using Core.Logic.Separacao;

namespace Core.Logic.Captchas
{
    public class CaptchaProjudiBA : Captcha
    {
        private const float MaximumLetterBrightness = 0.35F;

        public override int NumeroMinimoDeLetras
        {
            get { return 5; }
        }
        
        public CaptchaProjudiBA(String fileName) : base(fileName) {}

        public CaptchaProjudiBA(Bitmap bmpSource) : base(bmpSource) {}

        public CaptchaProjudiBA(NanoArray nanoArraySource) : base(nanoArraySource) {}

        public override IEnumerable<Types.ImgArray> GetCaracteres()
        {
            var cfs2 = new ColorFillingSegmentation2(this.ImgArray);
            var clusters = cfs2.GetCaracteres();
                        
            for (int itemNumber = 0; itemNumber < clusters.Count; itemNumber++)
            {
                int totalBlackPixels = clusters[itemNumber].CountPixelsWithColor(Color.Black);
                if (totalBlackPixels < NumeroMinimoDePixelsEmCluster)
                {
                    clusters.RemoveAt(itemNumber);
                }
            }

            return clusters.CortarECentralizarTodos(TamanhoImagemLetra.X, TamanhoImagemLetra.Y);
        }

        public override System.Drawing.Bitmap RemoverFundo(System.Drawing.Bitmap source)
        {
            var output = source.KeepPixelsWithMaximumBrightness(MaximumLetterBrightness);
            return output.PreencherPixels().ToBitmap();
        }
    }
}
