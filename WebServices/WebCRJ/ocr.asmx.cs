using System;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Services;
using Core.Common;
using Core.Data;
using Core.Logic;
using Core.Logic.Captchas;
using Core.Logic.Predict;
using Core.Logic.Types;
using WebCommon.Logging;

//TODO Avaliar se eh mesmo necessario usar varios predicts para este caso...

namespace WebCRJ
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class OCRCRJ : WebService
    {
        private readonly ocrdbEntities db = new ocrdbEntities();
        private readonly PredictCaptchaCRJ rede = (PredictCaptchaCRJ) HttpContext.Current.Cache["RedeCRJ"];

        private readonly PredictCaptchaCRJAzul redeAzul =
            (PredictCaptchaCRJAzul) HttpContext.Current.Cache["RedeCRJAzul"];

        private readonly PredictCaptchaCRJVerde redeVerde =
            (PredictCaptchaCRJVerde) HttpContext.Current.Cache["RedeCRJVerde"];

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

                var bmp = ImagemColorida.CreateBitmap();
                var captcha = new CaptchaCRJ(bmp);

                try
                {
                    switch (captcha.Padrao)
                    {
                        case TipoPadraoConsigRJ.Cinza:
                        {
                            palavra = rede.Recognize(captcha.GetCaracteres());
                            break;
                        }
                        case TipoPadraoConsigRJ.Azul:
                        {
                            palavra = redeAzul.Recognize(captcha.GetCaracteres());
                            break;
                        }
                        case TipoPadraoConsigRJ.Verde:
                        {
                            palavra = redeVerde.Recognize(captcha.GetCaracteres());
                            break;
                        }
                    }

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