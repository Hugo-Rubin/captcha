using System;
using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    public class PredictCaptchaCRJ : PredictNeuralNetwork
    {
        protected override int HiddenUnits
        {
            get { return 234; }
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
                               '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'A', 'b', 'B',
                               'c', 'd', 'D', 'e', 'E', 'f', 'F', 'g', 'G', 'h', 'H', 'i', 'j',
                               'J', 'k', 'l', 'L', 'm', 'M', 'n', 'N', 'p', 'P', 'q', 'Q', 'r',
                               'R', 's', 'S', 't', 'T', 'u', 'U', 'v', 'w', 'x', 'y', 'Y', 'z'
                           };
            }
        }

        #region Implementação de Singleton

        protected static volatile PredictCaptchaCRJ instance;
        protected static object SyncRoot = new Object();

        private PredictCaptchaCRJ()
        {
        }

        public static PredictCaptchaCRJ Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new PredictCaptchaCRJ();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        public override string SiglaServico
        {
            get { return "CRJ"; }
        }
    }
}