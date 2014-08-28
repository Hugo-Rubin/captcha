using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    public class PredictCaptchaCAM : PredictNeuralNetwork<PredictCaptchaCAM>
    {
        protected override int HiddenUnits
        {
            get { return 86; }
        }

        protected override int TamanhoCaracter
        {
            get { return 2025; }
        }

        protected override char[] Dicionario
        {
            get
            {
                return new[]
                           {
                               '2', '3', '4', '5', '6', '7', '8', 'b', 'c', 'd', 'e', 'f', 'g', 'm', 'n', 'p', 'w', 'x', 'y'
                           };
            }
        }

        public override string SiglaServico
        {
            get { return "CAM"; }
        }
    }
}