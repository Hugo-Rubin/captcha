using System.ComponentModel;
using System.Web.Services;
using Core.Logic.Captchas;
using WebCommon;

namespace WebNFE
{
    [WebService(Namespace = "http://www.ml-research.com/ocr/", Name = "OCRNFE")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class OCRNFE : BaseWebService<CaptchaNFE>
    {
        protected override string CachePredictItemName
        {
            get { return Global.PredictObjectCacheName; }
        }
    }
}