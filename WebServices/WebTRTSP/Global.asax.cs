using Core.Logic;
using Core.Logic.Predict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using WebCommon;

namespace WebTRTSP
{
    public class Global : BaseHttpApplication
    {
        protected override ICacheable PredictInstance
        {
            get { return PredictCaptchaTRTSP.Instance; }
        }
    }
}