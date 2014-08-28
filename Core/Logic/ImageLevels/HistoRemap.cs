using System;

namespace Core.Logic.ImageLevels
{
    public class HistoRemap
    {
        private readonly int[] remap = new int[256]; //remap levels for r, g or b or luminance values

        public HistoRemap()
        {
            for (var i = 0; i <= 255; i++)
                remap[i] = i;
        }

        //ctor


        //provide indexer for external access of remap array
        //but it will be slow, so do everything in this class's methods
        public int this[int i]
        {
            get { return remap[i]; }
        }


        //compute remap array for input slider values
        //assume black<gray<white
        private void RemapInput(int black, int white)
        {
            for (var i = 0; i < black; i++)
                remap[i] = black; //make anything less than black, black
            for (var i = white + 1; i <= 255; i++)
                remap[i] = white; //make anything greater than white, white
        }

        //RemapInput()


        //compute remap array for output slider values
        private void RemapOuput(int blackI, int whiteI, int blackO, int whiteO)
        {
            var slope = (double)(whiteO - blackO) / (whiteI - blackI);

            for (var i = 0; i <= 255; i++)
            {
                var newValue = (int)Math.Floor(slope * (remap[i] - blackI) + blackO);
                if (newValue < blackO)
                    newValue = blackO;
                if (newValue > whiteO)
                    newValue = whiteO;
                remap[i] = newValue;
            }
        }

        //RemapOutput()


        public void RemapAll(int inputBlack, int inputGray, int inputWhite, int outputBlack, int outputWhite)
        {
            RemapInput(inputBlack, inputWhite);
            var g = new GammaRemap {GrayGamma = inputGray};
            g.GammaAdjustRemapArray(remap);
            RemapOuput(inputBlack, inputWhite, outputBlack, outputWhite);
        }

        //RemapAll()


        //given an input source array from a Histo Class and
        //the corresponding ouput target array.
        //Use the level values in remap[] to map the input array into the output array
        public void RemapHistoArray(int[] ihArray, int[] ohArray)
        {
            for (var i = 0; i < 256; i++)
                ohArray[i] = 0; //init output array
            //place input array counts for all levels into remapped levels in output array 
            for (var i = 0; i < 256; i++)
                ohArray[remap[i]] += ihArray[i];
        }

        //RemapHistoArray()


        //given an origRgbArray of unmodified pixel data, use the remap[] array
        //to modify it and place results in modRgbArray.
        public void RemapImageArray(byte[] origRgbArray, byte[] modRgbArray)
        {
            for (var i = 0; i < origRgbArray.Length; i += 4)
            {
                modRgbArray[i + (int)RGB.Red] = (byte)remap[origRgbArray[i + (int)RGB.Red]];
                modRgbArray[i + (int)RGB.Green] = (byte)remap[origRgbArray[i + (int)RGB.Green]];
                modRgbArray[i + (int)RGB.Blue] = (byte)remap[origRgbArray[i + (int)RGB.Blue]];
            }
        }

        #region Nested type: RGB

        private enum RGB
        {
            Blue,
            Green,
            Red
        };

        #endregion

        //RemapImageArray()
    }

    //class HistoRemap
}