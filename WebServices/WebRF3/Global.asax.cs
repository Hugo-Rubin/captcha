using Core.Logic;
using Core.Logic.Predict;
using WebCommon;

namespace WebRF3
{
    public class Global : BaseHttpApplication
    {
        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaRF3.Instance; }
        }
    }
}