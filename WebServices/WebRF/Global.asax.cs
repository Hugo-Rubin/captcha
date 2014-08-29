using Core.Logic;
using Core.Logic.Predict;
using WebCommon;

namespace WebRF
{
    public class Global : BaseHttpApplication
    {
        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaRF.Instance; }
        }
    }
}