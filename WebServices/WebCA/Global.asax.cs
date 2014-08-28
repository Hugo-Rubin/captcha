#region

using System;
using System.Web;
using System.Web.Caching;
using Core.Logic;
using Core.Logic.Predict;

#endregion

namespace WebCA
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ServerLog.Append("Aplicação Consignações Aeronáutica/Marinha foi iniciada.");
            if (HttpContext.Current.Cache["RedeCAM"] == null)
            {
                HttpContext.Current.Cache.Insert("RedeCAM", PredictCaptchaCAM.Instance,
                    new CacheDependency(PredictCaptchaCAM.Instance.CacheDependencyFile));
                ServerLog.Append("O PredictConsigAM foi carregado em cache.");
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}