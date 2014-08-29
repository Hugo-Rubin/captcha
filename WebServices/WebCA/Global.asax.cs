using Core.Logic.Predict;
using Core.Logic.Predict.Abstract;
using WebCommon;

namespace WebCA
{
    public class Global : BaseHttpApplication
    {
        public override string PredictObjectCacheName
        {
            get { return "RedeCAM"; }
        }

        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaCAM.Instance; }
        }
    }
}