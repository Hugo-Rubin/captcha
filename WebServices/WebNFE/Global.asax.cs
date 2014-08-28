#region

using System;
using System.Web;
using System.Web.Caching;
using Core.Logic;
using Core.Logic.Predict;

#endregion

namespace WebNFE
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ServerLog.Append("WebNFE instanciou o PredictCaptchaNFE.");

            if (HttpContext.Current.Cache["RedeNFE"] == null)
            {
                HttpContext.Current.Cache.Insert("RedeNFE", PredictCaptchaNFE.Instance,
                    new CacheDependency(PredictCaptchaNFE.Instance.CacheDependencyFile));
                ServerLog.Append("O PredictNFE foi carregado em cache.");
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