using System.Web.Services;
using Core.Logic.Captchas;
using WebCommon;

namespace WebRF4
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class OCRRF4 : BaseWebService<CaptchaRF4>
    {
        protected override string CachePredictItemName
        {
            get { return Global.PredictObjectCacheName; }
        }
    }
}
