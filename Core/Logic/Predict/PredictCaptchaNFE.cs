using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    public class PredictCaptchaNFE : PredictNeuralNetwork<PredictCaptchaNFE>
    {
        protected override char[] Dicionario
        {
            get
            {
                return new[]
                           {
                               'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', '1', 'm',
                               'n', '2', 'p', 'q', 'r', 's', 't', 'u', 'w', 'v', 'x', 'y', 'z',
                               'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', '3', 'J', 'K', 'L', 'M',
                               'N', '4', 'P', 'Q', 'R', 'S', 'T', 'U', 'W', 'V', 'X', '~', 'Y',
                               'Z', '5', '6', '7', '8', '9'
                           };
            }
        }

        protected override int HiddenUnits
        {
            get { return 261; }
        }

        protected override int TamanhoCaracter
        {
            get { return 3600; }
        }
        
        public override string SiglaServico
        {
            get { return "NFE"; }
        }
    }
}