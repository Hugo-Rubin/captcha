using Core.Logic;
using Core.Logic.Predict;
using WebCommon;

namespace WebRJ
{
    public class Global : BaseHttpApplication
    {
        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaRJ.Instance; }
        }
    }
}