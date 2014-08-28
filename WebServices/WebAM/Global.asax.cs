#region

using System;
using System.Web;
using System.Web.Caching;
using Core.Logic;
using Core.Logic.Predict;

#endregion

namespace WebAM
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ServerLog.Append("Aplicação Sintegra AM foi iniciada.");
            if (HttpContext.Current.Cache["RedeAM"] == null)
            {
                HttpContext.Current.Cache.Insert("RedeAM", PredictCaptchaAM.Instance,
                    new CacheDependency(PredictCaptchaAM.Instance.CacheDependencyFile));
                ServerLog.Append("O PredictAM foi carregado em cache.");
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