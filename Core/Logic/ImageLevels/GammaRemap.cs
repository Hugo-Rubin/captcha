using System;

namespace Core.Logic.ImageLevels
{
    public class GammaRemap
    {
        private readonly int[] remap;
        private int grayGamma; //where gray (127) is mapped to. 0-255  This is what sets the Gamma

        public GammaRemap()
        {
            remap = new int[256];
            for (var i = 0; i <= 255; i++)
                remap[i] = i;
            PshopGamma = 1.0;
            Gamma = 1.0;
            grayGamma = 127;
        }

        public int[] Remap
        {
            get { return remap; }
        }

        public double PshopGamma { get; private set; }

        public double Gamma { get; private set; }

        public double GrayGamma
        {
            set { ComputeRemap(value); }
        }

        //ctor


        //given the value of the gray input slider, compute the remap array
        private void ComputeRemap(double grayD)
        {
            //most common gray value, if its already been done, do nothing
            if ((int)Math.Floor(grayD) == 127 && grayGamma == 127)
                return;
            grayGamma = (int)Math.Floor(grayD);
            if (grayGamma <= 0) grayGamma = 1;
            Gamma = (Math.Log10(grayGamma / 255.0D)) / (Math.Log10(127 / 255.0D));
            PshopGamma = 1 / Gamma;

            for (var i = 0 + 1; i < 256; i++)
            {
                //compute brightness, input adjusted to normalize over black to white
                var inputBrightness = i / 255.0D;
                var outputBrightness = Math.Pow(inputBrightness, Gamma);
                remap[i] = (int)Math.Floor(outputBrightness * 255);
            }
        }

        //ComputeRemap()


        public void GammaAdjustRemapArray(int[] srcRemap)
        {
            for (var i = 0; i < 256; i++)
                srcRemap[i] = remap[srcRemap[i]];
        }
    }
}