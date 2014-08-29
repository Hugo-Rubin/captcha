using System.ComponentModel;
using System.Web.Services;
using Core.Logic.Captchas;
using WebCommon;

namespace WebCA
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class OCRCA : BaseWebService<CaptchaCA>
    {
        protected override string CachePredictItemName
        {
            get { return Global.PredictObjectCacheName; }
        }
    }
}