using System.ComponentModel;
using System.Web.Services;
using Core.Logic.Captchas;
using WebCommon;

namespace WebRJ
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class OCRRJ : BaseWebService<CaptchaRJ>
    {
        protected override string CachePredictItemName
        {
            get { return "RedeRJ"; }
        }
    }
}