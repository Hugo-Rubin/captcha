using Core.Logic.Predict;
using Core.Logic.Predict.Abstract;
using WebCommon;

namespace WebNFE
{
    public class Global : BaseHttpApplication
    {
        public override string PredictObjectCacheName
        {
            get { return "RedeNFE"; }
        }

        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaNFE.Instance; }
        }
    }
}