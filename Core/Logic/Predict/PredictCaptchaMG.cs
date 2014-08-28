using System;
using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    public class PredictCaptchaMG : PredictNeuralNetwork
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

        #region Implementação de Singleton

        protected static volatile PredictCaptchaMG instance;
        protected static object SyncRoot = new Object();

        private PredictCaptchaMG()
        {
        }

        public static PredictCaptchaMG Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new PredictCaptchaMG();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        public override string SiglaServico
        {
            get { return "MG"; }
        }
    }
}