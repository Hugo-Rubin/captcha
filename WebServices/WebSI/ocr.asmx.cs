using System;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Services;
using Core.Common;
using Core.Data;
using Core.Logic;
using Core.Logic.Captchas;
using Core.Logic.Types;
using WebCommon.Logging;

namespace WebSI
{
    [WebService(Namespace = "http://localhost:9045", Name = "OCRSI")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class OCRSI : WebService
    {
        private readonly ocrdbEntities db = new ocrdbEntities();
        private readonly IWebLog log = new WebLog();

        [WebMethod]
        // ReSharper disable InconsistentNaming
        public string GetText(int[] NanoImg, int w, int h, string Token)
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

                var nano = new NanoArray(NanoImg.CreateBitmapFromNanoArray(w, h));
                var captcha = new CaptchaSI(nano);

                try
                {
                    palavra = captcha.Reconhecer();
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