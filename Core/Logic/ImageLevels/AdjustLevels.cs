using System.Collections.Generic;

namespace Core.Logic.ImageLevels
{
    public class AdjustLevels
    {
        private readonly CustomLevels customLevels;
        private readonly ImageWindow img; //image display, load and save

        private readonly byte[] modRgb;
        //modified levels adjusted rgb data goes here, inited when image loaded from file

        private readonly byte[] origRgb; //unmodified rgb data from a file

        private HistoRemap histoRemap; //represents image remapping specified by input and output sliders

        public AdjustLevels(string imgPath, IList<int> input, int[] output)
        {
            histoRemap = new HistoRemap();
            img = ImageWindow.Instance; //singleton
            customLevels = new CustomLevels();

            img.LoadFileImage(imgPath);
            origRgb = img.OrigRgb; //get unmodified rgb data from resized screen, NOT full screen rgb data
            modRgb = new byte[origRgb.Length];
            customLevels.SetDefaultValues();
            //causes immediate invocation of LevelsChanged(), set level controls back to defaults
            customLevels.ChangeValues();

            // Chamar SetLevels passando os níveis desejados
            var levels = new LevelValues();
            levels.SetLevelValues(customLevels.GetValues());
            ApplyOneLevelAtATime(levels, input, output);
            //levels.SetLevelValues(input, output);
            //SetLevels(levels);
        }

        private void ApplyOneLevelAtATime(LevelValues levels, IList<int> input, int[] output)
        {
            var shadows = (input[0] != levels.InputBlack);
            var midtones = (input[1] != levels.InputGray);
            var highlights = (input[2] != levels.InputWhite);

            if (shadows)
            {
                levels.SetLevelValues(new[] { input[0], levels.InputGray, levels.InputWhite }, output);
                SetLevels(levels);
            }
            if (midtones)
            {
                levels.SetLevelValues(new[] { levels.InputBlack, input[1], levels.InputWhite }, output);
                SetLevels(levels);
            }
            if (highlights)
            {
                levels.SetLevelValues(new[] { levels.InputBlack, levels.InputGray, input[2] }, output);
                SetLevels(levels);
            }

            img.SaveFileImage(@"C:\Users\Hugo\teste_raio5.jpg", modRgb);
        }

        public void SetLevels(LevelValues levels)
        {
            customLevels.ChangeValues(new[] { levels.InputBlack, levels.InputGray, levels.InputWhite });
            if (!img.ImgLoaded) return; //if image not yet available (need to load one)
            histoRemap = new HistoRemap(); //for level mapping of new slider values
            //using current slider values compute a level mapping
            histoRemap.RemapAll(levels.InputBlack, levels.InputGray, levels.InputWhite, levels.OutputBlack,
                                levels.OutputWhite);


            var bckgrndRemapImageArgs = new RemapImageArgs(origRgb, modRgb, histoRemap);
            BckgrndRemapImageArray(bckgrndRemapImageArgs);
            BckgrndRemapImageArrayCompleted();


            /*BackgroundWorker bckgrndWorker = new BackgroundWorker();
            bckgrndWorker.DoWork += BckgrndRemapImageArray;                         //work to do in background
            bckgrndWorker.RunWorkerCompleted += BckgrndRemapImageArrayCompleted;    //where to go when work finished
            RemapImageArgs bckgrndRemapImageArgs = new RemapImageArgs(origRgb, modRgb, histoRemap); //args to pass to background work
            bckgrndWorker.RunWorkerAsync(bckgrndRemapImageArgs);*/

            //img.CloseImage();
        }

        //private void BckgrndRemapImageArray(object sender, DoWorkEventArgs args)
        private void BckgrndRemapImageArray(RemapImageArgs ria)
        {
            var remapArgs = ria;
            var hr = remapArgs.Remap;
            hr.RemapImageArray(remapArgs.OrigRgb, remapArgs.ModRgb); //do work
            // args.Result = remapArgs.ModRgb;     //not used, but this is typically how its done
        }

        //BckgrndRemapImageArray()

        //private void BckgrndRemapImageArrayCompleted(object sender, RunWorkerCompletedEventArgs args)
        private void BckgrndRemapImageArrayCompleted()
        {
            img.UpdateImage(modRgb);
        }

        #region Nested type: RemapImageArgs

        public class RemapImageArgs
        {
            public RemapImageArgs(byte[] orig, byte[] mod, HistoRemap hr)
            {
                OrigRgb = orig;
                ModRgb = mod;
                Remap = hr;
            }

            public byte[] OrigRgb { get; set; }
            public byte[] ModRgb { get; set; }
            public HistoRemap Remap { get; set; }

            //ctor
        }

        #endregion
    }
}