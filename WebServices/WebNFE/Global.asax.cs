using Core.Logic;
using Core.Logic.Predict;
using WebCommon;

namespace WebNFE
{
    public class Global : BaseHttpApplication
    {
        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaNFE.Instance; }
        }
    }
}