namespace Core.Logic.ImageLevels
{
    /// <summary>
    ///   Interactions between the mail window and all the Custom Controls...
    ///   1.) input histograph
    ///   2.) input Slider
    ///   3.) output Histograph
    ///   4.) output slider
    /// </summary>
    public class CustomLevels
    {
        private readonly int[] input; //black, gray and white values for adjusting input
        private readonly int[] output; //black and white values for adjusting output

        private LevelValues lastLevels;

        public CustomLevels()
        {
            input = new int[3];
            output = new int[3];
        }

        public int[] GetValues()
        {
            return input;
        }

        public void ChangeValues(int[] inputValues)
        {
            ChangeValues(inputValues, new[] { 0, 255 });
        }

        public void ChangeValues()
        {
            if (input[0] == 0 && input[1] == 0 && input[2] == 0 && lastLevels == null) SetDefaultValues();
            var levels = new LevelValues(); //class to pack ALL slider values
            levels.SetLevelValues(input); //pack the class
            if (levels.LevelsEqual(lastLevels))
                return; //no change, do nothing
            lastLevels = levels; //save latest values
        }


        //All slider (TwoThumb or ThreeThumb) value change events come here.
        //If needed, repackage thumb values into a class and inform anyone registered for the event
        public void ChangeValues(int[] inputValuea, int[] outputValues)
        {
            var levels = new LevelValues(); //class to pack ALL slider values
            levels.SetLevelValues(inputValuea); //pack the class
            if (levels.LevelsEqual(lastLevels))
                return; //no change, do nothing
            lastLevels = levels; //save latest values
        }

        public void SetDefaultValues()
        {
            input[0] = 0;
            input[1] = 127;
            input[2] = 255;
            output[0] = 0;
            output[1] = 255;
        }
    }

    //class CustomLevels

    //class to return the value of all CustomLevel thumbs as ints
    public class LevelValues
    {
        public int InputBlack { get; private set; }

        public int InputGray { get; private set; }

        public int InputWhite { get; private set; }

        public int OutputBlack { get; private set; }

        public int OutputWhite { get; private set; }

        public void SetLevelValues(int[] inputs)
        {
            SetLevelValues(inputs, new[] { 0, 255 });
        }

        public void SetLevelValues(int[] inputs, int[] outputs)
        {
            InputBlack = inputs[0];
            InputGray = inputs[1];
            InputWhite = inputs[2];
            OutputBlack = outputs[0];
            OutputWhite = outputs[1];
        }

        //GetLevelValues


        public bool LevelsEqual(LevelValues levels)
        {
            if (levels == null) return false;
            if (InputBlack == levels.InputBlack && InputGray == levels.InputGray && InputWhite == levels.InputWhite &&
                OutputBlack == levels.OutputBlack && OutputWhite == levels.OutputWhite)
                return true;
            return false;
        }
    }

    //class LevelValues
}