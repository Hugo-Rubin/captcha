using System;
using Core.Logic.Predict.Abstract;

namespace Core.Logic.Predict
{
    /// <summary>
    ///   Classe de predição do captcha do siscarga
    /// </summary>
    public class PredictCaptchaSi : PredictTemplateMatching
    {
        #region Implementação de Singleton

        protected static volatile PredictCaptchaSi instance;
        protected static object SyncRoot = new Object();

        private PredictCaptchaSi()
        {
        }

        public static PredictCaptchaSi Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new PredictCaptchaSi();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        public override string SiglaServico
        {
            get { return "SI"; }
        }
    }
}