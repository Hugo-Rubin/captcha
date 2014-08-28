using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    public class PredictCaptchaRJ : PredictNeuralNetwork<PredictCaptchaRJ>
    {
        protected override int HiddenUnits
        {
            get { return 261; }
        }

        protected override int TamanhoCaracter
        {
            get { return 3600; }
        }

        protected override char[] Dicionario
        {
            get
            {
                return new[]
                           {
                               '2', '3', '4', '5', '6', '7', '8', '9', 'b', 'c', 'd',
                               'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r',
                               's', 't', 'v', 'w', 'x', 'y', 'z'
                           };
            }
        }

        public override string SiglaServico
        {
            get { return "RJ"; }
        }
    }
}