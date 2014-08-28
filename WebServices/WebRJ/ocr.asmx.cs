#region

using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using Core.Common;
using Core.Data;
using Core.Logic;
using Core.Logic.Captchas;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Predict;
using Core.Logic.Types;
using WebCommon.Logging;

#endregion

namespace WebRJ
{
    /// <summary>
    ///     Summary description for OCRRJ
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class OCRRJ : WebService
    {
        private readonly ocrdbEntities db = new ocrdbEntities();
        private readonly PredictCaptchaRJ rede = (PredictCaptchaRJ) HttpContext.Current.Cache["RedeRJ"];

        private readonly IWebLog log = new WebLog();

        // GetText deve receber imagem colorida!
        [WebMethod]
        // ReSharper disable InconsistentNaming
        public string GetText(byte[] ImagemColorida, int w, int h, string Token)
            // ReSharper restore InconsistentNaming
        {
            var palavra = "!!!!";
            var cliente = ListarClientePeloToken(Token);
            if (cliente == null)
            {
                palavra = "Licença inválida!";
            }
            else
            {
                var ip = HttpContext.Current.Request.UserHostAddress;

                var bmp = (Bitmap) ImagemColorida.ToImage();
                var captcha = new CaptchaRJ(bmp);

                try
                {
                    palavra = rede.Recognize(captcha.GetCaracteresImgArray());
                    log.GravarLogExecucao(Token, ip, palavra);
                }
                catch (Exception e)
                {
                    ServerLog.AppendErrorLog(e.Message, new ImgArray(10, 10).ToBitmap());
                    throw;
                }
                finally
                {
                    log.GravarRequisicao(cliente.id, captcha, palavra);
                }
            }

            return palavra;
        }
        
        private Clientes ListarClientePeloToken(String token)
        {
            var clientes = (from c in db.Clientes
                where c.Token == token
                select c).FirstOrDefault();
            return clientes;
        }
    }
}