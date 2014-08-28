using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    public class PredictCaptchaAM : PredictNeuralNetwork<PredictCaptchaAM>
    {
        protected override int HiddenUnits
        {
            get { return 158; }
        }

        protected override int TamanhoCaracter
        {
            get { return 900; }
        }

        protected override char[] Dicionario
        {
            get
            {
                return new[]
                           {
                               '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c',
                               'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
                               'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
                           };
            }
        }

        public override string SiglaServico
        {
            get { return "AM"; }
        }
    }
}