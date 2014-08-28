using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    public class PredictCaptchaSP : PredictNeuralNetwork<PredictCaptchaSP>
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
                               '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'A', 'b', 'B',
                               'c', 'd', 'D', 'e', 'E', 'f', 'F', 'g', 'G', 'h', 'H', 'i',
                               'I', 'j', 'J', 'k', 'm', 'M', 'n', 'N', 'p', 'P', 'q', 'Q',
                               'r', 'R', 's', 't', 'T', 'u', 'U', 'v', 'w', 'x', 'y', 'Y', 'z'
                           };
            }
        }

        public override string SiglaServico
        {
            get { return "SP"; }
        }
    }
}