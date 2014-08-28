#region

using System;
using System.Web;
using System.Web.Caching;
using Core.Logic;
using Core.Logic.Predict;

#endregion

namespace WebCM
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ServerLog.Append("Aplicação Consignações Marinha foi iniciada.");
            if (HttpContext.Current.Cache["RedeCAM"] == null)
            {
                HttpContext.Current.Cache.Insert("RedeCAM", PredictCaptchaCAM.Instance,
                    new CacheDependency(PredictCaptchaCAM.Instance.CacheDependencyFile));
                ServerLog.Append("O PredictConsigAM foi carregado em cache.");
            }
        }


        private void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
        }

        private void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
        }

        private void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
        }

        private void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
        }
    }
}