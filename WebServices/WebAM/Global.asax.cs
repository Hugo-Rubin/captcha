using Core.Logic.Predict;
using Core.Logic.Predict.Abstract;
using WebCommon;

namespace WebAM
{
    public class Global : BaseHttpApplication
    {
        public override string PredictObjectCacheName
        {
            get { return "RedeAM"; }
        }

        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaAM.Instance; }
        }
    }
}