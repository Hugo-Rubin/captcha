#region

using System;
using System.Web;
using System.Web.Caching;
using Core.Logic;
using Core.Logic.Predict;

#endregion

namespace WebRJ
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ServerLog.Append("Aplicação Sintegra RJ foi iniciada.");

            if (HttpContext.Current.Cache["RedeRJ"] == null)
            {
                HttpContext.Current.Cache.Insert("RedeRJ", PredictCaptchaRJ.Instance,
                    new CacheDependency(PredictCaptchaRJ.Instance.CacheDependencyFile));
                ServerLog.Append("O PredictRJ foi carregado em cache.");
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