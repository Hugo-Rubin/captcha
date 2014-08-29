using Core.Logic.Predict;
using Core.Logic.Predict.Abstract;
using WebCommon;

namespace WebMG
{
    public class Global : BaseHttpApplication
    {
        public override string PredictObjectCacheName
        {
            get { return "RedeMG"; }
        }

        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaMG.Instance; }
        }
    }
}