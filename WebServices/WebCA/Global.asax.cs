using Core.Logic;
using Core.Logic.Predict;
using WebCommon;

namespace WebCA
{
    public class Global : BaseHttpApplication
    {
        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaCAM.Instance; }
        }
    }
}