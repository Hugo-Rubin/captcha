using System.Web.Services;
using Core.Logic.Captchas;
using WebCommon;

namespace WebRF
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class OCRRF : BaseWebService<CaptchaRF>
    {        
        protected override string CachePredictItemName
        {
            get { return Global.PredictObjectCacheName(); }
        }
    }
}
