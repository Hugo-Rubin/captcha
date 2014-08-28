using System;
using Core.Logic.Predict.Abstract;
using Core.Logic.Types;
using Core.Logic.Utils;

namespace Core.Logic.Predict
{
    public class PredictPadraoCaptchaNFE : PredictNeuralNetwork
    {
        //TODO: Esse método precisou ser sobrescrito porque a rede NFE foi treinada invertida
        // Remover esse método assim que retreinar a a rede NFE

        protected override int HiddenUnits
        {
            get { return 20; }
        }

        protected override int TamanhoCaracter
        {
            get { return 18000; }
        }

        protected override char[] Dicionario
        {
            get { return new[] { 'b', 'd' }; }
        }

        protected override string ChaveTheta1
        {
            get { return "ArquivoTheta1Padroes_NFE"; }
        }

        protected override string ChaveTheta2
        {
            get { return "ArquivoTheta2Padroes_NFE"; }
        }

        #region Implementação de Singleton

        protected static volatile PredictPadraoCaptchaNFE instance;
        protected static object SyncRoot = new Object();

        private PredictPadraoCaptchaNFE()
        {
        }

        public static PredictPadraoCaptchaNFE Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new PredictPadraoCaptchaNFE();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        public override string SiglaServico
        {
            get { return "NFE"; }
        }

        public override char Recognize(ImgArray caracter)
        {
            if (caracter == null)
            {
                throw new ArgumentNullException("caracter");
            }
            var idx = Predict(caracter.PixelIntensityParaRedeInvertida());
            return Dicionario[idx];
        }
    }
}