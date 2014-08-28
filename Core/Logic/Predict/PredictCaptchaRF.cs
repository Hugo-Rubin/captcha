using System;
using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    public class PredictCaptchaRf : PredictNeuralNetwork
    {
        protected override int HiddenUnits
        {
            get { return 162; }
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
                               '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b',
                               'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
                               'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
                           };
            }
        }

        #region Implementação de Singleton

        protected static volatile PredictCaptchaRf instance;
        protected static object SyncRoot = new Object();

        private PredictCaptchaRf()
        {
        }

        public static PredictCaptchaRf Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new PredictCaptchaRf();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        public override string SiglaServico
        {
            get { return "RF"; }
        }
    }
}