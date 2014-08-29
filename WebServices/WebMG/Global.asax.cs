using Core.Logic;
using Core.Logic.Predict;
using WebCommon;

namespace WebMG
{
    public class Global : BaseHttpApplication
    {
        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaMG.Instance; }
        }
    }
}