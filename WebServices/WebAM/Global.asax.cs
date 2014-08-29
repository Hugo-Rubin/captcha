using Core.Logic;
using Core.Logic.Predict;
using WebCommon;

namespace WebAM
{
    public class Global : BaseHttpApplication
    {
        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaAM.Instance; }
        }
    }
}