using System.Drawing;
using PalleteQuantizer.Helpers;
using IColorQuantizer = PalleteQuantizer.Quantizers.IColorQuantizer;

namespace Core.Logic
{
    public class MyPalleteQuantizer
    {
        private readonly int numberOfColors;
        private readonly IColorQuantizer quantizer;
        private readonly Image sourceImage;
        private Image targetImage;

        public MyPalleteQuantizer(Image img, IColorQuantizer quant, int numColors)
        {
            sourceImage = img;
            quantizer = quant;
            numberOfColors = numColors;
        }

        public Image ApplyFilter()
        {
            const int parallelTaskCount = 1;
            var colorCount = numberOfColors;
            targetImage = ImageBuffer.QuantizeImage(sourceImage, quantizer, null, colorCount, parallelTaskCount);
            return targetImage;
        }
    }
}