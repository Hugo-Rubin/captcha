using System.Drawing;
using Core.Logic.ImageQuantizer.Ditherers;
using Core.Logic.ImageQuantizer.Helpers;
using Core.Logic.ImageQuantizer.Quantizers;

namespace Core.Logic
{
    public class PalleteQuantizer
    {
        // private ColorModel colorModel;
        //private IColorCache colorCache;
        private readonly IColorDitherer ditherer;
        private readonly int numberOfColors;
        private readonly int parallel;
        private readonly IColorQuantizer quantizer;
        private readonly Image sourceImage;
        private Image targetImage;
        //private ConcurrentDictionary<Color, Int64> errorCache;

        public PalleteQuantizer(Image img, IColorQuantizer quant, int numColors, IColorDitherer dit = null, int par = 0)
        {
            sourceImage = img;
            quantizer = quant;
            numberOfColors = numColors;
            ditherer = dit;
            parallel = par;
        }

        public Image ApplyFilter()
        {
            var parallelTaskCount = quantizer.AllowParallel ? parallel : 1;
            var colorCount = numberOfColors;
            targetImage = ImageBuffer.QuantizeImage(sourceImage, quantizer, ditherer, colorCount, parallelTaskCount);
            return targetImage;
        }
    }
}