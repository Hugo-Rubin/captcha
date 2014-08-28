using System;
using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    public class PredictCaptchaCRJVerde : PredictNeuralNetwork
    {
        protected override int HiddenUnits
        {
            get { return 274; }
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
                               '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'A', 'b', 'B',
                               'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F', 'g', 'G', 'h', 'H', 'i', 'I',
                               'j', 'J', 'k', 'K', 'l', 'L', 'm', 'M', 'n', 'N', 'o', 'p', 'P', 'q',
                               'Q', 'r', 'R', 's', 'S', 't', 'T', 'u', 'U', 'v', 'V', 'w', 'W', 'x',
                               'X', 'y', 'Y', 'z', 'Z'
                           };
            }
        }

        protected override string ChaveTheta1
        {
            get { return "verde_theta1"; }
        }

        protected override string ChaveTheta2
        {
            get { return "verde_theta2"; }
        }

        #region Implementação de Singleton

        protected static volatile PredictCaptchaCRJVerde instance;
        protected static object SyncRoot = new Object();

        private PredictCaptchaCRJVerde()
        {
        }

        public static PredictCaptchaCRJVerde Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new PredictCaptchaCRJVerde();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        public override string SiglaServico
        {
            get { return "CRJv"; }
        }
    }
}