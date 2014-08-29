using System;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Services;
using Core.Common;
using Core.Data;
using Core.Logic;
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

        // GetText deve receber imagem colorida!
        [WebMethod]
        // ReSharper disable InconsistentNaming
        public virtual string GetText(byte[] ImagemColorida, int w, int h, string Token)
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

                var bmp = ImagemColorida.CreateBitmap();

                var captcha = (Captcha)Activator.CreateInstance(typeof(T), new object[] { bmp });

                try
                {
                    palavra = Rede.Recognize(captcha.GetCaracteres());
                    Log.GravarLogExecucao(Token, ip, palavra);
                }
                catch (Exception exception)
                {
                    ServerLog.AppendErrorLog(exception.Message, new ImgArray(10, 10).ToBitmap());
                    throw;
                }
                finally
                {
                    Log.GravarRequisicao(cliente.id, captcha, palavra);
                }
            }

            return palavra;
        }
        protected Clientes ListarClientePeloToken(String token)
        {
            var clientes = (from c in db.Clientes
                            where c.Token == token
                            select c).FirstOrDefault();
            return clientes;
        }
    }
}
