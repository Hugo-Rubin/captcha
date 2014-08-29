using Core.Logic;
using Core.Logic.Predict;
using WebCommon;

namespace WebSP
{
    public class Global : BaseHttpApplication
    {
        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaSP.Instance; }
        }
    }
}