using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    public class PredictCaptchaESAJ : PredictNeuralNetwork<PredictCaptchaESAJ>
    {
        protected override int HiddenUnits
        {
            get { return 203; }
        }

        protected override int TamanhoCaracter
        {
            get { return 400; }
        }

        protected override char[] Dicionario
        {
            get
            {
                return new[]
                            {
                                'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F', 'h', 'H', 'i',
                                'j', 'J', 'k', 'K', 'm', 'M', 'n', 'N', 'p', 'P', 'q', 'Q', 'r', 'R', 's',
                                'S', 't', 'T', 'u', 'U', 'v', 'V', 'w', 'W', 'x', 'X', 'y', 'Y', 'z', 'Z'
                            };
            }
        }

        public override string SiglaServico
        {
            get { return "ESAJ"; }

        }
    }

}

