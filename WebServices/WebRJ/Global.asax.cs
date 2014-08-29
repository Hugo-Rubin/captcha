using Core.Logic.Predict;
using Core.Logic.Predict.Abstract;
using WebCommon;

namespace WebRJ
{
    public class Global : BaseHttpApplication
    {
        public override string PredictObjectCacheName
        {
            get { return "RedeRJ"; }
        }

        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaRJ.Instance; }
        }
    }
}