using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{

    //Atencao: Apesar de ser a mesma coisa que PredictCaptchaRF nao podemos herdar dela ja que ela implementa um generico de RF e nao de RF3
    public class PredictCaptchaRF3 : PredictNeuralNetwork<PredictCaptchaRF3>
    {
        protected override int HiddenUnits
        {
            get { return 162; }
        }

        protected override int TamanhoCaracter
        {
            get { return 1350; }
        }

        protected override char[] Dicionario
        {
            get
            {
                return new[]
                           {
                               '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b',
                               'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
                               'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
                           };
            }
        }

        public override string SiglaServico
        {
            get { return "RF3"; }
        }
    }
}
