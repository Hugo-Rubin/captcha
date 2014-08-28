using System;
using System.Web;
using System.Web.Caching;
using Core.Logic;
using Core.Logic.Predict;

namespace WebMG
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ServerLog.Append("Aplicação Sintegra MG foi iniciada.");
            if (HttpContext.Current.Cache["RedeMG"] != null)
            {
                return;
            }
            HttpContext.Current.Cache.Insert("RedeMG", PredictCaptchaMG.Instance,
                new CacheDependency(PredictCaptchaMG.Instance.CacheDependencyFile));
            ServerLog.Append("O PredictMG foi carregado em cache.");
        }
    }
}