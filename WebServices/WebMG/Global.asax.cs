#region

using System;
using System.Web;
using System.Web.Caching;
using Core.Logic;
using Core.Logic.Predict;

#endregion

namespace WebMG
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ServerLog.Append("Aplicação Sintegra MG foi iniciada.");
            if (HttpContext.Current.Cache["RedeMG"] == null)
            {
                HttpContext.Current.Cache.Insert("RedeMG", PredictCaptchaMG.Instance,
                    new CacheDependency(PredictCaptchaMG.Instance.CacheDependencyFile));
                ServerLog.Append("O PredictMG foi carregado em cache.");
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