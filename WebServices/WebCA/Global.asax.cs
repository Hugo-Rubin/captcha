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
        public const string PredictObjectCacheName = "RedeCAM";

        protected void Application_Start(object sender, EventArgs e)
        {
            ServerLog.Append("Aplicação Consignações Aeronáutica/Marinha foi iniciada.");
            if (HttpContext.Current.Cache[PredictObjectCacheName] != null)
            {
                return;
            }
            HttpContext.Current.Cache.Insert(PredictObjectCacheName, PredictCaptchaCAM.Instance,
                new CacheDependency(PredictCaptchaCAM.Instance.CacheDependencyFile));
            ServerLog.Append(PredictObjectCacheName + " foi carregado em cache.");
        }
    }
}