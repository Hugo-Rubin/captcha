using Core.Logic.Predict;
using Core.Logic.Predict.Abstract;
using WebCommon;

namespace WebSP
{
    public class Global : BaseHttpApplication
    {
        public override string PredictObjectCacheName
        {
            get { return "RedeSP"; }
        }

        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaSP.Instance; }
        }
    }
}