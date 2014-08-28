using System;
using System.Web;
using System.Web.Caching;
using Core.Logic;
using Core.Logic.Predict;

namespace WebAM
{
    public class Global : HttpApplication
    {
        public const string PredictObjectCacheName = "RedeAM";

        protected void Application_Start(object sender, EventArgs e)
        {
            ServerLog.Append("Aplicação Sintegra AM foi iniciada.");
            if (HttpContext.Current.Cache[PredictObjectCacheName] != null)
            {
                return;
            }
            HttpContext.Current.Cache.Insert(PredictObjectCacheName, PredictCaptchaMG.Instance,
                new CacheDependency(PredictCaptchaMG.Instance.CacheDependencyFile));
            ServerLog.Append(PredictObjectCacheName + " foi carregado em cache.");
        }
    }
}