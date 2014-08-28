#region

using System;
using System.Web;
using Core.Logic;
using Core.Logic.Predict;

#endregion

namespace WebSP
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ServerLog.Append("WebAppOCRTipo3 instanciou o PredictCaptchaSp.");
            Application.Add("Rede", PredictCaptchaSP.Instance);
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