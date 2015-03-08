using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WebCommon;
using Core.Logic.Captchas;

namespace WebESAJ
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class OCRESAJ : BaseWebService<CaptchaESAJ>
    {
        protected override string CachePredictItemName
        {
            get { return Global.PredictObjectCacheName; }
        }
    }
}
