using System.ComponentModel;
using System.Web.Services;
using Core.Logic.Captchas;
using WebCommon;

namespace WebAM
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class OCRAM : BaseWebService<CaptchaAM>
    {
        protected override string CachePredictItemName
        {
            get { return Global.PredictObjectCacheName; }
        }
    }
}