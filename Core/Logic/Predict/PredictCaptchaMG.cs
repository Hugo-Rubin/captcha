using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    public class PredictCaptchaMG : PredictNeuralNetwork<PredictCaptchaMG>
    {
        protected override int HiddenUnits
        {
            get { return 113; }
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
                               '2', '3', '4', '5', '6', '7', '8', 'a', 'b', 'c', 'd',
                               'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p', 's', 'u',
                               'w', 'x', 'y'
                           };
            }
        }

        public override string SiglaServico
        {
            get { return "MG"; }
        }
    }
}