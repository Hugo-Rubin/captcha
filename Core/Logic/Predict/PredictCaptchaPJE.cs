using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    public class PredictCaptchaPJE : PredictNeuralNetwork<PredictCaptchaPJE>
    {
        protected override int HiddenUnits
        {
            get { return 41; }
        }

        protected override int TamanhoCaracter
        {
            get { return 625; }
        }

        protected override char[] Dicionario
        {
            get
            {
                return new[]
                            {
                                '1', '2', '3', '4', '5', '6', '7', '8', '9'
                            };
            }
        }

        public override string SiglaServico
        {
            get { return "PJE"; }

        }
    }

}

