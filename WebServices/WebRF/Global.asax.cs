using Core.Logic.Predict;
using Core.Logic.Predict.Abstract;
using WebCommon;

namespace WebRF
{
    public class Global : BaseHttpApplication
    {
        public override string PredictObjectCacheName()
        {
            return "RedeRF";
        }

        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaRF.Instance; }
        }
    }
}