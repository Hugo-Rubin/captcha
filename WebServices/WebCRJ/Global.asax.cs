#region

using System;
using System.Web;
using System.Web.Caching;
using Core.Logic;
using Core.Logic.Predict;

#endregion

namespace WebCRJ
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ServerLog.Append("Aplicação Consignações RJ foi iniciada.");
            HttpContext.Current.Cache.Remove("RedeCRJ");
            if (HttpContext.Current.Cache["RedeCRJ"] == null)
            {
                HttpContext.Current.Cache.Insert("RedeCRJ", PredictCaptchaCRJ.Instance,
                    new CacheDependency(PredictCaptchaCRJ.Instance.CacheDependencyFile));
                ServerLog.Append("O PredictConsigRJ foi carregado em cache.");
            }
            if (HttpContext.Current.Cache["RedeCRJAzul"] == null)
            {
                HttpContext.Current.Cache.Insert("RedeCRJAzul", PredictCaptchaCRJAzul.Instance,
                    new CacheDependency(PredictCaptchaCRJAzul.Instance.CacheDependencyFile));
                ServerLog.Append("O PredictAzulConsigRJ foi carregado em cache.");
            }
            if (HttpContext.Current.Cache["RedeCRJVerde"] == null)
            {
                HttpContext.Current.Cache.Insert("RedeCRJVerde", PredictCaptchaCRJVerde.Instance,
                    new CacheDependency(PredictCaptchaCRJVerde.Instance.CacheDependencyFile));
                ServerLog.Append("O PredictVerdeConsigRJ foi carregado em cache.");
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