using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using Core.Common;
using Core.Data;
using Core.Logic.Captchas.Abstract;
using Core.Logic.Predict.Abstract;
using Core.Logic.Types;
using WebCommon.Logging;

namespace WebCommon
{
    public abstract class BaseWebService<T> where T : Captcha
    {
        private readonly ocrdbEntities db = new ocrdbEntities();
        protected readonly IWebLog Log = new WebLog();

        protected IPredict Rede
        {
            get { return (IPredict)HttpContext.Current.Cache[CachePredictItemName]; }
        }

        protected abstract string CachePredictItemName { get; }

        [WebMethod]
        public string GetTextFromNano(int[] nanoImg, int w, int h, string token)
        {
            return GetText(nanoImg: nanoImg, rawImage: null, w: w, h: h, token: token);
        }

        // GetText deve receber imagem colorida!
        [WebMethod]
        public string GetText(byte[] rawImage, int w, int h, string token)
        {
            return GetText(nanoImg: null, rawImage: rawImage, w: w, h: h, token: token);
        }


        protected Clientes ListarClientePeloToken(String token)
        {
            var clientes = (from c in db.Clientes
                            where c.Token == token
                            select c).FirstOrDefault();
            return clientes;
        }

        private string GetText(int[] nanoImg, byte[] rawImage, int w, int h, string token)
        {
            var cliente = ListarClientePeloToken(token);
            if (cliente == null)
            {
                return "Licença inválida!";
            }

            var palavra = "!!!!";
            var ip = HttpContext.Current.Request.UserHostAddress;
            Captcha captcha;
            if (nanoImg != null)
            {
                var nano = new NanoArray(nanoImg.BitmapFromNanoArray(w, h));
                captcha = (Captcha)Activator.CreateInstance(typeof(T), new object[] { nano });
            }
            else
            {
                captcha = (Captcha)Activator.CreateInstance(typeof(T), new object[] { rawImage.ToBitmap() });
            }
            
            try
            {
                var caracteres = captcha.GetCaracteres();
                palavra = Rede.Recognize(caracteres);
                Log.GravarLogExecucao(token, ip, palavra);
            }
            finally
            {
                Log.GravarRequisicao(cliente.id, captcha, palavra);
            }

            return palavra;
        }
    }
}
