using System;
using System.Web;
using System.Web.Caching;
using Core.Logic;

namespace WebCommon
{
    public abstract class BaseHttpApplication : HttpApplication
    {
        protected abstract ICacheable PredictInstance { get; }

        public const string PredictObjectCacheName = "PredictCacheObj";

        protected void Application_Start(object sender, EventArgs e)
        {
            ServerLog.Append("Aplicação foi iniciada.");
            if (HttpContext.Current.Cache[PredictObjectCacheName] == null)
            {
                HttpContext.Current.Cache.Insert(PredictObjectCacheName, PredictInstance, new CacheDependency(PredictInstance.CacheDependencyFile));
                ServerLog.Append("O PredictMG foi carregado em cache.");
            }
        }
    }
}
